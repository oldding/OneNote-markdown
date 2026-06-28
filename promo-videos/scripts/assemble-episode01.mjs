import { execFileSync } from "node:child_process";
import fs from "node:fs";
import path from "node:path";

const root = path.resolve(process.argv[2] || ".");
const episode = path.join(root, "promo-videos", "episode-01");
const voiceDir = path.join(episode, "voice");
const testDir = path.join(episode, "voice-test");
const cardsDir = path.join(episode, "cards");
const capturesDir = path.join(episode, "captures");
const outputDir = path.join(episode, "output");
const workDir = path.join(episode, "work");
const ffmpeg =
  process.env.FFMPEG ||
  String.raw`D:\OpenCode\AI Note\.tools\ffmpeg\ffmpeg-8.1.1-essentials_build\bin\ffmpeg.exe`;
const ffprobe =
  process.env.FFPROBE ||
  String.raw`D:\OpenCode\AI Note\.tools\ffmpeg\ffmpeg-8.1.1-essentials_build\bin\ffprobe.exe`;
const gap = 0.42;

fs.mkdirSync(voiceDir, { recursive: true });
fs.mkdirSync(outputDir, { recursive: true });
fs.mkdirSync(workDir, { recursive: true });

for (const extension of [".wav", ".srt"]) {
  const source = path.join(testDir, `01${extension}`);
  const destination = path.join(voiceDir, `01${extension}`);
  if (!fs.existsSync(destination) && fs.existsSync(source)) {
    fs.copyFileSync(source, destination);
  }
}

function run(executable, args) {
  console.log([path.basename(executable), ...args].join(" "));
  execFileSync(executable, args, { stdio: "inherit" });
}

function duration(file) {
  const value = execFileSync(
    ffprobe,
    [
      "-v",
      "error",
      "-show_entries",
      "format=duration",
      "-of",
      "default=noprint_wrappers=1:nokey=1",
      file,
    ],
    { encoding: "utf8" },
  ).trim();
  return Number(value);
}

function concatPath(file) {
  return file.replaceAll("\\", "/").replaceAll("'", "'\\''");
}

function parseTime(value) {
  const match = value.match(/(\d+):(\d+):(\d+),(\d+)/);
  if (!match) throw new Error(`Invalid SRT timestamp: ${value}`);
  return (
    Number(match[1]) * 3600 +
    Number(match[2]) * 60 +
    Number(match[3]) +
    Number(match[4]) / 1000
  );
}

function formatTime(value) {
  const milliseconds = Math.max(0, Math.round(value * 1000));
  const hours = Math.floor(milliseconds / 3600000);
  const minutes = Math.floor((milliseconds % 3600000) / 60000);
  const seconds = Math.floor((milliseconds % 60000) / 1000);
  const millis = milliseconds % 1000;
  return `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")},${String(millis).padStart(3, "0")}`;
}

function offsetSrt(srt, offset, startIndex) {
  let index = startIndex;
  const blocks = srt.trim().split(/\r?\n\r?\n/).filter(Boolean);
  const output = [];
  for (const block of blocks) {
    const lines = block.split(/\r?\n/);
    const timingIndex = lines.findIndex((line) => line.includes("-->"));
    if (timingIndex < 0) continue;
    const [from, to] = lines[timingIndex].split(" --> ");
    const body = lines.slice(timingIndex + 1).join("\n");
    output.push(
      `${index}\n${formatTime(parseTime(from) + offset)} --> ${formatTime(parseTime(to) + offset)}\n${body}`,
    );
    index += 1;
  }
  return { text: output.join("\n\n"), nextIndex: index };
}

const chunks = Array.from({ length: 9 }, (_, index) => {
  const id = String(index + 1).padStart(2, "0");
  const audio = path.join(voiceDir, `${id}.wav`);
  const srt = path.join(voiceDir, `${id}.srt`);
  if (!fs.existsSync(audio)) throw new Error(`Missing voice chunk: ${audio}`);
  return { id, audio, srt, duration: duration(audio) };
});

const silence = path.join(workDir, "silence.wav");
run(ffmpeg, [
  "-y",
  "-hide_banner",
  "-loglevel",
  "error",
  "-f",
  "lavfi",
  "-i",
  "anullsrc=r=48000:cl=mono",
  "-t",
  String(gap),
  "-c:a",
  "pcm_s16le",
  silence,
]);

const audioConcat = path.join(workDir, "audio-concat.txt");
const audioLines = [];
for (const [index, chunk] of chunks.entries()) {
  audioLines.push(`file '${concatPath(chunk.audio)}'`);
  if (index < chunks.length - 1) {
    audioLines.push(`file '${concatPath(silence)}'`);
  }
}
fs.writeFileSync(audioConcat, `${audioLines.join("\n")}\n`, "utf8");

const voiceMaster = path.join(outputDir, "episode01-voice.wav");
run(ffmpeg, [
  "-y",
  "-hide_banner",
  "-loglevel",
  "error",
  "-f",
  "concat",
  "-safe",
  "0",
  "-i",
  audioConcat,
  "-af",
  "loudnorm=I=-16:TP=-1.5:LRA=11",
  "-ar",
  "48000",
  "-ac",
  "1",
  voiceMaster,
]);

let subtitleOffset = 0;
let subtitleIndex = 1;
const subtitleBlocks = [];
for (const [index, chunk] of chunks.entries()) {
  if (fs.existsSync(chunk.srt)) {
    const shifted = offsetSrt(
      fs.readFileSync(chunk.srt, "utf8"),
      subtitleOffset,
      subtitleIndex,
    );
    subtitleBlocks.push(shifted.text);
    subtitleIndex = shifted.nextIndex;
  }
  subtitleOffset += chunk.duration + (index < chunks.length - 1 ? gap : 0);
}
const subtitles = path.join(outputDir, "episode01.srt");
fs.writeFileSync(subtitles, `${subtitleBlocks.join("\n\n")}\n`, "utf8");

