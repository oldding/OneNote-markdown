using System;
using System.Text;
using OneNoteMarkdown.OneNote.Models;

namespace OneNoteMarkdown.Markdown
{
    internal static class MarkdownExporter
    {
        public static string Export(PageContent page)
        {
            if (page == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(page.Title))
            {
                builder.Append("# ");
                builder.Append(page.Title.Trim());
                builder.AppendLine();
                builder.AppendLine();
            }

            bool firstLine = true;
            foreach (OutlineContent outline in page.Outlines)
            {
                if (outline == null || outline.TextBlocks == null)
                {
                    continue;
                }

                foreach (TextBlock block in outline.TextBlocks)
                {
                    if (block == null || block.IsMarkdownContinuation)
                    {
                        continue;
                    }

                    string text = !string.IsNullOrEmpty(block.MarkdownSource)
                        ? block.MarkdownSource.Trim()
                        : (block.Text ?? string.Empty).Trim();
                    if (text.Length == 0)
                    {
                        continue;
                    }

                    if (!firstLine)
                    {
                        builder.AppendLine();
                    }

                    firstLine = false;

                    if (!string.IsNullOrEmpty(block.MarkdownSource))
                    {
                        builder.Append(text);
                        continue;
                    }

                    int indent = block.IndentLevel < 0 ? 0 : block.IndentLevel;
                    for (int i = 0; i < indent; i++)
                    {
                        builder.Append("  ");
                    }

                    if (LooksLikeTask(text))
                    {
                        builder.Append(text.StartsWith("☑") ? "- [x] " : "- [ ] ");
                        builder.Append(text.Substring(1).Trim());
                    }
                    else if (LooksLikeBullet(text))
                    {
                        builder.Append("- ");
                        builder.Append(text.Substring(1).Trim());
                    }
                    else
                    {
                        builder.Append(text);
                    }
                }
            }

            return builder.ToString().Trim();
        }

        private static bool LooksLikeTask(string text)
        {
            return text.StartsWith("☐") || text.StartsWith("☑");
        }

        private static bool LooksLikeBullet(string text)
        {
            if (text.Length < 2)
            {
                return false;
            }

            char c = text[0];
            return (c == '•' || c == '-' || c == '*') && char.IsWhiteSpace(text[1]);
        }
    }
}
