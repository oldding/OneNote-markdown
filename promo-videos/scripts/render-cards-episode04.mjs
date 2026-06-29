import fs from "node:fs";
import path from "node:path";
import { createRequire } from "node:module";

const episodeDir = path.resolve(process.argv[2] || "promo-videos/episode-04");
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
    ${text(585, 345, "ONENOTE MARKDOWN · EP04", 58, { fill: "#d9a6ff", weight: 800 })}
    ${text(300, 760, title, 190, { weight: 800 })}
    ${text(305, 930, subtitle, 70, { fill: "#cec2d6", weight: 450 })}
    ${rows}`);
}

function vertical(titleLines, subtitle, items = []) {
  const titleSvg = titleLines.map((line, index) =>
    text(70, 560 + index * 150, line, 112, { weight: 800 })).join("");
  const rows = items.map((item, index) =>
    `<rect x="70" y="${1100 + index * 175}" width="940" height="135" rx="34" fill="#2b1c35" stroke="#fff" stroke-opacity=".14"/>
     ${text(120, 1188 + index * 175, item, 45, { weight: 650 })}`).join("");
  return background(1080, 1920, `
    <rect x="70" y="110" width="160" height="160" rx="38" fill="#8b2bbf"/>
    ${text(150, 224, "N", 108, { anchor: "middle", family: "Segoe UI", weight: 800 })}
    ${text(270, 177, "ONENOTE", 38, { fill: "#d9a6ff", weight: 800 })}
    ${text(270, 226, "MARKDOWN · EP04", 38, { fill: "#d9a6ff", weight: 800 })}
    ${titleSvg}
    ${text(70, 930, subtitle, 43, { fill: "#cec2d6", weight: 450 })}
    ${rows}`);
}

const cards = {
  title: horizontal("导入、导出与个性化设置", "README 进来，Markdown 出去", ["导入 README", "导出 .md", "复制 Markdown", "字体 · 行号 · 语言"]),
  import: horizontal("GitHub README 怎样导入？", "当前版本先下载或克隆，再选择本地 .md 文件", ["GitHub / Git 仓库", "本地 README.md", "导入 Markdown"]),
  export: horizontal("两种带走方式", "需要文件就导出，只想粘贴就复制", ["导出：UTF-8 .md", "复制：系统剪贴板", "快捷键：F8"]),
  settings: horizontal("一套设置，各取所需", "字体、字号、代码行号分别控制", ["正文 / 代码 / 数学字体", "正文 / 代码字号", "显示代码行号"]),
  language: horizontal("中文与 English", "支持自动跟随系统；完整刷新建议重启 OneNote", ["自动（跟随系统）", "中文", "English"]),
  cta: horizontal("开源 · 免费下载", "github.com/oldding/OneNote-markdown · nexanote.cn", ["导入：本地 README", "导出：标准 .md", "复制：F8"]),
  "vertical-title": vertical(["导入与导出", "个性化设置"], "README 进来，Markdown 出去", ["导入 README", "导出 / 复制", "字体 / 行号 / 语言"]),
  "vertical-import": vertical(["GitHub README", "先保存到本地"], "当前版本不直接读取 URL", ["下载或克隆仓库", "选择 README.md", "追加到当前页面"]),
  "vertical-export": vertical(["导出文件", "或者直接复制"], "同一页面，两种去向", ["UTF-8 .md", "系统剪贴板", "F8 快捷复制"]),
  "vertical-settings": vertical(["字体字号", "代码行号"], "重新渲染后应用设置", ["正文 / 代码 / 数学字体", "两类字号独立设置", "代码行号按需开关"]),
  "vertical-language": vertical(["自动 · 中文", "English"], "完整刷新建议重启 OneNote", ["只改变插件界面", "不会翻译笔记内容"]),
  "vertical-cta": vertical(["项目已开源", "欢迎下载"], "导入、导出、复制都讲清楚了", ["GitHub：oldding/OneNote-markdown", "国内下载：nexanote.cn"]),
};

for (const [name, svg] of Object.entries(cards)) {
  fs.writeFileSync(path.join(outputDir, `${name}.svg`), svg, "utf8");
  await sharp(Buffer.from(svg)).png().toFile(path.join(outputDir, `${name}.png`));
}
