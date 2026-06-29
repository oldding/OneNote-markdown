# EP04 - 导入、导出与个性化设置

本集用真实 OneNote 界面演示导入本地 GitHub README、导出 `.md`、复制
Markdown，以及字体、字号、代码行号和中英文界面设置。

## 事实口径

- 当前版本导入本地 `.md` / `.markdown` 文件，不直接读取 GitHub URL。
- 导入内容以 UTF-8 读取，渲染后追加到当前页面，不覆盖原内容。
- 导出文件使用 UTF-8 无 BOM，默认文件名来自当前页面标题。
- “复制 Markdown”和 `F8` 都会把当前页面转换后写入剪贴板。
- 设置修改后需要重新渲染页面；语言要完整刷新时建议重启 OneNote。

## 成片

- `output/OneNote-Markdown-EP04-4K.mp4`：3840x2160，151.05 秒
- `output/OneNote-Markdown-EP04-vertical-full.mp4`：1080x1920，完整竖版
- `output/OneNote-Markdown-EP04-vertical-55s.mp4`：1080x1920，55 秒竖版

详细媒体检查结果见 `qa-report.md`。

`captures/`、`voice/`、`cards/`、`work/` 和 `output/` 是可再生成或
大体积素材，默认不提交 Git。

## 复现

```powershell
node promo-videos/scripts/voxcpm-generate.mjs `
  --input promo-videos/episode-04/voice-chunks.txt `
  --output-dir promo-videos/episode-04/voice

$base = "C:\Users\dinghuqiang\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\node_modules"
$env:NODE_PATH = "$base;$base\.pnpm\node_modules"
& "C:\Users\dinghuqiang\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" `
  promo-videos/scripts/render-cards-episode04.mjs `
  promo-videos/episode-04

node promo-videos/scripts/assemble-episode04.mjs .
```

配音使用已获授权的本人参考音色；剪辑采用 `1.35x` 语速，并将响度标准化
到 `-16 LUFS`。
