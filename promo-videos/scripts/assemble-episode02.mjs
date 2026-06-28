import { execFileSync } from "node:child_process";
import fs from "node:fs";
import path from "node:path";

const root = path.resolve(process.argv[2] || ".");
const episode = path.join(root, "promo-videos", "episode-02");
const voiceDir = path.join(episode, "voice");
const cardsDir = path.join(episode, "cards");
const capturesDir = path.join(episode, "captures");
const outputDir = path.join(episode, "output");
const workDir = path.join(episode, "work");
const ffmpeg = process.env.FFMPEG || String.raw`D:\OpenCode\AI Note\.tools\ffmpeg\ffmpeg-8.1.1-essentials_build\bin\ffmpeg.exe`;
const ffprobe = process.env.FFPROBE || String.raw`D:\OpenCode\AI Note\.tools\ffmpeg\ffmpeg-8.1.1-essentials_build\bin\ffprobe.exe`;
const pace = 1.35;
const gap = 0.22;

for (const dir of [outputDir, workDir]) fs.mkdirSync(dir, { recursive: true });

function run(executable, args) {
  console.log([path.basename(executable), ...args].join(" "));
  execFileSync(executable, args, { stdio: "inherit" });
}

function duration(file) {
  return Number(execFileSync(ffprobe, [
    "-v", "error", "-show_entries", "format=duration",
    "-of", "default=noprint_wrappers=1:nokey=1", file,
  ], { encoding: "utf8" }).trim());
}

const concatPath = (file) => file.replaceAll("\\", "/").replaceAll("'", "'\\''");
const parseTime = (value) => {
  const match = value.match(/(\d+):(\d+):(\d+),(\d+)/);
  return Number(match[1]) * 3600 + Number(match[2]) * 60 + Number(match[3]) + Number(match[4]) / 1000;
};
const formatTime = (value) => {
  const ms = Math.max(0, Math.round(value * 1000));
  return `${String(Math.floor(ms / 3600000)).padStart(2, "0")}:${String(Math.floor(ms % 3600000 / 60000)).padStart(2, "0")}:${String(Math.floor(ms % 60000 / 1000)).padStart(2, "0")},${String(ms % 1000).padStart(3, "0")}`;
};

function offsetSrt(srt, offset, startIndex) {
  let index = startIndex;
  const output = [];
  for (const block of srt.trim().split(/\r?\n\r?\n/).filter(Boolean)) {
    const lines = block.split(/\r?\n/);
    const timingIndex = lines.findIndex((line) => line.includes("-->"));
    if (timingIndex < 0) continue;
    const [from, to] = lines[timingIndex].split(" --> ");
    output.push(`${index}\n${formatTime(parseTime(from) / pace + offset)} --> ${formatTime(parseTime(to) / pace + offset)}\n${lines.slice(timingIndex + 1).join("\n")}`);
    index += 1;
  }
  return { text: output.join("\n\n"), nextIndex: index };
}

const fastVoiceDir = path.join(workDir, "voice-fast");
fs.mkdirSync(fastVoiceDir, { recursive: true });
const chunks = Array.from({ length: 13 }, (_, index) => {
  const id = String(index + 1).padStart(2, "0");
  const source = path.join(voiceDir, `${id}.wav`);
  const audio = path.join(fastVoiceDir, `${id}.wav`);
  if (!fs.existsSync(source)) throw new Error(`Missing voice chunk: ${source}`);
  run(ffmpeg, ["-y", "-hide_banner", "-loglevel", "error", "-i", source, "-af", `atempo=${pace}`, "-ar", "48000", "-ac", "1", audio]);
  return { id, audio, srt: path.join(voiceDir, `${id}.srt`), duration: duration(audio) };
});

const silence = path.join(workDir, "silence.wav");
run(ffmpeg, ["-y", "-hide_banner", "-loglevel", "error", "-f", "lavfi", "-i", "anullsrc=r=48000:cl=mono", "-t", String(gap), "-c:a", "pcm_s16le", silence]);
const audioConcat = path.join(workDir, "audio-concat.txt");
const audioLines = [];
for (const [index, chunk] of chunks.entries()) {
  audioLines.push(`file '${concatPath(chunk.audio)}'`);
  if (index < chunks.length - 1) audioLines.push(`file '${concatPath(silence)}'`);
}
fs.writeFileSync(audioConcat, `${audioLines.join("\n")}\n`, "utf8");
const voiceMaster = path.join(outputDir, "episode02-voice.wav");
run(ffmpeg, ["-y", "-hide_banner", "-loglevel", "error", "-f", "concat", "-safe", "0", "-i", audioConcat, "-af", "loudnorm=I=-16:TP=-1.5:LRA=11", "-ar", "48000", "-ac", "1", voiceMaster]);

let subtitleOffset = 0;
let subtitleIndex = 1;
const subtitleBlocks = [];
for (const [index, chunk] of chunks.entries()) {
  if (fs.existsSync(chunk.srt)) {
    const shifted = offsetSrt(fs.readFileSync(chunk.srt, "utf8"), subtitleOffset, subtitleIndex);
    subtitleBlocks.push(shifted.text);
    subtitleIndex = shifted.nextIndex;
  }
  subtitleOffset += chunk.duration + (index < chunks.length - 1 ? gap : 0);
}
const subtitles = path.join(outputDir, "episode02.srt");
fs.writeFileSync(subtitles, `${subtitleBlocks.join("\n\n")}\n`, "utf8");

