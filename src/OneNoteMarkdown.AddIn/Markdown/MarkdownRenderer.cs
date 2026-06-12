using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OneNoteMarkdown.Markdown
{
    /// <summary>
    /// Phase 2 Markdown renderer: headings + list items.
    /// </summary>
    internal static class MarkdownRenderer
    {
        private static readonly Regex UnorderedRegex = new Regex("^([-+*])\\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex OrderedRegex = new Regex("^(\\d+)[.)]\\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex TaskRegex = new Regex("^\\[( |x|X)\\]\\s+(.+)$", RegexOptions.Compiled);
        private static readonly HashSet<string> DiagramLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mermaid", "mindmap", "flow", "sequence"
        };

        public static List<MarkdownBlock> RenderToBlocks(string markdown)
        {
            List<MarkdownBlock> blocks = new List<MarkdownBlock>();
            if (string.IsNullOrEmpty(markdown))
            {
                return blocks;
            }

            string[] lines = markdown
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n');

            bool inCode = false;
            string codeFence = null;
            string codeLanguage = string.Empty;
            List<string> codeLines = new List<string>();
            bool inLatex = false;
            List<string> latexLines = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i] ?? string.Empty;
                string trimmed = line.Trim();

                if (inLatex)
                {
                    if (trimmed == "$$")
                    {
                        FlushLatexBlock(blocks, latexLines);
                        latexLines.Clear();
                        inLatex = false;
                    }
                    else
                    {
                        latexLines.Add(line);
                    }
                    continue;
                }

                if (inCode)
                {
                    if (trimmed.StartsWith(codeFence))
                    {
                        FlushCodeBlock(blocks, codeLines, codeLanguage);
                        codeLines.Clear();
                        inCode = false;
                        codeFence = null;
                        codeLanguage = string.Empty;
                    }
                    else
                    {
                        codeLines.Add(line);
                    }
                    continue;
                }

                if (trimmed.StartsWith("```") || trimmed.StartsWith("~~~"))
                {
                    inCode = true;
                    codeFence = trimmed.StartsWith("~~~") ? "~~~" : "```";
                    codeLanguage = ParseFenceLanguage(trimmed, codeFence);
                    continue;
                }

                if (trimmed == "$$")
                {
                    inLatex = true;
                    latexLines.Clear();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    blocks.Add(MarkdownBlock.Blank());
                    continue;
                }

                if (string.Equals(trimmed, "[TOC]", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(trimmed, "[[_TOC_]]", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(trimmed, "{:toc}", StringComparison.OrdinalIgnoreCase))
                {
                    blocks.Add(MarkdownBlock.Toc());
                    continue;
                }

                ListParseResult list;
                if (TryParseListItem(line, out list))
                {
                    blocks.Add(MarkdownBlock.ListItem(list.Text, list.IndentLevel, list.Kind, list.OrderedNumber, list.IsTaskChecked));
                    continue;
                }

                if (IsHorizontalRule(trimmed))
                {
                    blocks.Add(MarkdownBlock.HorizontalRule());
                    continue;
                }

                if (trimmed.StartsWith(">"))
                {
                    string quoted = trimmed.Substring(1);
                    if (quoted.StartsWith(" ")) quoted = quoted.Substring(1);
                    blocks.Add(MarkdownBlock.Blockquote(quoted));
                    continue;
                }

                int level;
                string headingText;
                if (TryParseAtxHeading(trimmed, out level, out headingText))
                {
                    blocks.Add(MarkdownBlock.Heading(level, headingText));
                    continue;
                }

                blocks.Add(MarkdownBlock.Paragraph(NormalizeParagraph(trimmed)));
            }

            if (inCode && codeLines.Count > 0)
            {
                FlushCodeBlock(blocks, codeLines, codeLanguage);
            }
            if (inLatex && latexLines.Count > 0)
            {
                FlushLatexBlock(blocks, latexLines);
            }

            return blocks;
        }

        private static bool IsHorizontalRule(string trimmed)
        {
            // A horizontal rule is a line of 3+ identical -, * or _ (spaces allowed).
            if (string.IsNullOrEmpty(trimmed)) return false;
            string compact = trimmed.Replace(" ", string.Empty);
            if (compact.Length < 3) return false;
            char c = compact[0];
            if (c != '-' && c != '*' && c != '_') return false;
            for (int i = 0; i < compact.Length; i++)
            {
                if (compact[i] != c) return false;
            }
            return true;
        }

        private static bool TryParseListItem(string line, out ListParseResult result)
        {
            result = default(ListParseResult);
            if (string.IsNullOrEmpty(line)) return false;


            int leadingColumns;
            int contentStart;
            MeasureLeadingWhitespace(line, out leadingColumns, out contentStart);
            string rest = line.Substring(contentStart);
            if (rest.Length == 0) return false;

            Match mUnordered = UnorderedRegex.Match(rest);
            if (mUnordered.Success)
            {
                int indent = leadingColumns / 2;
                string tail = mUnordered.Groups[2].Value.Trim();
                Match mTask = TaskRegex.Match(tail);
                if (mTask.Success)
                {
                    result = new ListParseResult
                    {
                        IndentLevel = indent,
                        Kind = MarkdownListKind.Task,
                        OrderedNumber = 1,
                        IsTaskChecked = string.Equals(mTask.Groups[1].Value, "x", StringComparison.OrdinalIgnoreCase),
                        Text = mTask.Groups[2].Value.Trim()
                    };
                    return true;
                }

                result = new ListParseResult
                {
                    IndentLevel = indent,
                    Kind = MarkdownListKind.Unordered,
                    OrderedNumber = 1,
                    IsTaskChecked = false,
                    Text = tail
                };
                return true;
            }

            Match mOrdered = OrderedRegex.Match(rest);
            if (mOrdered.Success)
            {
                int indent = leadingColumns / 2;
                int number;
                if (!int.TryParse(mOrdered.Groups[1].Value, out number) || number < 1)
                {
                    number = 1;
                }

                result = new ListParseResult
                {
                    IndentLevel = indent,
                    Kind = MarkdownListKind.Ordered,
                    OrderedNumber = number,
                    IsTaskChecked = false,
                    Text = mOrdered.Groups[2].Value.Trim()
                };
                return true;
            }

            return false;
        }

        private static void MeasureLeadingWhitespace(string line, out int columns, out int chars)
        {
            columns = 0;
            chars = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == ' ')
                {
                    columns += 1;
                    chars++;
                    continue;
                }

                if (c == '\t')
                {
                    columns += 4;
                    chars++;
                    continue;
                }

                break;
            }
        }

        private static bool TryParseAtxHeading(string trimmed, out int level, out string text)
        {
            level = 0;
            text = null;

            int hashes = 0;
            while (hashes < trimmed.Length && trimmed[hashes] == '#') hashes++;
            if (hashes == 0 || hashes > 6) return false;
            if (hashes >= trimmed.Length) return false;
            if (trimmed[hashes] != ' ') return false;

            string body = trimmed.Substring(hashes + 1).Trim();
            // Strip optional closing hashes (CommonMark allows `# Title #`).
            body = body.TrimEnd('#').TrimEnd();
            if (body.Length == 0) return false;

            level = hashes;
            text = body;
            return true;
        }

        private static string NormalizeParagraph(string trimmed)
        {
            // Inline emphasis (**bold**, *italic*, `code`, ~~strike~~) is intentionally
            // left as literal characters during phase 1. Inline styling will be added
            // by a pair-wise regex pass after the heading block path proves stable.
            return trimmed;
        }

        private static void FlushCodeBlock(List<MarkdownBlock> blocks, List<string> codeLines, string language)
        {
            if (codeLines == null || codeLines.Count == 0) return;
            string[] normalized = new string[codeLines.Count];
            for (int i = 0; i < codeLines.Count; i++)
            {
                normalized[i] = codeLines[i] ?? string.Empty;
            }
            string text = string.Join("\n", normalized);
            string lang = (language ?? string.Empty).Trim();
            if (DiagramLanguages.Contains(lang))
            {
                blocks.Add(MarkdownBlock.DiagramBlock(text, lang));
                return;
            }
            blocks.Add(MarkdownBlock.CodeBlock(text, lang));
        }

        private static void FlushLatexBlock(List<MarkdownBlock> blocks, List<string> latexLines)
        {
            if (latexLines == null || latexLines.Count == 0) return;
            string[] normalized = new string[latexLines.Count];
            for (int i = 0; i < latexLines.Count; i++)
            {
                normalized[i] = latexLines[i] ?? string.Empty;
            }
            string text = string.Join("\n", normalized).Trim();
            if (text.Length > 0)
            {
                blocks.Add(MarkdownBlock.LatexBlock(text));
            }
        }

        private static string ParseFenceLanguage(string trimmedFenceLine, string fence)
        {
            if (string.IsNullOrEmpty(trimmedFenceLine) || string.IsNullOrEmpty(fence)) return string.Empty;
            if (!trimmedFenceLine.StartsWith(fence, StringComparison.Ordinal)) return string.Empty;
            string tail = trimmedFenceLine.Substring(fence.Length).Trim();
            if (string.IsNullOrEmpty(tail)) return string.Empty;

            int split = tail.IndexOfAny(new[] { ' ', '\t' });
            if (split >= 0)
            {
                tail = tail.Substring(0, split);
            }
            return tail.Trim();
        }

        private struct ListParseResult
        {
            public int IndentLevel;
            public MarkdownListKind Kind;
            public int OrderedNumber;
            public bool IsTaskChecked;
            public string Text;
        }
    }
}
