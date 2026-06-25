**English** | [中文](README.zh-CN.md)

# OneNote Markdown

A Microsoft OneNote add-in for writing and rendering Markdown directly in your notes. Supports live preview, LaTeX formulas, diagram source blocks, and syntax highlighting.

![OneNote Markdown Demo](docs/demo.png)

## Download

Download from the [Releases](https://github.com/oldding/OneNote-markdown/releases) page:

- `OneNoteMarkdownSetup-x86.exe` — for 32-bit OneNote
- `OneNoteMarkdownSetup-x64.exe` — for 64-bit OneNote

> The installer must match **OneNote's bitness**, not Windows'. A 64-bit Windows may still run 32-bit OneNote.

## Features

| Feature | Description |
|---------|-------------|
| **Markdown Rendering** | Render Markdown text as formatted content: headings, lists, code blocks, tables, and more |
| **Live Preview** | Create source and preview zones on the page; auto-refresh after edits |
| **LaTeX Formulas** | Inline and block math formula rendering |
| **Diagram Source Blocks** | Detect fenced code blocks (e.g. Mermaid) and display source with preserved formatting |
| **Code Highlighting** | Syntax highlighting for multiple languages |
| **Import/Export** | Import `.md` files or export pages as Markdown |
| **Clipboard Support** | One-click copy page as Markdown |
| **Settings Dialog** | Configure fonts, sizes, LaTeX output, code line numbers, and UI language |

## Shortcuts

| Key | Action |
|-----|--------|
| `F5` | Render page |
| `F8` | Copy Markdown to clipboard |
| `Ctrl+\` | Toggle live preview |

## Getting Started

1. Check OneNote bitness: `File → Account → About OneNote`
2. Install the matching version (`x86` for 32-bit OneNote, `x64` for 64-bit)
3. After installation, a "Markdown" tab appears in the OneNote ribbon
4. Write Markdown text on a page, then click "Render Page" or press `F5`

For detailed usage, see [HELP.md](HELP.md). For plugin testing guide, see [PLUGIN_TEST_GUIDE.md](PLUGIN_TEST_GUIDE.md).

## Project Structure

```
src/
├── OneNoteMarkdown.AddIn/         # Add-in core
│   ├── AddIn/                     # Entry point & ribbon handling
│   ├── Features/                  # Commands (render, import, export, etc.)
│   ├── Markdown/                  # Markdown parser & renderer
│   ├── OneNote/                   # OneNote API interaction
│   ├── Localization/              # i18n (Chinese + English)
│   ├── Rendering/                 # Formula & diagram rendering
│   ├── Settings/                  # Theme & configuration
│   └── UI/                        # UI components (Settings, Help dialogs)
└── OneNoteMarkdown.Installer/     # Inno Setup installer
```

## Tech Stack

- C# / .NET Framework 4.8
- OneNote COM Interop API
- WPF / WinForms (UI dialogs)
- Inno Setup (installer)

Office Primary Interop Assemblies are bundled at `src/OneNoteMarkdown.AddIn/ThirdParty/Office`, so CI does not require Microsoft Office pre-installed.

## Build & Test

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

Build output:

- `src/OneNoteMarkdown.Installer/Output/OneNoteMarkdownSetup-x86.exe`
- `src/OneNoteMarkdown.Installer/Output/OneNoteMarkdownSetup-x64.exe`

GitHub Actions builds also upload both installers as artifacts.

## License

MIT
