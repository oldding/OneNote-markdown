using System;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class ExportMarkdownToClipboardCommand
    {
        public static void Execute()
        {
            try
            {
                Logger.Info("ExportMarkdownToClipboardCommand started");
                string pageTitle;
                string markdown = MarkdownExportService.ExportCurrentPageMarkdown(out pageTitle);
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    Msg.Show(Loc.S("Msg.PageEmptyCopy"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    Clipboard.SetText(markdown, TextDataFormat.UnicodeText);
                }
                catch (System.Runtime.InteropServices.ExternalException exClip)
                {
                    Logger.Error("Clipboard access failed", exClip);
                    Msg.Show(Loc.S("Msg.ClipboardBusy"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Logger.Info("ExportMarkdownToClipboardCommand completed");
                Msg.Show(Loc.S("Msg.CopySuccess"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Error("Export markdown to clipboard failed", ex);
                Msg.Show(Loc.S("Msg.CopyFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
