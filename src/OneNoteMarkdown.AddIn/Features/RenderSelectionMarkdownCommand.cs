using System;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class RenderSelectionMarkdownCommand
    {
        public static void Execute()
        {
            try
            {
                OneNoteProvider provider = new OneNoteProvider();
                string pageId = provider.GetCurrentPageId();
                if (string.IsNullOrWhiteSpace(pageId))
                {
                    Msg.Show(Loc.S("Msg.NoPage"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string selection = provider.GetCurrentSelectionText();
                if (string.IsNullOrWhiteSpace(selection))
                {
                    Msg.Show(Loc.S("Msg.NoSelection"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var blocks = MarkdownRenderer.RenderToBlocks(selection);
                if (blocks == null || blocks.Count == 0)
                {
                    Msg.Show(Loc.S("Msg.RenderSelEmpty"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PageWriter writer = new PageWriter();
                writer.AppendBlocks(pageId, blocks, "Markdown Render");
            }
            catch (Exception ex)
            {
                Logger.Error("Render selection failed", ex);
                Msg.Show(Loc.S("Msg.RenderSelFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
