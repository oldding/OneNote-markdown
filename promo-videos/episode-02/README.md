# EP02 - 完整 Markdown 渲染演示

本集使用真实 OneNote 页面演示 Markdown 渲染前后对比，覆盖标题、行内样式、列表、任务、引用、代码高亮和 LaTeX。

## 成片

- `output/OneNote-Markdown-EP02-4K.mp4`：3840x2160，约 3 分 14 秒
- `output/OneNote-Markdown-EP02-vertical-full.mp4`：1080x1920，完整正片
- `output/OneNote-Markdown-EP02-vertical-55s.mp4`：1080x1920，55 秒预告

`captures/`、`voice/`、`cards/`、`work/` 和 `output/` 为可再生或大体积素材，默认不提交 Git。

## 复现

```powershell
node promo-videos/scripts/voxcpm-generate.mjs `
  --input promo-videos/episode-02/voice-chunks.txt `
  --output-dir promo-videos/episode-02/voice

$base = "C:\Users\dinghuqiang\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\node_modules"
$env:NODE_PATH = "$base;$base\.pnpm\node_modules"
& "C:\Users\dinghuqiang\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" `
  promo-videos/scripts/render-cards-episode02.mjs `
  promo-videos/episode-02

node promo-videos/scripts/assemble-episode02.mjs .
```

配音使用已获授权的本人参考音色，成片在剪辑阶段应用 `1.35x` 语速和 `-16 LUFS` 响度标准化。
