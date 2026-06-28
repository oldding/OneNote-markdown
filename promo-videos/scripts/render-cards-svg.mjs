import fs from "node:fs";
import path from "node:path";
import { createRequire } from "node:module";

const episodeDir = path.resolve(process.argv[2] || "promo-videos/episode-01");
const outputDir = path.join(episodeDir, "cards");
const require = createRequire(import.meta.url);
const sharp = require("sharp");
fs.mkdirSync(outputDir, { recursive: true });

const W = 3840;
const H = 2160;
const purple = "#cf8cff";
const muted = "#c8bdd3";
const panel = "#21172b";

function escapeXml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;");
}

function text(x, y, value, size, options = {}) {
  const {
    fill = "#f8f4ff",
    weight = 600,
    anchor = "start",
    family = "Microsoft YaHei UI, Segoe UI, sans-serif",
    spacing = 0,
  } = options;
  return `<text x="${x}" y="${y}" fill="${fill}" font-size="${size}" font-weight="${weight}" text-anchor="${anchor}" font-family="${family}" letter-spacing="${spacing}">${escapeXml(value)}</text>`;
}

function panelBox(x, y, width, height, radius = 58) {
  return `<rect x="${x}" y="${y}" width="${width}" height="${height}" rx="${radius}" fill="${panel}" fill-opacity=".92" stroke="#ffffff" stroke-opacity=".13" stroke-width="3"/>`;
}

function base(content) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${W}" height="${H}" viewBox="0 0 ${W} ${H}">
  <defs>
    <radialGradient id="glow1" cx="0" cy="0" r="1" gradientTransform="translate(520 360) rotate(35) scale(1500 1100)">
      <stop stop-color="#8b2bbf" stop-opacity=".48"/><stop offset="1" stop-color="#8b2bbf" stop-opacity="0"/>
    </radialGradient>
    <radialGradient id="glow2" cx="0" cy="0" r="1" gradientTransform="translate(3300 1750) rotate(180) scale(1450 900)">
      <stop stop-color="#5838c7" stop-opacity=".35"/><stop offset="1" stop-color="#5838c7" stop-opacity="0"/>
    </radialGradient>
    <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1"><stop stop-color="#100b15"/><stop offset=".56" stop-color="#191020"/><stop offset="1" stop-color="#0e0c16"/></linearGradient>
    <linearGradient id="brand" x1="0" y1="0" x2="1" y2="1"><stop stop-color="#a84ddb"/><stop offset="1" stop-color="#61209a"/></linearGradient>
    <pattern id="grid" width="144" height="144" patternUnits="userSpaceOnUse"><path d="M 144 0 L 0 0 0 144" fill="none" stroke="#fff" stroke-opacity=".035" stroke-width="2"/></pattern>
  </defs>
  <rect width="${W}" height="${H}" fill="url(#bg)"/>
  <rect width="${W}" height="${H}" fill="url(#glow1)"/>
  <rect width="${W}" height="${H}" fill="url(#glow2)"/>
  <rect width="${W}" height="${H}" fill="url(#grid)"/>
  ${content}
</svg>`;
}

function verticalBase(content) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="1080" height="1920" viewBox="0 0 1080 1920">
  <defs>
    <radialGradient id="vglow1" cx="0" cy="0" r="1" gradientTransform="translate(180 300) rotate(40) scale(900 1000)">
      <stop stop-color="#8b2bbf" stop-opacity=".55"/><stop offset="1" stop-color="#8b2bbf" stop-opacity="0"/>
    </radialGradient>
    <radialGradient id="vglow2" cx="0" cy="0" r="1" gradientTransform="translate(930 1600) rotate(180) scale(850 900)">
      <stop stop-color="#5838c7" stop-opacity=".4"/><stop offset="1" stop-color="#5838c7" stop-opacity="0"/>
    </radialGradient>
    <linearGradient id="vbg" x1="0" y1="0" x2="1" y2="1"><stop stop-color="#100b15"/><stop offset=".56" stop-color="#191020"/><stop offset="1" stop-color="#0e0c16"/></linearGradient>
    <linearGradient id="vbrand" x1="0" y1="0" x2="1" y2="1"><stop stop-color="#a84ddb"/><stop offset="1" stop-color="#61209a"/></linearGradient>
    <pattern id="vgrid" width="90" height="90" patternUnits="userSpaceOnUse"><path d="M 90 0 L 0 0 0 90" fill="none" stroke="#fff" stroke-opacity=".035" stroke-width="2"/></pattern>
  </defs>
  <rect width="1080" height="1920" fill="url(#vbg)"/>
  <rect width="1080" height="1920" fill="url(#vglow1)"/>
  <rect width="1080" height="1920" fill="url(#vglow2)"/>
  <rect width="1080" height="1920" fill="url(#vgrid)"/>
  ${content}
</svg>`;
}

