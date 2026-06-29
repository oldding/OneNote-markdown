# EP03 - 实时模式与快捷键

本集使用真实 OneNote 页面演示实时模式、源码状态与预览结果，以及
`F5`、`F8`、`Ctrl+\` 三个快捷键。横版约 2 分钟，并提供完整版和
55 秒竖版。

## 事实口径

- 当前实时模式不是固定双栏编辑器。视频中的“源码区”和“预览区”指同一
  内容块提交前的 Markdown 源码状态与提交后的渲染结果。
- 按下回车后，插件会将当前内容提交给 OneNote，再用渲染结果替换原内容。
- 提交通常会有短暂延迟；复杂页面、同步或设备负载较高时可能更久。
- `Ctrl+\`：开启或关闭实时模式。
- `F5`：渲染当前整页 Markdown。
- `F8`：将当前页面复制为 Markdown。

## 成片

- `output/OneNote-Markdown-EP03-4K.mp4`：3840x2160，约 2 分 02 秒
- `output/OneNote-Markdown-EP03-vertical-full.mp4`：1080x1920，完整竖版
- `output/OneNote-Markdown-EP03-vertical-55s.mp4`：1080x1920，55 秒竖版

`captures/`、`voice/`、`cards/`、`work/` 和 `output/` 是可再生成或
大体积素材，默认不提交 Git。

## 复现

```powershell
node promo-videos/scripts/voxcpm-generate.mjs `
  --input promo-videos/episode-03/voice-chunks.txt `
  --output-dir promo-videos/episode-03/voice

$base = "C:\Users\dinghuqiang\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\node_modules"
$env:NODE_PATH = "$base;$base\.pnpm\node_modules"
& "C:\Users\dinghuqiang\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" `
  promo-videos/scripts/render-cards-episode03.mjs `
  promo-videos/episode-03

node promo-videos/scripts/assemble-episode03.mjs .
```

配音使用已获授权的本人参考音色；剪辑采用 `1.35x` 语速，并将响度标准化
到 `-16 LUFS`。
