import fs from "node:fs/promises";
import path from "node:path";

const baseUrl = process.env.VOXCPM_URL || "http://127.0.0.1:9000";
const presetName = process.env.VOXCPM_PRESET || "oldding";
const defaultPreset = "--- 请选择预设音频 ---";
const promptText =
  process.env.VOXCPM_PROMPT ||
  "本次演示将带您了解核心操作，画面中的每一步都对应实际反馈，请注意观察左侧菜单变化以及右侧的数据更新，我们开始吧。";

function parseArgs(argv) {
  const options = {
    input: "",
    outputDir: "",
    only: "",
    from: "",
    subtitle: true,
    speed: 1,
  };

  for (let index = 0; index < argv.length; index += 1) {
    const key = argv[index];
    if (key === "--input") options.input = argv[++index];
    else if (key === "--output-dir") options.outputDir = argv[++index];
    else if (key === "--only") options.only = argv[++index];
    else if (key === "--from") options.from = argv[++index];
    else if (key === "--speed") options.speed = Number(argv[++index]);
    else if (key === "--no-subtitle") options.subtitle = false;
    else throw new Error(`Unknown argument: ${key}`);
  }

  if (!options.input || !options.outputDir) {
    throw new Error(
      "Usage: node voxcpm-generate.mjs --input voice-chunks.txt --output-dir voice [--only 01]",
    );
  }

  return options;
}

async function callEndpoint(endpoint, payload = {}) {
  const queuedResponse = await fetch(
    `${baseUrl}/gradio_api/call/v2/${endpoint}`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify(payload),
    },
  );
  if (!queuedResponse.ok) {
    throw new Error(`${endpoint} queue request failed: ${queuedResponse.status}`);
  }

  const queued = await queuedResponse.json();
  if (!queued.event_id) throw new Error(`${endpoint} returned no event id`);

  const eventResponse = await fetch(
    `${baseUrl}/gradio_api/call/${endpoint}/${queued.event_id}`,
  );
  if (!eventResponse.ok) {
    throw new Error(`${endpoint} event request failed: ${eventResponse.status}`);
  }

  const sse = await eventResponse.text();
  const blocks = sse.split(/\r?\n\r?\n/).filter(Boolean);
  const complete = [...blocks]
    .reverse()
    .find((block) => block.startsWith("event: complete"));
  if (!complete) {
    throw new Error(`${endpoint} did not complete:\n${sse.slice(-2000)}`);
  }

  const dataLine = complete
    .split(/\r?\n/)
    .find((line) => line.startsWith("data: "));
  return JSON.parse(dataLine.slice(6));
}

async function getPresetAudio() {
  const result = await callEndpoint("_on_preset_change", { name: presetName });
  const update = result[0];
  const audio = update?.value || update;
  if (!audio?.path) throw new Error(`Preset not found: ${presetName}`);
  return audio;
}

function generatePayload(text, audio, options) {
  const payload = {
    text,
    srt_file: null,
    ref_wav: audio,
    prompt_text_value: promptText,
    cfg: 2.6,
    normalize: false,
    denoise: false,
    steps: 15,
    speed: options.speed,
    subtitle: options.subtitle,
    multi_role: false,
    srt_mode: false,
  };

  for (let row = 0; row < 8; row += 1) {
    const start = 12 + row * 3;
    payload[`param_${start}`] = "";
    payload[`param_${start + 1}`] = defaultPreset;
    payload[`param_${start + 2}`] = 1;
  }

  return payload;
}

function pollAudio(result) {
  const candidates = result
    .map((value) => value?.value || value)
    .filter((value) => value && typeof value === "object" && value.path);
  return candidates.find((value) =>
    /\.(wav|mp3|m4a|flac|ogg)$/i.test(value.path),
  );
}

function pollSubtitle(result) {
  return result
    .map((value) => value?.value || value)
    .find((value) => value?.path?.endsWith(".srt"));
}

async function waitForNewAudio(previousPath, timeoutMs = 20 * 60 * 1000) {
  const started = Date.now();
  while (Date.now() - started < timeoutMs) {
    const result = await callEndpoint("_poll");
    const audio = pollAudio(result);
    const summary = typeof result[0] === "string" ? result[0] : "";
    process.stdout.write(`\r${summary.replaceAll("`", "")}   `);

    if (audio?.path && audio.path !== previousPath) {
      process.stdout.write("\n");
      return { audio, subtitle: pollSubtitle(result) };
    }
    if (/失败：[1-9]/.test(summary)) {
      throw new Error(`VoxCPM task failed: ${summary}`);
    }
    await new Promise((resolve) => setTimeout(resolve, 3000));
  }
  throw new Error("Timed out waiting for VoxCPM output");
}

async function copyResult(fileData, destination) {
  if (!fileData?.path) return;
  await fs.copyFile(fileData.path, destination);
}

const options = parseArgs(process.argv.slice(2));
const inputPath = path.resolve(options.input);
const outputDir = path.resolve(options.outputDir);
await fs.mkdir(outputDir, { recursive: true });

const chunks = (await fs.readFile(inputPath, "utf8"))
  .split(/\r?\n/)
  .map((line) => line.trim())
  .filter(Boolean)
  .map((line) => {
    const separator = line.indexOf("|");
    if (separator < 1) throw new Error(`Invalid chunk line: ${line}`);
    return { id: line.slice(0, separator), text: line.slice(separator + 1) };
  })
  .filter((chunk) => !options.only || chunk.id === options.only)
  .filter((chunk) => !options.from || chunk.id >= options.from);

if (!chunks.length) throw new Error("No voice chunks selected");

const presetAudio = await getPresetAudio();
let previous = pollAudio(await callEndpoint("_poll"))?.path || "";

for (const chunk of chunks) {
  console.log(`Generating ${chunk.id}: ${chunk.text}`);
  await callEndpoint("generate", generatePayload(chunk.text, presetAudio, options));
  const result = await waitForNewAudio(previous);
  previous = result.audio.path;

  const audioExtension = path.extname(result.audio.path) || ".wav";
  await copyResult(
    result.audio,
    path.join(outputDir, `${chunk.id}${audioExtension}`),
  );
  if (result.subtitle) {
    await copyResult(result.subtitle, path.join(outputDir, `${chunk.id}.srt`));
  }
}

console.log(`Saved ${chunks.length} chunk(s) to ${outputDir}`);