function brand(x, y) {
  return `<rect x="${x}" y="${y}" width="248" height="248" rx="56" fill="url(#brand)"/>
    ${text(x + 124, y + 178, "N", 160, { anchor: "middle", weight: 800, family: "Segoe UI" })}`;
}

const cards = {
  title: base(`
    ${brand(300, 260)}
    ${text(610, 355, "ONENOTE MARKDOWN", 64, { fill: purple, weight: 800, spacing: 10 })}
    ${text(300, 880, "在 OneNote 里", 232, { weight: 800 })}
    ${text(300, 1150, "真正用上 Markdown", 232, { weight: 800 })}
    ${text(308, 1375, "从原始文本，到标题、列表、任务、代码与公式，一键进入熟悉的 OneNote 页面。", 78, { fill: muted, weight: 400 })}
    ${text(300, 1940, "大家好，我是 OneNote MVP", 60, { fill: "#e2d7ea", weight: 500 })}
  `),
  problem: base(`
    ${panelBox(300, 350, 1380, 1420)}
    ${text(420, 520, "原生 OneNote", 60, { fill: purple, weight: 800, spacing: 6 })}
    <rect x="420" y="650" width="1140" height="900" rx="42" fill="#0d0911" stroke="#cf8cff" stroke-opacity=".24" stroke-width="3"/>
    ${text(500, 800, "# 项目计划", 70, { family: "Consolas, monospace" })}
    ${text(500, 960, "- [x] 整理目标", 65, { fill: "#e9d8f3", family: "Consolas, monospace" })}
    ${text(500, 1085, "- [ ] 编写脚本", 65, { fill: "#e9d8f3", family: "Consolas, monospace" })}
    ${text(500, 1280, "**重点内容**", 65, { fill: "#e9d8f3", family: "Consolas, monospace" })}
    <circle cx="1920" cy="1060" r="138" fill="url(#brand)"/>
    ${text(1920, 1110, "→", 145, { anchor: "middle", weight: 700 })}
    ${text(2220, 530, "ONENOTE MARKDOWN", 54, { fill: purple, weight: 800, spacing: 5 })}
    ${text(2220, 760, "写完，一键渲染", 142, { weight: 800 })}
    ${text(2220, 1030, "✓  保留 Markdown 原文", 72, { fill: "#ded4e6" })}
    ${text(2220, 1190, "✓  在下方追加渲染结果", 72, { fill: "#ded4e6" })}
    ${text(2220, 1350, "✓  继续使用 OneNote 整理", 72, { fill: "#ded4e6" })}
  `),
  features: base(`
    ${text(300, 260, "MARKDOWN 功能区", 58, { fill: purple, weight: 800, spacing: 7 })}
    ${text(300, 500, "八个入口，覆盖完整工作流", 160, { weight: 800 })}
    ${[
      "导入 Markdown", "导出 Markdown", "复制 Markdown", "渲染选区",
      "渲染整页", "实时模式", "个性化设置", "内置帮助",
    ].map((label, index) => {
      const col = index % 4;
      const row = Math.floor(index / 4);
      const x = 300 + col * 855;
      const y = 720 + row * 500;
      return `${panelBox(x, y, 780, 410, 46)}
        ${text(x + 70, y + 110, String(index + 1).padStart(2, "0"), 54, { fill: purple, weight: 800 })}
        ${text(x + 70, y + 260, label, 76, { weight: 700 })}`;
    }).join("")}
  `),
  keys: base(`
    ${text(300, 340, "高频操作", 58, { fill: purple, weight: 800, spacing: 7 })}
    ${text(300, 610, "少点鼠标，多用快捷键", 170, { weight: 800 })}
    ${[
      ["F5", "渲染整页"],
      ["F8", "复制 Markdown"],
      ["Ctrl + \\", "切换实时模式"],
    ].map(([key, label], index) => {
      const x = 300 + index * 1140;
      return `${panelBox(x, 920, 1040, 690, 52)}
        <rect x="${x + 95}" y="1050" width="850" height="210" rx="38" fill="#2e2038" stroke="#fff" stroke-opacity=".3" stroke-width="4"/>
        ${text(x + 520, 1195, key, 92, { anchor: "middle", weight: 800, family: "Segoe UI" })}
        ${text(x + 95, 1465, label, 70, { fill: muted, weight: 600 })}`;
    }).join("")}
  `),
  install: base(`
    <rect x="300" y="390" width="1750" height="1380" rx="72" fill="#7021a1" fill-opacity=".25" stroke="#cf8cff" stroke-opacity=".45" stroke-width="4"/>
    ${text(440, 610, "安装前只看这一点", 58, { fill: purple, weight: 800, spacing: 5 })}
    ${text(440, 1040, "匹配 OneNote", 150, { weight: 800 })}
    ${text(440, 1260, "位数", 240, { weight: 800 })}
    ${text(440, 1515, "不是 Windows 位数", 78, { fill: muted, weight: 500 })}
    ${text(2290, 560, "确认路径", 70, { fill: purple, weight: 800 })}
    ${["文件", "帐户", "关于 OneNote"].map((label, index) => {
      const y = 760 + index * 300;
      return `<rect x="2290" y="${y - 115}" width="132" height="132" rx="34" fill="#8b2bbf"/>
        ${text(2356, y - 26, index + 1, 62, { anchor: "middle", weight: 800 })}
        ${text(2490, y - 22, label, 86, { weight: 700 })}`;
    }).join("")}
    ${text(2290, 1710, "确认 32 位或 64 位，再下载对应安装包。", 62, { fill: muted, weight: 400 })}
  `),
  cta: base(`
    ${brand(1796, 250)}
    ${text(1920, 700, "ONENOTE MARKDOWN", 62, { anchor: "middle", fill: purple, weight: 800, spacing: 9 })}
    ${text(1920, 1040, "开源、可下载、欢迎一起完善", 150, { anchor: "middle", weight: 800 })}
    ${panelBox(320, 1250, 1520, 230, 42)}
    ${text(440, 1395, "GitHub", 58, { fill: purple, weight: 800 })}
    ${text(720, 1395, "github.com/oldding/OneNote-markdown", 58, { weight: 500 })}
    ${panelBox(2000, 1250, 1520, 230, 42)}
    ${text(2120, 1395, "国内下载", 58, { fill: purple, weight: 800 })}
    ${text(2520, 1395, "nexanote.cn", 58, { weight: 500 })}
    ${text(1920, 1900, "下一集：标题、样式、列表、代码高亮与 LaTeX 公式", 58, { anchor: "middle", fill: muted, weight: 500 })}
  `),
  "vertical-title": verticalBase(`
    <rect x="80" y="110" width="150" height="150" rx="34" fill="url(#vbrand)"/>
    ${text(155, 218, "N", 100, { anchor: "middle", weight: 800, family: "Segoe UI" })}
    ${text(270, 205, "ONENOTE", 42, { fill: purple, weight: 800, spacing: 5 })}
    ${text(270, 252, "MARKDOWN", 42, { fill: purple, weight: 800, spacing: 5 })}
    ${text(80, 670, "OneNote", 160, { weight: 800 })}
    ${text(80, 855, "终于能写", 160, { weight: 800 })}
    ${text(80, 1040, "Markdown", 160, { weight: 800 })}
    ${text(85, 1240, "写完，一键渲染", 66, { fill: muted, weight: 500 })}
    ${text(85, 1338, "标题 · 列表 · 任务", 52, { fill: muted, weight: 400 })}
    ${text(85, 1418, "代码 · 公式", 52, { fill: muted, weight: 400 })}
    ${text(80, 1780, "大家好，我是 OneNote MVP", 42, { fill: "#e2d7ea", weight: 500 })}
  `),
  "vertical-problem": verticalBase(`
    ${text(70, 150, "原生不支持 Markdown？", 72, { fill: purple, weight: 800 })}
    <rect x="70" y="250" width="940" height="610" rx="46" fill="#0d0911" stroke="#cf8cff" stroke-opacity=".24" stroke-width="3"/>
    ${text(130, 390, "# 项目计划", 60, { family: "Consolas, monospace" })}
    ${text(130, 520, "- [x] 整理目标", 52, { fill: "#e9d8f3", family: "Consolas, monospace" })}
    ${text(130, 625, "- [ ] 编写脚本", 52, { fill: "#e9d8f3", family: "Consolas, monospace" })}
    ${text(130, 760, "**重点内容**", 52, { fill: "#e9d8f3", family: "Consolas, monospace" })}
    <circle cx="540" cy="1035" r="90" fill="url(#vbrand)"/>
    ${text(540, 1080, "↓", 100, { anchor: "middle", weight: 800 })}
    ${text(70, 1290, "写完，一键渲染", 105, { weight: 800 })}
    ${text(70, 1455, "✓ 保留 Markdown 原文", 50, { fill: "#ded4e6" })}
    ${text(70, 1555, "✓ 下方追加渲染结果", 50, { fill: "#ded4e6" })}
    ${text(70, 1655, "✓ 继续用 OneNote 整理", 50, { fill: "#ded4e6" })}
  `),
  "vertical-cta": verticalBase(`
    <rect x="440" y="170" width="200" height="200" rx="46" fill="url(#vbrand)"/>
    ${text(540, 315, "N", 130, { anchor: "middle", weight: 800, family: "Segoe UI" })}
    ${text(540, 540, "项目已开源", 112, { anchor: "middle", weight: 800 })}
    ${text(540, 660, "欢迎下载与共同完善", 54, { anchor: "middle", fill: muted, weight: 500 })}
    <rect x="70" y="870" width="940" height="230" rx="42" fill="${panel}" stroke="#fff" stroke-opacity=".13" stroke-width="3"/>
    ${text(130, 965, "GitHub", 46, { fill: purple, weight: 800 })}
    ${text(130, 1045, "oldding/OneNote-markdown", 44, { weight: 600 })}
    <rect x="70" y="1160" width="940" height="230" rx="42" fill="${panel}" stroke="#fff" stroke-opacity=".13" stroke-width="3"/>
    ${text(130, 1255, "国内下载", 46, { fill: purple, weight: 800 })}
    ${text(130, 1335, "nexanote.cn", 52, { weight: 700 })}
    ${text(540, 1730, "下一集：完整 Markdown 渲染测试", 42, { anchor: "middle", fill: muted, weight: 500 })}
  `),
  "vertical-features": verticalBase(`
    ${text(70, 150, "八个入口", 94, { fill: purple, weight: 800 })}
    ${text(70, 260, "覆盖完整工作流", 72, { weight: 800 })}
    ${[
      "导入 Markdown", "导出 Markdown", "复制 Markdown", "渲染选区",
      "渲染整页", "实时模式", "个性化设置", "内置帮助",
    ].map((label, index) => {
      const col = index % 2;
      const row = Math.floor(index / 2);
      const x = 70 + col * 490;
      const y = 390 + row * 340;
      return `<rect x="${x}" y="${y}" width="450" height="285" rx="34" fill="${panel}" stroke="#fff" stroke-opacity=".13" stroke-width="3"/>
        ${text(x + 36, y + 75, String(index + 1).padStart(2, "0"), 38, { fill: purple, weight: 800 })}
        ${text(x + 36, y + 175, label, 38, { weight: 700 })}`;
    }).join("")}
  `),
  "vertical-keys": verticalBase(`
    ${text(70, 160, "高频操作", 58, { fill: purple, weight: 800, spacing: 5 })}
    ${text(70, 285, "少点鼠标", 98, { weight: 800 })}
    ${text(70, 400, "多用快捷键", 98, { weight: 800 })}
    ${[
      ["F5", "渲染整页"],
      ["F8", "复制 Markdown"],
      ["Ctrl + \\", "切换实时模式"],
    ].map(([key, label], index) => {
      const y = 560 + index * 390;
      return `<rect x="70" y="${y}" width="940" height="310" rx="42" fill="${panel}" stroke="#fff" stroke-opacity=".13" stroke-width="3"/>
        <rect x="120" y="${y + 55}" width="380" height="190" rx="30" fill="#2e2038" stroke="#fff" stroke-opacity=".3" stroke-width="3"/>
        ${text(310, y + 175, key, 62, { anchor: "middle", weight: 800, family: "Segoe UI" })}
        ${text(570, y + 175, label, 48, { weight: 700 })}`;
    }).join("")}
  `),
  "vertical-install": verticalBase(`
    ${text(70, 155, "安装前只看这一点", 58, { fill: purple, weight: 800 })}
    <rect x="70" y="260" width="940" height="600" rx="54" fill="#7021a1" fill-opacity=".26" stroke="#cf8cff" stroke-opacity=".46" stroke-width="4"/>
    ${text(130, 470, "匹配 OneNote", 92, { weight: 800 })}
    ${text(130, 650, "位数", 180, { weight: 800 })}
    ${text(130, 780, "不是 Windows 位数", 48, { fill: muted, weight: 500 })}
    ${text(70, 1030, "确认路径", 54, { fill: purple, weight: 800 })}
    ${["文件", "帐户", "关于 OneNote"].map((label, index) => {
      const y = 1180 + index * 190;
      return `<rect x="70" y="${y - 78}" width="110" height="110" rx="28" fill="#8b2bbf"/>
        ${text(125, y - 2, index + 1, 50, { anchor: "middle", weight: 800 })}
        ${text(230, y, label, 62, { weight: 700 })}`;
    }).join("")}
    ${text(70, 1760, "确认 32 位或 64 位，再下载对应安装包。", 42, { fill: muted, weight: 400 })}
  `),
  "vertical-next": verticalBase(`
    <rect x="440" y="150" width="200" height="200" rx="46" fill="url(#vbrand)"/>
    ${text(540, 295, "N", 130, { anchor: "middle", weight: 800, family: "Segoe UI" })}
    ${text(540, 560, "下一集", 84, { anchor: "middle", fill: purple, weight: 800 })}
    ${text(540, 760, "完整 Markdown", 106, { anchor: "middle", weight: 800 })}
    ${text(540, 890, "渲染测试", 106, { anchor: "middle", weight: 800 })}
    <rect x="70" y="1080" width="940" height="470" rx="44" fill="${panel}" stroke="#fff" stroke-opacity=".13" stroke-width="3"/>
    ${text(130, 1190, "标题与行内样式", 48, { weight: 700 })}
    ${text(130, 1300, "任务列表与代码高亮", 48, { weight: 700 })}
    ${text(130, 1410, "LaTeX 数学公式", 48, { weight: 700 })}
    ${text(540, 1750, "github.com/oldding/OneNote-markdown", 34, { anchor: "middle", fill: muted, weight: 500 })}
  `),
};

for (const [name, svg] of Object.entries(cards)) {
  const svgPath = path.join(outputDir, `${name}.svg`);
  const pngPath = path.join(outputDir, `${name}.png`);
  fs.writeFileSync(svgPath, svg, "utf8");
  await sharp(Buffer.from(svg)).png().toFile(pngPath);
  console.log(`Rendered ${pngPath}`);
}
