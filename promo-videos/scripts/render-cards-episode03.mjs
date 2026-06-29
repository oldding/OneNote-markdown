import fs from "node:fs";
import path from "node:path";
import { createRequire } from "node:module";

const episodeDir = path.resolve(process.argv[2] || "promo-videos/episode-03");
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
    ${text(585, 345, "ONENOTE MARKDOWN · EP03", 58, { fill: "#d9a6ff", weight: 800 })}
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
    ${text(270, 226, "MARKDOWN · EP03", 38, { fill: "#d9a6ff", weight: 800 })}
    ${titleSvg}
    ${text(70, 930, subtitle, 43, { fill: "#cec2d6", weight: 450 })}
    ${rows}`);
}

const cards = {
  title: horizontal("实时模式与快捷键", "真实演示 F5、F8、Ctrl + \\", ["回车触发", "F5 整页渲染", "F8 复制", "Ctrl + \\ 切换"]),
  truth: horizontal("当前版本：原位预览", "不是左右双栏，源码提交后在当前内容块内格式化", ["源码状态", "按下回车", "预览结果"]),
  delay: horizontal("不是零延迟", "等待 OneNote 提交编辑，再读取并渲染", ["通常 0.2–0.5 秒", "复杂页面可能更慢", "必要时使用 F5"]),
  shortcuts: horizontal("三个高频快捷键", "各自职责不同，别混用", ["F5：整页追加", "F8：复制 Markdown", "Ctrl + \\：切换实时"]),
  cta: horizontal("开源 · 免费下载", "github.com/oldding/OneNote-markdown · nexanote.cn", ["短内容：实时模式", "长文：F5 整页", "带走内容：F8"]),
  "vertical-title": vertical(["实时模式", "与快捷键"], "真实行为与限制一起讲清楚", ["回车触发", "F5 · F8", "Ctrl + \\"]),
  "vertical-truth": vertical(["不是双栏", "而是原位预览"], "当前发布版真实运行方式", ["输入 Markdown 源码", "回车提交编辑", "当前内容块格式化"]),
  "vertical-delay": vertical(["等待提交", "再刷新"], "OneNote 提交时机决定速度", ["通常 0.2–0.5 秒", "同步时可能更明显", "它不是零延迟"]),
  "vertical-shortcuts": vertical(["三个快捷键", "三个职责"], "记住这张表就够了", ["F5：整页追加", "F8：复制 Markdown", "Ctrl + \\：切换实时"]),
  "vertical-cta": vertical(["项目已开源", "欢迎下载"], "短内容实时，长文整页", ["GitHub：oldding/OneNote-markdown", "国内下载：nexanote.cn"]),
};

for (const [name, svg] of Object.entries(cards)) {
  fs.writeFileSync(path.join(outputDir, `${name}.svg`), svg, "utf8");
  await sharp(Buffer.from(svg)).png().toFile(path.join(outputDir, `${name}.png`));
}
