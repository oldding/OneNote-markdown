using System.Collections.Generic;

namespace OneNoteMarkdown.Localization
{
    internal static partial class Loc
    {
        internal static readonly Dictionary<string, string> ZhStrings = new Dictionary<string, string>
        {
            // ─── Ribbon labels ─────────────────────────────────────
            {"Ribbon.ImportMarkdown", "导入 Markdown"},
            {"Ribbon.ExportMarkdown", "导出 Markdown"},
            {"Ribbon.CopyMarkdown", "复制 Markdown"},
            {"Ribbon.RenderSelection", "渲染选区"},
            {"Ribbon.RenderPage", "渲染整页"},
            {"Ribbon.LiveMode", "实时模式"},
            {"Ribbon.Settings", "设置"},
            {"Ribbon.Help", "帮助"},

            // ─── Ribbon screentips ─────────────────────────────────
            {"Ribbon.ImportMarkdown.Tip", "导入 Markdown"},
            {"Ribbon.ExportMarkdown.Tip", "导出 Markdown"},
            {"Ribbon.CopyMarkdown.Tip", "复制 Markdown"},
            {"Ribbon.RenderSelection.Tip", "渲染选区"},
            {"Ribbon.RenderPage.Tip", "渲染整页"},
            {"Ribbon.LiveMode.Tip", "实时模式"},
            {"Ribbon.Settings.Tip", "Markdown 设置"},
            {"Ribbon.Help.Tip", "帮助"},

            // ─── Ribbon supertips ──────────────────────────────────
            {"Ribbon.ImportMarkdown.SuperTip", "从 .md 文件导入 Markdown 原文到当前页面"},
            {"Ribbon.ExportMarkdown.SuperTip", "将当前页面导出为 Markdown 文件"},
            {"Ribbon.CopyMarkdown.SuperTip", "将当前页面导出为 Markdown 并复制到剪贴板"},
            {"Ribbon.RenderSelection.SuperTip", "把当前选中的 Markdown 文本渲染成 OneNote 内容"},
            {"Ribbon.RenderPage.SuperTip", "把当前页内容按 Markdown 重新渲染"},
            {"Ribbon.LiveMode.SuperTip", "开启或关闭 Markdown 实时预览（Ctrl+\\）"},
            {"Ribbon.Settings.SuperTip", "打开主题设置，修改字体、字号、代码行号、LaTeX 输出等配置"},
            {"Ribbon.Help.SuperTip", "查看插件功能说明、快捷键和实时模式使用方法"},

            // ─── Dialog titles ─────────────────────────────────────
            {"Dialog.Help.Title", "OneNote Markdown - 帮助"},
            {"Dialog.Settings.Title", "OneNote Markdown - 设置"},
            {"Dialog.Help.Header", "✦  使用帮助"},
            {"Dialog.Settings.Header", "✦  主题设置"},

            // ─── Settings dialog labels ────────────────────────────
            {"Settings.Section.Font", "字体设置"},
            {"Settings.Section.Render", "渲染设置"},
            {"Settings.Section.Language", "语言设置"},
            {"Settings.FontFamily", "正文字体"},
            {"Settings.MonoFont", "代码字体"},
            {"Settings.MathFont", "数学字体"},
            {"Settings.ParagraphSize", "正文字号"},
            {"Settings.CodeSize", "代码字号"},
            {"Settings.EnableLatex", "启用 LaTeX 公式渲染为图片"},
            {"Settings.EnableLineNumber", "显示代码行号"},
            {"Settings.Language", "界面语言"},
            {"Settings.Language.Auto", "自动（跟随系统）"},
            {"Settings.Language.Zh", "中文"},
            {"Settings.Language.En", "English"},
            {"Settings.Save", "保存"},
            {"Settings.Cancel", "取消"},
            {"Settings.Hint", "修改后重新渲染页面（F5）即可生效。\n设置文件：%AppData%\\OneNoteMarkdown\\settings\\theme.ini"},
            {"Settings.SaveFailed", "保存设置失败：{0}"},
            {"Settings.RestartHint", "语言变更将在下次启动 OneNote 后完全生效。"},

            // ─── Help tree nodes ───────────────────────────────────
            {"Help.Tree.Root", "目录"},
            {"Help.Tree.QuickStart", "快速入门"},
            {"Help.Tree.Features", "功能说明"},
            {"Help.Tree.RenderPage", "渲染整页"},
            {"Help.Tree.RenderSelection", "渲染选区"},
            {"Help.Tree.LiveMode", "实时模式"},
            {"Help.Tree.ImportExport", "导入/导出"},
            {"Help.Tree.LaTeX", "LaTeX 公式"},
            {"Help.Tree.CodeHighlight", "代码高亮"},
            {"Help.Tree.Settings", "设置说明"},
            {"Help.Tree.Shortcuts", "快捷键"},
            {"Help.Tree.FAQ", "常见问题"},
            {"Help.Tree.About", "关于"},

            // ─── Messages (Features) ──────────────────────────────
            {"Msg.PageEmpty", "当前页面内容为空，无法导出。"},
            {"Msg.ExportSuccess", "已导出当前页为 Markdown。"},
            {"Msg.ExportFailed", "导出 Markdown 失败：{0}"},
            {"Msg.PageEmptyCopy", "当前页面内容为空，无法复制。"},
            {"Msg.ClipboardBusy", "剪贴板被占用，请稍后重试。"},
            {"Msg.CopySuccess", "已复制当前页 Markdown 到剪贴板。"},
            {"Msg.CopyFailed", "复制 Markdown 失败：{0}"},
            {"Msg.NoPage", "当前没有打开的页面。"},
            {"Msg.FileTooLarge", "文件过大（超过 10 MB），请选择较小的文件。"},
            {"Msg.FileEmpty", "所选 Markdown 文件为空。"},
            {"Msg.ParseEmpty", "Markdown 解析后为空。"},
            {"Msg.ImportSuccess", "已导入并渲染 Markdown 到当前页面。"},
            {"Msg.ImportFailed", "导入 Markdown 失败：{0}"},
            {"Msg.PageContentEmpty", "当前页面为空。"},
            {"Msg.RenderPageEmpty", "整页内容无法渲染。"},
            {"Msg.RenderPageFailed", "渲染整页失败：{0}"},
            {"Msg.NoSelection", "请先在页面中选中一段 Markdown 文本。"},
            {"Msg.RenderSelEmpty", "选区内容无法渲染。"},
            {"Msg.RenderSelFailed", "渲染选区失败：{0}"},
            {"Msg.LiveOn", "实时模式已开启。"},
            {"Msg.LiveOff", "实时模式已关闭。"},
            {"Msg.LiveToggleFailed", "切换实时模式失败：{0}"},
            {"Msg.HelpFailed", "打开帮助失败：{0}"},
            {"Msg.SettingsFailed", "打开主题配置失败：{0}"},

            // ─── Common ────────────────────────────────────────────
            {"Common.AppTitle", "OneNote Markdown"},
            {"Common.Close", "关闭"},
        };
    }
}
