using System;
using System.Collections.Generic;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote;
using OneNoteMarkdown.OneNote.Models;

namespace OneNoteMarkdown.Features
{
    /// <summary>
    /// Ctrl+, — toggles the current line between rendered view and raw Markdown source.
    ///
    /// If the OE has a stored md-src Meta, it is in rendered state → switch to source text.
    /// If the OE has no Meta, it is in source/plain state → render it.
    ///
    /// Must be called on the UI/STA thread.
    /// </summary>
    internal static class ToggleSourceCommand
    {
        public static void Execute()
        {
            try
            {
                OneNoteProvider provider = new OneNoteProvider();
                OeInfo oe = provider.GetCurrentOeInfo();
                if (oe == null)
                {
                    Logger.Info("ToggleSource: no active OE found");
                    return;
                }

                // Determine current state: if GetCurrentOeInfo found a Meta md-src,
                // the OE is in rendered state; otherwise it's raw.
                bool isRendered = provider.CurrentOeHasMdMeta(oe.PageId, oe.ObjectId);

                if (isRendered)
                {
                    // Switch to raw source view.
                    Logger.Info("ToggleSource: rendered → source, src=" + oe.MarkdownSource);
                    PageWriter writer = new PageWriter();
                    writer.ReplaceOeWithRawSource(oe.PageId, oe.ObjectId, oe.MarkdownSource);
                }
                else
                {
                    // Switch to rendered view.
                    string src = (oe.MarkdownSource ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(src)) return;
                    Logger.Info("ToggleSource: source → rendered, src=" + src);
                    List<MarkdownBlock> blocks = MarkdownRenderer.RenderToBlocks(src);
                    if (blocks == null || blocks.Count == 0)
                        blocks = new List<MarkdownBlock> { MarkdownBlock.Blank() };
                    PageWriter writer = new PageWriter();
                    writer.ReplaceOeWithRenderedBlocks(oe.PageId, oe.ObjectId, src, blocks);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ToggleSource failed", ex);
            }
        }
    }
}