function prepare(source, destination, vertical = false) {
  const filter = vertical
    ? "scale=-2:1920,crop=1080:1920:0:0"
    : "[0:v]scale=3840:2160,boxblur=28:14[bg];[0:v]scale=3640:1956:force_original_aspect_ratio=decrease[fg];[bg][fg]overlay=(W-w)/2:(H-h)/2";
  run(ffmpeg, ["-y", "-hide_banner", "-loglevel", "error", "-i", source, vertical ? "-vf" : "-filter_complex", filter, "-frames:v", "1", destination]);
}

const captureNames = ["raw-top", "rendered-styles-lists", "rendered-tasks", "rendered-quote-code-latex", "rendered-code-latex"];
const screens = {};
for (const name of captureNames) {
  screens[name] = path.join(workDir, `${name}.png`);
  screens[`${name}-vertical`] = path.join(workDir, `${name}-vertical.png`);
  prepare(path.join(capturesDir, `${name}.jpg`), screens[name]);
  prepare(path.join(capturesDir, `${name}.jpg`), screens[`${name}-vertical`], true);
}

const imagePlan = [
  [[path.join(cardsDir, "title.png"), 1]],
  [[screens["raw-top"], 0.52], [screens["rendered-styles-lists"], 0.48]],
  [[screens["rendered-styles-lists"], 1]],
  [[screens["rendered-styles-lists"], 1]],
  [[screens["rendered-styles-lists"], 1]],
  [[screens["rendered-styles-lists"], 1]],
  [[screens["rendered-tasks"], 1]],
  [[screens["rendered-quote-code-latex"], 1]],
  [[screens["rendered-quote-code-latex"], 1]],
  [[screens["rendered-code-latex"], 1]],
  [[screens["rendered-code-latex"], 1]],
  [[path.join(cardsDir, "compare.png"), 0.38], [screens["raw-top"], 0.28], [screens["rendered-styles-lists"], 0.34]],
  [[path.join(cardsDir, "cta.png"), 1]],
];

function segmentsFor(plan) {
  return chunks.flatMap((chunk, index) => {
    const segmentDuration = chunk.duration + (index < chunks.length - 1 ? gap : 0);
    return plan[index].map(([file, ratio]) => [file, segmentDuration * ratio]);
  });
}

function renderStillVideo(segments, size, destination) {
  const inputFilters = segments.map(
    (_, index) => `[${index}:v]fps=30,scale=${size},setsar=1,format=yuv420p[v${index}]`,
  ).join(";");
  const concatFilter = segments.map((_, index) => `[v${index}]`).join("") +
    `concat=n=${segments.length}:v=1:a=0[v]`;
  run(ffmpeg, [
    "-y", "-hide_banner", "-loglevel", "error",
    ...segments.flatMap(([file, itemDuration]) => [
      "-loop", "1", "-t", itemDuration.toFixed(3), "-i", file,
    ]),
    "-filter_complex", `${inputFilters};${concatFilter}`,
    "-map", "[v]",
    "-c:v", "libx264", "-preset", "medium",
    "-crf", size.startsWith("3840") ? "19" : "20",
    "-pix_fmt", "yuv420p", destination,
  ]);
}

function mux(video, destination, timeLimit = "") {
  const args = ["-y", "-hide_banner", "-loglevel", "error", "-i", video, "-i", voiceMaster, "-i", subtitles, "-map", "0:v:0", "-map", "1:a:0", "-map", "2:s:0"];
  if (timeLimit) args.push("-t", timeLimit);
  args.push("-c:v", "copy", "-c:a", "aac", "-b:a", "192k", "-c:s", "mov_text", "-metadata:s:s:0", "language=chi", "-shortest", "-movflags", "+faststart", destination);
  run(ffmpeg, args);
}

const horizontalSilent = path.join(workDir, "episode02-horizontal.mp4");
renderStillVideo(segmentsFor(imagePlan), "3840:2160", horizontalSilent);
mux(horizontalSilent, path.join(outputDir, "OneNote-Markdown-EP02-4K.mp4"));

const verticalPlan = imagePlan.map((items, index) => {
  if (index === 0) return [[path.join(cardsDir, "vertical-title.png"), 1]];
  if (index === 11) return [[path.join(cardsDir, "vertical-compare.png"), 1]];
  if (index === 12) return [[path.join(cardsDir, "vertical-cta.png"), 1]];
  return items.map(([file, ratio]) => {
    const screenName = Object.keys(screens).find((key) => screens[key] === file && !key.endsWith("-vertical"));
    return [screenName ? screens[`${screenName}-vertical`] : file, ratio];
  });
});
const verticalSilent = path.join(workDir, "episode02-vertical.mp4");
renderStillVideo(segmentsFor(verticalPlan), "1080:1920", verticalSilent);
mux(verticalSilent, path.join(outputDir, "OneNote-Markdown-EP02-vertical-full.mp4"));

const teaserSegments = [
  [path.join(cardsDir, "vertical-title.png"), 8],
  [screens["raw-top-vertical"], 9],
  [screens["rendered-styles-lists-vertical"], 12],
  [screens["rendered-tasks-vertical"], 8],
  [screens["rendered-quote-code-latex-vertical"], 12],
  [path.join(cardsDir, "vertical-cta.png"), 6],
];
const teaserSilent = path.join(workDir, "episode02-teaser.mp4");
renderStillVideo(teaserSegments, "1080:1920", teaserSilent);
mux(teaserSilent, path.join(outputDir, "OneNote-Markdown-EP02-vertical-55s.mp4"), "55");

console.log(`Duration: ${duration(voiceMaster).toFixed(2)} seconds`);
