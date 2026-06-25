using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class ExportMarkdownCommand
    {
        public static void Execute()
        {
            try
            {
                Logger.Info("ExportMarkdownCommand started");
                string pageTitle;
                string markdown = MarkdownExportService.ExportCurrentPageMarkdown(out pageTitle);
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    Msg.Show(Loc.S("Msg.PageEmpty"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Markdown|*.md";
                    dialog.FileName = string.IsNullOrWhiteSpace(pageTitle) ? "OneNotePage.md" : pageTitle + ".md";
                    DialogResult result = FileDialogHost.ShowSave(dialog);
                    Logger.Info("ExportMarkdownCommand file dialog result=" + result);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    File.WriteAllText(dialog.FileName, markdown, new UTF8Encoding(false));
                    Logger.Info("ExportMarkdownCommand completed");
                    Msg.Show(Loc.S("Msg.ExportSuccess"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Export markdown failed", ex);
                Msg.Show(Loc.S("Msg.ExportFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
