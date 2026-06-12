# OneNote Markdown

在 Microsoft OneNote 中编写和渲染 Markdown 的插件。支持实时预览、LaTeX 公式、图表渲染、代码高亮。

## 功能

- **Markdown 渲染** — 将 Markdown 文本渲染为格式化内容，支持标题、列表、代码块、表格等
- **实时预览模式** — 在页面中创建源码区和预览区，编辑后自动刷新
- **LaTeX 公式** — 支持行内和块级数学公式渲染
- **图表渲染** — 支持 Mermaid 等图表
- **代码高亮** — 多种语言语法高亮
- **导入/导出** — 导入 `.md` 文件渲染到页面，或将页面导出为 Markdown
- **剪贴板支持** — 一键复制页面为 Markdown

## 快捷键

| 快捷键 | 功能 |
|--------|------|
| `F5` | 渲染整页 |
| `F8` | 复制 Markdown 到剪贴板 |
| `Ctrl+\` | 开启/关闭实时模式 |

## 使用方法

1. 安装后，OneNote 功能区会出现 "Markdown" 选项卡
2. 在页面中编写 Markdown 文本
3. 点击"渲染整页"或按 `F5` 预览效果
4. 点击"实时模式"进入实时编辑状态

详细说明见 [HELP.md](HELP.md) 和 [PLUGIN_TEST_GUIDE.md](PLUGIN_TEST_GUIDE.md)。

## 项目结构

```
src/
├── OneNoteMarkdown.AddIn/         # 插件主体
│   ├── AddIn/                     # 插件入口与功能区处理
│   ├── Features/                  # 功能命令（渲染、导入、导出等）
│   ├── Markdown/                  # Markdown 解析与渲染引擎
│   ├── OneNote/                   # OneNote API 交互
│   ├── Rendering/                 # 公式与图表渲染
│   ├── Settings/                  # 主题与配置
│   └── UI/                        # UI 工具类
└── OneNoteMarkdown.Installer/     # 安装程序（Inno Setup）
```

## 技术栈

- C# / .NET Framework 4.8
- OneNote COM Interop API
- WPF（窗口交互）
- Inno Setup（安装程序）

## 许可证

MIT
