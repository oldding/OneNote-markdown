import fs from "node:fs";
import path from "node:path";
import { createRequire } from "node:module";

const episodeDir = path.resolve(process.argv[2] || "promo-videos/episode-02");
const outputDir = path.join(episodeDir, "cards");
const require = createRequire(import.meta.url);
const sharp = require("sharp");
fs.mkdirSync(outputDir, { recursive: true });

const escapeXml = (value) =>
  String(value).replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;");

function text(x, y, value, size, options = {}) {
  const { fill = "#fff", weight = 700, anchor = "start", family = "Microsoft YaHei UI, Segoe UI, sans-serif" } = options;
  return `<text x="${x}" y="${y}" fill="${fill}" font-size="${size}" font-weight="${weight}" text-anchor="${anchor}" font-family="${family}">${escapeXml(value)}</text>`;
}

function background(width, height, content) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1"><stop stop-color="#100b15"/><stop offset=".55" stop-color="#24142d"/><stop offset="1" stop-color="#101323"/></linearGradient>
    <radialGradient id="glow"><stop stop-color="#a84ddb" stop-opacity=".55"/><stop offset="1" stop-color="#a84ddb" stop-opacity="0"/></radialGradient>
  </defs>
  <rect width="100%" height="100%" fill="url(#bg)"/>
  <circle cx="${width * 0.12}" cy="${height * 0.16}" r="${height * 0.68}" fill="url(#glow)"/>
  ${content}</svg>`;
}

function horizontal(title, subtitle, items = []) {
  const rows = items.map((item, index) => {
    const x = 300 + (index % 3) * 1120;
    const y = 1120 + Math.floor(index / 3) * 330;
    return `<rect x="${x}" y="${y - 120}" width="1010" height="250" rx="44" fill="#2b1c35" stroke="#fff" stroke-opacity=".14"/>
      ${text(x + 65, y + 22, item, 66, { weight: 650 })}`;
  }).join("");
  return background(3840, 2160, `
    <rect x="300" y="240" width="220" height="220" rx="52" fill="#8b2bbf"/>
    ${text(410, 395, "N", 145, { anchor: "middle", family: "Segoe UI", weight: 800 })}
    ${text(585, 345, "ONENOTE MARKDOWN · EP02", 58, { fill: "#d9a6ff", weight: 800 })}
    ${text(300, 760, title, 190, { weight: 800 })}
    ${text(305, 930, subtitle, 70, { fill: "#cec2d6", weight: 450 })}
    ${rows}`);
}

function vertical(titleLines, subtitle, items = []) {
  const titleSvg = titleLines.map((line, index) =>
    text(70, 570 + index * 150, line, 116, { weight: 800 })).join("");
  const rows = items.map((item, index) =>
    `<rect x="70" y="${1110 + index * 175}" width="940" height="135" rx="34" fill="#2b1c35" stroke="#fff" stroke-opacity=".14"/>
     ${text(120, 1198 + index * 175, item, 46, { weight: 650 })}`).join("");
  return background(1080, 1920, `
    <rect x="70" y="110" width="160" height="160" rx="38" fill="#8b2bbf"/>
    ${text(150, 224, "N", 108, { anchor: "middle", family: "Segoe UI", weight: 800 })}
    ${text(270, 177, "ONENOTE", 38, { fill: "#d9a6ff", weight: 800 })}
    ${text(270, 226, "MARKDOWN · EP02", 38, { fill: "#d9a6ff", weight: 800 })}
    ${titleSvg}
    ${text(70, 940, subtitle, 45, { fill: "#cec2d6", weight: 450 })}
    ${rows}`);
}

const cards = {
  title: horizontal("完整 Markdown 渲染演示", "同一份原文，逐项对比渲染前后", ["标题 H1-H6", "行内样式", "列表与任务", "引用", "代码高亮", "LaTeX"]),
  compare: horizontal("原文保留 · 结果追加", "写作保持纯文本效率，阅读获得 OneNote 原生体验", ["渲染前：Markdown 语法", "渲染后：清晰格式", "同页随时回看"]),
  recap: horizontal("八类语法，一次验证", "标题、样式、列表、任务、引用、代码与数学公式", ["层级清楚", "状态醒目", "代码可读", "公式稳定"]),
  cta: horizontal("开源 · 免费下载", "下一集：导入、导出与复制 Markdown", ["GitHub：oldding/OneNote-markdown", "国内下载：nexanote.cn"]),
  "vertical-title": vertical(["完整 Markdown", "渲染演示"], "渲染前后，逐项实测", ["标题与行内样式", "列表、任务、引用", "代码高亮与 LaTeX"]),
  "vertical-compare": vertical(["原文保留", "结果追加"], "同一页完成前后对照", ["上方：Markdown 原文", "下方：格式化结果", "原文随时继续修改"]),
  "vertical-recap": vertical(["八类语法", "一次验证"], "不是截图拼功能，是实际运行结果", ["标题 H1-H6", "列表与任务", "代码语法着色", "高清数学公式"]),
  "vertical-cta": vertical(["项目已开源", "欢迎下载"], "OneNote Markdown", ["GitHub：oldding/OneNote-markdown", "国内下载：nexanote.cn"]),
};

for (const [name, svg] of Object.entries(cards)) {
  fs.writeFileSync(path.join(outputDir, `${name}.svg`), svg, "utf8");
  await sharp(Buffer.from(svg)).png().toFile(path.join(outputDir, `${name}.png`));
}
