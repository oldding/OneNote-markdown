using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace OneNoteMarkdown.Markdown
{
    /// <summary>
    /// Lightweight, language-agnostic syntax highlighter for fenced code blocks.
    ///
    /// It tokenizes source into a few coarse categories (comment, string, number,
    /// keyword, plain) and emits HtmlEncoded text wrapped in &lt;span style="color:..."&gt;.
    ///
    /// SAFETY INVARIANT: every piece of source text is HtmlEncoded before being
    /// placed inside a span. The only raw HTML produced is the span tags this
    /// class generates itself, preserving the &lt;one:T&gt; CDATA safety rule.
    /// </summary>
    internal static class CodeHighlighter
    {
        // VS-style palette tuned for a light (#f5f5f5) background.
        private const string ColorComment = "#008000";
        private const string ColorString  = "#a31515";
        private const string ColorNumber  = "#098658";
        private const string ColorKeyword = "#0000ff";

        private static readonly HashSet<string> CommonKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            // C-family / JS / TS / C# / Java shared keywords
            "abstract","async","await","base","bool","boolean","break","byte","case","catch","char","class","const",
            "continue","debugger","default","delegate","do","double","else","enum","event","explicit","export","extends",
            "false","final","finally","float","for","foreach","function","goto","if","implements","import","in","instanceof",
            "int","interface","internal","is","let","long","namespace","new","null","object","operator","out","override",
            "package","private","protected","public","readonly","ref","return","sealed","short","static","string","struct",
            "super","switch","synchronized","this","throw","throws","transient","true","try","typeof","using","var","virtual",
            "void","volatile","while","with","yield",
            // Python
            "and","as","assert","def","del","elif","except","from","global","lambda","nonlocal","not","or","pass","raise",
            "self","None","True","False",
            // SQL (upper handled case-insensitively below would over-match; keep lowercase set only)
        };

        /// <summary>
        /// Returns HTML (HtmlEncoded text + color spans) for the given code.
        /// Newlines are converted to &lt;br&gt; and leading spaces to &amp;nbsp; so
        /// indentation is preserved inside &lt;one:T&gt;.
        /// </summary>
        public static string Highlight(string code, string language)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;

            string normalized = code.Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            StringBuilder sb = new StringBuilder();

            for (int li = 0; li < lines.Length; li++)
            {
                if (li > 0) sb.Append("<br>");
                HighlightLine(lines[li] ?? string.Empty, sb);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns one highlighted HTML fragment per source line (no &lt;br&gt;).
        /// Used to place each code line in its own OneNote OE, which stacks
        /// vertically without relying on &lt;br&gt; rendering inside a single OE.
        /// </summary>
        public static List<string> HighlightLines(string code, string language)
        {
            List<string> result = new List<string>();
            if (code == null) return result;

            string normalized = code.Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            for (int li = 0; li < lines.Length; li++)
            {
                StringBuilder sb = new StringBuilder();
                HighlightLine(lines[li] ?? string.Empty, sb);
                string html = sb.ToString();
                // Empty lines need a non-breaking space so the OE has height.
                if (html.Length == 0) html = "&nbsp;";
                result.Add(html);
            }
            return result;
        }

        private static void HighlightLine(string line, StringBuilder sb)
        {
            int n = line.Length;
            int i = 0;

            // Preserve leading whitespace as nbsp.
            while (i < n && line[i] == ' ') { sb.Append("&nbsp;"); i++; }
            while (i < n && line[i] == '\t') { sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;"); i++; }

            while (i < n)
            {
                char c = line[i];

                // Line comments: // ... or # ... or -- ...
                if ((c == '/' && i + 1 < n && line[i + 1] == '/') ||
                    c == '#' ||
                    (c == '-' && i + 1 < n && line[i + 1] == '-'))
                {
                    AppendSpan(sb, line.Substring(i), ColorComment);
                    return;
                }

                // Block comment start /* ... */ (single-line portion only; multi-line
                // blocks are uncommon in short snippets and handled per line).
                if (c == '/' && i + 1 < n && line[i + 1] == '*')
                {
                    int end = line.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (end < 0) { AppendSpan(sb, line.Substring(i), ColorComment); return; }
                    AppendSpan(sb, line.Substring(i, end + 2 - i), ColorComment);
                    i = end + 2;
                    continue;
                }

                // Strings: " ... " or ' ... ' or ` ... `
                if (c == '"' || c == '\'' || c == '`')
                {
                    int j = i + 1;
                    while (j < n)
                    {
                        if (line[j] == '\\') { j += 2; continue; }
                        if (line[j] == c) { j++; break; }
                        j++;
                    }
                    if (j > n) j = n;
                    AppendSpan(sb, line.Substring(i, j - i), ColorString);
                    i = j;
                    continue;
                }

                // Numbers
                if (char.IsDigit(c))
                {
                    int j = i;
                    while (j < n && (char.IsLetterOrDigit(line[j]) || line[j] == '.' || line[j] == 'x' || line[j] == 'X')) j++;
                    AppendSpan(sb, line.Substring(i, j - i), ColorNumber);
                    i = j;
                    continue;
                }

                // Identifiers / keywords
                if (char.IsLetter(c) || c == '_' || c == '$')
                {
                    int j = i;
                    while (j < n && (char.IsLetterOrDigit(line[j]) || line[j] == '_' || line[j] == '$')) j++;
                    string word = line.Substring(i, j - i);
                    if (CommonKeywords.Contains(word))
                    {
                        AppendSpan(sb, word, ColorKeyword);
                    }
                    else
                    {
                        AppendEncoded(sb, word);
                    }
                    i = j;
                    continue;
                }

                // Any other single character (operators, punctuation, spaces).
                if (c == ' ') { sb.Append("&nbsp;"); i++; continue; }
                AppendEncoded(sb, c.ToString());
                i++;
            }
        }

        private static void AppendSpan(StringBuilder sb, string text, string color)
        {
            sb.Append("<span style=\"color:").Append(color).Append(";\">");
            AppendEncoded(sb, text);
            sb.Append("</span>");
        }

        private static void AppendEncoded(StringBuilder sb, string text)
        {
            // HtmlEncode then preserve interior spaces so alignment survives.
            sb.Append(WebUtility.HtmlEncode(text).Replace(" ", "&nbsp;"));
        }
    }
}
