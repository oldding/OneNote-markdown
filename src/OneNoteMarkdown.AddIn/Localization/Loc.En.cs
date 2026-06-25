using System.Collections.Generic;

namespace OneNoteMarkdown.Localization
{
    internal static partial class Loc
    {
        internal static readonly Dictionary<string, string> EnStrings = new Dictionary<string, string>
        {
            // ─── Ribbon labels ─────────────────────────────────────
            {"Ribbon.ImportMarkdown", "Import Markdown"},
            {"Ribbon.ExportMarkdown", "Export Markdown"},
            {"Ribbon.CopyMarkdown", "Copy Markdown"},
            {"Ribbon.RenderSelection", "Render Selection"},
            {"Ribbon.RenderPage", "Render Page"},
            {"Ribbon.LiveMode", "Live Mode"},
            {"Ribbon.Settings", "Settings"},
            {"Ribbon.Help", "Help"},

            // ─── Ribbon screentips ─────────────────────────────────
            {"Ribbon.ImportMarkdown.Tip", "Import Markdown"},
            {"Ribbon.ExportMarkdown.Tip", "Export Markdown"},
            {"Ribbon.CopyMarkdown.Tip", "Copy Markdown"},
            {"Ribbon.RenderSelection.Tip", "Render Selection"},
            {"Ribbon.RenderPage.Tip", "Render Page"},
            {"Ribbon.LiveMode.Tip", "Live Mode"},
            {"Ribbon.Settings.Tip", "Markdown Settings"},
            {"Ribbon.Help.Tip", "Help"},

            // ─── Ribbon supertips ──────────────────────────────────
            {"Ribbon.ImportMarkdown.SuperTip", "Import Markdown source from a .md file into the current page"},
            {"Ribbon.ExportMarkdown.SuperTip", "Export the current page as a Markdown file"},
            {"Ribbon.CopyMarkdown.SuperTip", "Export the current page as Markdown and copy to clipboard"},
            {"Ribbon.RenderSelection.SuperTip", "Render the selected Markdown text into OneNote formatted content"},
            {"Ribbon.RenderPage.SuperTip", "Re-render the entire page content as Markdown"},
            {"Ribbon.LiveMode.SuperTip", "Toggle Markdown live preview (Ctrl+\\)"},
            {"Ribbon.Settings.SuperTip", "Open theme settings to configure fonts, sizes, code line numbers, LaTeX output, etc."},
            {"Ribbon.Help.SuperTip", "View plugin features, keyboard shortcuts, and live mode instructions"},

            // ─── Dialog titles ─────────────────────────────────────
            {"Dialog.Help.Title", "OneNote Markdown - Help"},
            {"Dialog.Settings.Title", "OneNote Markdown - Settings"},
            {"Dialog.Help.Header", "✦  Help"},
            {"Dialog.Settings.Header", "✦  Theme Settings"},

            // ─── Settings dialog labels ────────────────────────────
            {"Settings.Section.Font", "Font Settings"},
            {"Settings.Section.Render", "Rendering"},
            {"Settings.Section.Language", "Language"},
            {"Settings.FontFamily", "Body Font"},
            {"Settings.MonoFont", "Code Font"},
            {"Settings.MathFont", "Math Font"},
            {"Settings.ParagraphSize", "Body Font Size"},
            {"Settings.CodeSize", "Code Font Size"},
            {"Settings.EnableLatex", "Render LaTeX formulas as images"},
            {"Settings.EnableLineNumber", "Show code line numbers"},
            {"Settings.Language", "UI Language"},
            {"Settings.Language.Auto", "Auto (follow system)"},
            {"Settings.Language.Zh", "中文"},
            {"Settings.Language.En", "English"},
            {"Settings.Save", "Save"},
            {"Settings.Cancel", "Cancel"},
            {"Settings.Hint", "Changes take effect after re-rendering the page (F5).\nSettings file: %AppData%\\OneNoteMarkdown\\settings\\theme.ini"},
            {"Settings.SaveFailed", "Failed to save settings: {0}"},
            {"Settings.RestartHint", "Language changes will take full effect after restarting OneNote."},

            // ─── Help tree nodes ───────────────────────────────────
            {"Help.Tree.Root", "Contents"},
            {"Help.Tree.QuickStart", "Quick Start"},
            {"Help.Tree.Features", "Features"},
            {"Help.Tree.RenderPage", "Render Page"},
            {"Help.Tree.RenderSelection", "Render Selection"},
            {"Help.Tree.LiveMode", "Live Mode"},
            {"Help.Tree.ImportExport", "Import/Export"},
            {"Help.Tree.LaTeX", "LaTeX Formulas"},
            {"Help.Tree.CodeHighlight", "Code Highlighting"},
            {"Help.Tree.Settings", "Settings"},
            {"Help.Tree.Shortcuts", "Shortcuts"},
            {"Help.Tree.FAQ", "FAQ"},
            {"Help.Tree.About", "About"},

            // ─── Messages (Features) ──────────────────────────────
            {"Msg.PageEmpty", "The current page is empty and cannot be exported."},
            {"Msg.ExportSuccess", "Page exported as Markdown successfully."},
            {"Msg.ExportFailed", "Export Markdown failed: {0}"},
            {"Msg.PageEmptyCopy", "The current page is empty and cannot be copied."},
            {"Msg.ClipboardBusy", "Clipboard is busy, please try again later."},
            {"Msg.CopySuccess", "Page Markdown copied to clipboard."},
            {"Msg.CopyFailed", "Copy Markdown failed: {0}"},
            {"Msg.NoPage", "No page is currently open."},
            {"Msg.FileTooLarge", "File is too large (exceeds 10 MB), please select a smaller file."},
            {"Msg.FileEmpty", "The selected Markdown file is empty."},
            {"Msg.ParseEmpty", "Markdown parsing produced empty output."},
            {"Msg.ImportSuccess", "Markdown imported and rendered to the current page."},
            {"Msg.ImportFailed", "Import Markdown failed: {0}"},
            {"Msg.PageContentEmpty", "The current page is empty."},
            {"Msg.RenderPageEmpty", "Full page content cannot be rendered."},
            {"Msg.RenderPageFailed", "Render page failed: {0}"},
            {"Msg.NoSelection", "Please select some Markdown text first."},
            {"Msg.RenderSelEmpty", "Selected content cannot be rendered."},
            {"Msg.RenderSelFailed", "Render selection failed: {0}"},
            {"Msg.LiveOn", "Live mode enabled."},
            {"Msg.LiveOff", "Live mode disabled."},
            {"Msg.LiveToggleFailed", "Toggle live mode failed: {0}"},
            {"Msg.HelpFailed", "Failed to open help: {0}"},
            {"Msg.SettingsFailed", "Failed to open theme settings: {0}"},

            // ─── Common ────────────────────────────────────────────
            {"Common.AppTitle", "OneNote Markdown"},
            {"Common.Close", "Close"},
        };
    }
}
