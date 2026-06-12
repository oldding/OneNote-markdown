using System;
using System.Windows.Forms;
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
                    Msg.Show("当前没有打开的页面。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string content = provider.GetCurrentPageTextForRender();
                if (string.IsNullOrWhiteSpace(content))
                {
                    Msg.Show("当前页面为空。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var blocks = MarkdownRenderer.RenderToBlocks(content);
                if (blocks == null || blocks.Count == 0)
                {
                    Msg.Show("整页内容无法渲染。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PageWriter writer = new PageWriter();
                writer.AppendBlocks(pageId, blocks, "Markdown 渲染（整页）");
            }
            catch (Exception ex)
            {
                Logger.Error("Render page failed", ex);
                Msg.Show("渲染整页失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