function prepareScreen(source, destination) {
  run(ffmpeg, [
    "-y",
    "-hide_banner",
    "-loglevel",
    "error",
    "-i",
    source,
    "-filter_complex",
    "[0:v]scale=3840:2160,boxblur=28:14[bg];[0:v]scale=3640:1956:force_original_aspect_ratio=decrease[fg];[bg][fg]overlay=(W-w)/2:(H-h)/2",
    "-frames:v",
    "1",
    destination,
  ]);
}

const rawScreen = path.join(workDir, "screen-raw.png");
const renderedScreen = path.join(workDir, "screen-rendered.png");
prepareScreen(path.join(capturesDir, "demo-page-raw.png"), rawScreen);
prepareScreen(path.join(capturesDir, "demo-page-rendered.png"), renderedScreen);

const imagePlan = [
  [{ file: path.join(cardsDir, "title.png"), ratio: 1 }],
  [{ file: path.join(cardsDir, "problem.png"), ratio: 1 }],
  [
    { file: rawScreen, ratio: 0.42 },
    { file: renderedScreen, ratio: 0.58 },
  ],
  [{ file: renderedScreen, ratio: 1 }],
  [{ file: path.join(cardsDir, "features.png"), ratio: 1 }],
  [{ file: path.join(cardsDir, "keys.png"), ratio: 1 }],
  [{ file: path.join(cardsDir, "install.png"), ratio: 1 }],
  [{ file: path.join(cardsDir, "cta.png"), ratio: 1 }],
  [{ file: path.join(cardsDir, "cta.png"), ratio: 1 }],
];

const videoConcat = path.join(workDir, "video-concat.txt");
const videoLines = [];
for (const [index, chunk] of chunks.entries()) {
  const segmentDuration =
    chunk.duration + (index < chunks.length - 1 ? gap : 0);
  for (const item of imagePlan[index]) {
    videoLines.push(`file '${concatPath(item.file)}'`);
    videoLines.push(`duration ${(segmentDuration * item.ratio).toFixed(3)}`);
  }
}
videoLines.push(`file '${concatPath(imagePlan.at(-1)[0].file)}'`);
fs.writeFileSync(videoConcat, `${videoLines.join("\n")}\n`, "utf8");

const silentVideo = path.join(workDir, "episode01-video.mp4");
run(ffmpeg, [
  "-y",
  "-hide_banner",
  "-loglevel",
  "error",
  "-f",
  "concat",
  "-safe",
  "0",
  "-i",
  videoConcat,
  "-vf",
  "fps=30,scale=3840:2160:force_original_aspect_ratio=decrease,pad=3840:2160:(ow-iw)/2:(oh-ih)/2,format=yuv420p",
  "-c:v",
  "libx264",
  "-preset",
  "medium",
  "-crf",
  "19",
  "-pix_fmt",
  "yuv420p",
  silentVideo,
]);

const master = path.join(outputDir, "OneNote-Markdown-EP01-4K.mp4");
run(ffmpeg, [
  "-y",
  "-hide_banner",
  "-loglevel",
  "error",
  "-i",
  silentVideo,
  "-i",
  voiceMaster,
  "-i",
  subtitles,
  "-map",
  "0:v:0",
  "-map",
  "1:a:0",
  "-map",
  "2:s:0",
  "-c:v",
  "copy",
  "-c:a",
  "aac",
  "-b:a",
  "192k",
  "-c:s",
  "mov_text",
  "-metadata:s:s:0",
  "language=chi",
  "-shortest",
  "-movflags",
  "+faststart",
  master,
]);

const teaser = path.join(outputDir, "OneNote-Markdown-EP01-vertical-45s.mp4");
const verticalScreen = path.join(workDir, "screen-rendered-vertical.png");
run(ffmpeg, [
  "-y",
  "-hide_banner",
  "-loglevel",
  "error",
  "-i",
  path.join(capturesDir, "demo-page-rendered.png"),
  "-vf",
  "scale=-2:1920,crop=1080:1920:0:0",
  "-frames:v",
  "1",
  verticalScreen,
]);

const verticalPlan = [
  [path.join(cardsDir, "vertical-title.png"), 8],
  [path.join(cardsDir, "vertical-problem.png"), 17],
  [verticalScreen, 14],
  [path.join(cardsDir, "vertical-cta.png"), 6],
];

run(ffmpeg, [
  "-y",
  "-hide_banner",
  "-loglevel",
  "error",
  ...verticalPlan.flatMap(([file, itemDuration]) => [
    "-loop",
    "1",
    "-t",
    String(itemDuration),
    "-i",
    file,
  ]),
  "-i",
  voiceMaster,
  "-filter_complex",
  "[0:v]fps=30,scale=1080:1920,setsar=1,format=yuv420p[v0];[1:v]fps=30,scale=1080:1920,setsar=1,format=yuv420p[v1];[2:v]fps=30,scale=1080:1920,setsar=1,format=yuv420p[v2];[3:v]fps=30,scale=1080:1920,setsar=1,format=yuv420p[v3];[v0][v1][v2][v3]concat=n=4:v=1:a=0[v]",
  "-map",
  "[v]",
  "-map",
  "4:a:0",
  "-t",
  "45",
  "-c:v",
  "libx264",
  "-preset",
  "medium",
  "-crf",
  "20",
  "-c:a",
  "aac",
  "-b:a",
  "160k",
  "-pix_fmt",
  "yuv420p",
  "-movflags",
  "+faststart",
  teaser,
]);

console.log(`Created ${master}`);
console.log(`Created ${teaser}`);
