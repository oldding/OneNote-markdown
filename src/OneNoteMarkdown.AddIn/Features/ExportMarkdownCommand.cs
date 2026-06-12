using System;
using System.IO;
using System.Windows.Forms;
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
                    Msg.Show("当前页面内容为空，无法导出。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Markdown 文件|*.md";
                    dialog.Title = "导出当前页为 Markdown";
                    dialog.FileName = string.IsNullOrWhiteSpace(pageTitle) ? "OneNotePage.md" : pageTitle + ".md";
                    DialogResult result = FileDialogHost.ShowSave(dialog);
                    Logger.Info("ExportMarkdownCommand file dialog result=" + result);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    File.WriteAllText(dialog.FileName, markdown);
                    Logger.Info("ExportMarkdownCommand completed");
                    Msg.Show("已导出当前页为 Markdown。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Export markdown failed", ex);
                Msg.Show("导出 Markdown 失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
