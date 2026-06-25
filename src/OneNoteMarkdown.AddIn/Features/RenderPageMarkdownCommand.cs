using System;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class RenderPageMarkdownCommand
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
                string content = provider.GetCurrentPageTextForRender();
                if (string.IsNullOrWhiteSpace(content))
                {
                    Msg.Show(Loc.S("Msg.PageContentEmpty"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var blocks = MarkdownRenderer.RenderToBlocks(content);
                if (blocks == null || blocks.Count == 0)
                {
                    Msg.Show(Loc.S("Msg.RenderPageEmpty"), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PageWriter writer = new PageWriter();
                writer.AppendBlocks(pageId, blocks, "Markdown Render");
            }
            catch (Exception ex)
            {
                Logger.Error("Render page failed", ex);
                Msg.Show(Loc.S("Msg.RenderPageFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
