using System;
using System.Windows.Forms;
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
                    Msg.Show("当前没有打开的页面。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string selection = provider.GetCurrentSelectionText();
                if (string.IsNullOrWhiteSpace(selection))
                {
                    Msg.Show("请先在页面中选中一段 Markdown 文本。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var blocks = MarkdownRenderer.RenderToBlocks(selection);
                if (blocks == null || blocks.Count == 0)
                {
                    Msg.Show("选区内容无法渲染。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PageWriter writer = new PageWriter();
                writer.AppendBlocks(pageId, blocks, "Markdown 渲染（选区）");
            }
            catch (Exception ex)
            {
                Logger.Error("Render selection failed", ex);
                Msg.Show("渲染选区失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
