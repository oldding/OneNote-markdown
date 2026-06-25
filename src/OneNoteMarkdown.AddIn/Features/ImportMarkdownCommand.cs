using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
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
                    dialog.Filter = "Markdown|*.md;*.markdown|*.*|*.*";
                    DialogResult result = FileDialogHost.ShowOpen(dialog);
                    Logger.Info("ImportMarkdownCommand file dialog result=" + result);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    string pageId = provider.GetCurrentPageId();
                    if (string.IsNullOrWhiteSpace(pageId))
                    {
                        Msg.Show(Loc.S("Msg.NoPage"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    FileInfo fileInfo = new FileInfo(dialog.FileName);
                    if (fileInfo.Length > 10 * 1024 * 1024)
                    {
                        Msg.Show(Loc.S("Msg.FileTooLarge"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string markdown = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                    if (string.IsNullOrWhiteSpace(markdown))
                    {
                        Msg.Show(Loc.S("Msg.FileEmpty"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var blocks = MarkdownRenderer.RenderToBlocks(markdown);
                    if (blocks == null || blocks.Count == 0)
                    {
                        Msg.Show(Loc.S("Msg.ParseEmpty"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    PageWriter writer = new PageWriter();
                    writer.AppendBlocks(pageId, blocks, "Markdown Import");
                    Logger.Info("ImportMarkdownCommand completed");
                    Msg.Show(Loc.S("Msg.ImportSuccess"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Import markdown failed", ex);
                Msg.Show(Loc.S("Msg.ImportFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
