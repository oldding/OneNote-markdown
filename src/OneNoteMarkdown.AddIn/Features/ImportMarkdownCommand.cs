using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class ImportMarkdownCommand
    {
        public static void Execute()
        {
            try
            {
                Logger.Info("ImportMarkdownCommand started");
                OneNoteProvider provider = new OneNoteProvider();

                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Markdown 文件|*.md;*.markdown|所有文件|*.*";
                    dialog.Title = "选择要导入的 Markdown 文件";
                    DialogResult result = FileDialogHost.ShowOpen(dialog);
                    Logger.Info("ImportMarkdownCommand file dialog result=" + result);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    // Capture page ID AFTER the dialog closes so we target the page
                    // currently in focus, not the one that was active at dialog-open time.
                    string pageId = provider.GetCurrentPageId();
                    if (string.IsNullOrWhiteSpace(pageId))
                    {
                        Msg.Show("当前没有打开的页面。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    FileInfo fileInfo = new FileInfo(dialog.FileName);
                    if (fileInfo.Length > 10 * 1024 * 1024)
                    {
                        Msg.Show("文件过大（超过 10 MB），请选择较小的文件。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string markdown = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                    if (string.IsNullOrWhiteSpace(markdown))
                    {
                        Msg.Show("所选 Markdown 文件为空。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var blocks = MarkdownRenderer.RenderToBlocks(markdown);
                    if (blocks == null || blocks.Count == 0)
                    {
                        Msg.Show("Markdown 解析后为空。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    PageWriter writer = new PageWriter();
                    writer.AppendBlocks(pageId, blocks, "Markdown 导入");
                    Logger.Info("ImportMarkdownCommand completed");
                    Msg.Show("已导入并渲染 Markdown 到当前页面。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Import markdown failed", ex);
                Msg.Show("导入 Markdown 失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
