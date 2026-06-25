# OneNote Markdown Help

> [中文版本见下方](#中文帮助)

## Button Reference

- **Import Markdown**: Import a `.md` file into the current page and render it
- **Export Markdown**: Export the current page to a `.md` file
- **Copy Markdown**: Export current page as Markdown and copy to clipboard
- **Render Selection**: Render selected Markdown text to the end of the page
- **Render Page**: Re-render the entire page content as Markdown
- **Live Mode**: Create "Markdown Source (Live)" and "Markdown Preview (Live)" zones on the page
- **Settings**: Open theme and rendering configuration (`theme.ini`)
- **Help**: Open this help file

## Shortcuts

| Key | Action |
|-----|--------|
| `F5` | Render page |
| `F8` | Copy Markdown to clipboard |
| `Ctrl+\` | Toggle live preview |

Notes:
- The current version no longer intercepts `Enter` key to avoid interfering with normal OneNote input
- Live mode automatically polls and refreshes preview after you submit an edit

## Live Mode Usage

1. Click "Live Mode"
2. The page will create two zones:
   - `Markdown Source (Live)`
   - `Markdown Preview (Live)`
3. Edit Markdown only within the `Markdown Source (Live)` zone
4. After pressing Enter, the add-in automatically refreshes `Markdown Preview (Live)`

## Important Notes

- Live mode is affected by OneNote's edit commit timing
- If changes don't appear immediately, wait ~0.2-0.5 seconds
- If still not updated, press `F5` to verify the source renders correctly

## Settings File

The settings button opens:

`%AppData%\OneNoteMarkdown\settings\theme.ini`

Supported settings:

- `font.family`
- `font.monospace`
- `font.math`
- `font.size.paragraph`
- `font.size.code`
- `enable.latex.image`
- `enable.code.lineNumber`

## Supported Syntax

- Headings: `#` to `######`
- Unordered lists: `-` `*` `+`
- Ordered lists: `1.` / `1)`
- Task lists: `- [ ]` / `- [x]`
- Code blocks: ``` and `~~~`
- Inline: `**bold**` `*italic*` `` `code` `` `~~strikethrough~~`
- TOC: `[TOC]` / `[[_TOC_]]` / `{:toc}`
- Diagram blocks: `mermaid` / `mindmap` / `flow` / `sequence`
- LaTeX blocks: `$$ ... $$`

## Current Limitations

- Live mode is not yet as seamless as OneMark's per-line instant preview
- `Ctrl+,` line-level source toggle not yet implemented
- Diagrams and LaTeX prioritize stability over full OneMark compatibility
- Rendering may vary with older OneNote versions

## Troubleshooting

Log file:

`%AppData%\OneNoteMarkdown\logs\onenotemarkdown.log`

---

<a name="中文帮助"></a>

# OneNote Markdown 使用帮助

## 按钮说明

- **导入 Markdown**：把 `.md` 文件导入当前页面并渲染
- **导出 Markdown**：导出当前页到 `.md` 文件
- **复制 Markdown**：把当前页导出为 Markdown 并复制到剪贴板
- **渲染选区**：把当前选中的 Markdown 文本渲染到页面末尾
- **渲染整页**：把当前页正文按 Markdown 重新渲染到页面末尾
- **实时模式**：在当前页创建"Markdown 源码（实时）"和"Markdown 预览（实时）"两块
- **设置**：打开主题与渲染配置文件 `theme.ini`
- **帮助**：打开本说明文件

## 快捷键

| 快捷键 | 功能 |
|--------|------|
| `F5` | 渲染整页 |
| `F8` | 复制 Markdown 到剪贴板 |
| `Ctrl+\` | 开启/关闭实时模式 |

说明：

- 当前版本不再拦截 `Enter` 键，避免影响 OneNote 正常换行输入
- 实时模式会在你回车提交编辑后，通过轮询自动尝试刷新预览

## 实时模式正确用法

1. 点击"实时模式"
2. 页面会出现：
   - `Markdown 源码（实时）`
   - `Markdown 预览（实时）`
3. 只在 `Markdown 源码（实时）` 这块里编辑 Markdown
4. 输入回车后，OneNote 完成提交时，插件会自动尝试刷新 `Markdown 预览（实时）`

## 重要说明

- 当前版本的实时模式仍受 OneNote 编辑提交时机影响
- 如果刚按回车没有马上变化，可再等待约 0.2~0.5 秒
- 如仍未变化，可按一次 `F5` 验证当前源码是否可正常渲染

## 设置文件

设置按钮会打开：

`%AppData%\OneNoteMarkdown\settings\theme.ini`

当前支持：

- `font.family`
- `font.monospace`
- `font.math`
- `font.size.paragraph`
- `font.size.code`
- `enable.latex.image`
- `enable.code.lineNumber`

## 已支持语法

- 标题：`#` 到 `######`
- 无序列表：`-` `*` `+`
- 有序列表：`1.` / `1)`
- 任务列表：`- [ ]` / `- [x]`
- 代码块：``` 和 `~~~`
- 行内强调：`**粗体**` `*斜体*` `` `代码` `` `~~删除~~`
- TOC：`[TOC]` / `[[_TOC_]]` / `{:toc}`
- 图表块：`mermaid` / `mindmap` / `flow` / `sequence`
- LaTeX 块：`$$ ... $$`

## 目前限制

- 实时模式还不是 OneMark 那种完全无感、逐行即时的体验
- `Ctrl+,` 行级源码切换尚未完成
- 图表与 LaTeX 目前以安全兼容优先，不保证完全对标 OneMark
- 若 OneNote 版本较旧，某些渲染表现可能不同

## 故障排查

日志文件：

`%AppData%\OneNoteMarkdown\logs\onenotemarkdown.log`
