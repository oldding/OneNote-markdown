using System;
using System.Windows.Forms;
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
                    Msg.Show("当前页面内容为空，无法复制。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Clipboard.SetText(markdown, TextDataFormat.UnicodeText);
                Logger.Info("ExportMarkdownToClipboardCommand completed");
                Msg.Show("已复制当前页 Markdown 到剪贴板。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Error("Export markdown to clipboard failed", ex);
                Msg.Show("复制 Markdown 失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
