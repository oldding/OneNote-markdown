namespace OneNoteMarkdown.Markdown
{
    internal enum MarkdownBlockKind
    {
        Paragraph = 0,
        Heading = 1,
        Blank = 2,
        ListItem = 3,
        CodeBlock = 4,
        DiagramBlock = 5,
        LatexBlock = 6,
        Toc = 7,
        Blockquote = 8,
        HorizontalRule = 9
    }

    internal enum MarkdownListKind
    {
        None = 0,
        Unordered = 1,
        Ordered = 2,
        Task = 3
    }

    internal class MarkdownBlock
    {
        public MarkdownBlockKind Kind { get; set; }

        /// <summary>
        /// Plain text content (NOT HTML-encoded). PageWriter is responsible for
        /// encoding before placing the value inside an &lt;one:T&gt; CDATA block.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Heading level (1..6). Only meaningful when <see cref="Kind"/> is
        /// <see cref="MarkdownBlockKind.Heading"/>.
        /// </summary>
        public int Level { get; set; }

        public int IndentLevel { get; set; }

        public MarkdownListKind ListKind { get; set; }

        public int OrderedNumber { get; set; }

        public bool IsTaskChecked { get; set; }

        public string CodeLanguage { get; set; }

        public static MarkdownBlock Paragraph(string text)
        {
            return new MarkdownBlock { Kind = MarkdownBlockKind.Paragraph, Text = text ?? string.Empty, Level = 0 };
        }

        public static MarkdownBlock Heading(int level, string text)
        {
            if (level < 1) level = 1;
            if (level > 6) level = 6;
            return new MarkdownBlock { Kind = MarkdownBlockKind.Heading, Text = text ?? string.Empty, Level = level };
        }

        public static MarkdownBlock ListItem(string text, int indentLevel, MarkdownListKind listKind, int orderedNumber, bool isTaskChecked)
        {
            if (indentLevel < 0) indentLevel = 0;
            if (orderedNumber < 1) orderedNumber = 1;
            return new MarkdownBlock
            {
                Kind = MarkdownBlockKind.ListItem,
                Text = text ?? string.Empty,
                Level = 0,
                IndentLevel = indentLevel,
                ListKind = listKind,
                OrderedNumber = orderedNumber,
                IsTaskChecked = isTaskChecked
            };
        }

        public static MarkdownBlock CodeBlock(string code, string language)
        {
            return new MarkdownBlock
            {
                Kind = MarkdownBlockKind.CodeBlock,
                Text = code ?? string.Empty,
                CodeLanguage = language ?? string.Empty
            };
        }

        public static MarkdownBlock DiagramBlock(string code, string diagramType)
        {
            return new MarkdownBlock
            {
                Kind = MarkdownBlockKind.DiagramBlock,
                Text = code ?? string.Empty,
                CodeLanguage = diagramType ?? string.Empty
            };
        }

        public static MarkdownBlock LatexBlock(string latex)
        {
            return new MarkdownBlock
            {
                Kind = MarkdownBlockKind.LatexBlock,
                Text = latex ?? string.Empty,
                CodeLanguage = "latex"
            };
        }

        public static MarkdownBlock Toc()
        {
            return new MarkdownBlock
            {
                Kind = MarkdownBlockKind.Toc,
                Text = "[TOC]"
            };
        }

        public static MarkdownBlock Blank()
        {
            return new MarkdownBlock { Kind = MarkdownBlockKind.Blank, Text = string.Empty, Level = 0 };
        }

        public static MarkdownBlock Blockquote(string text)
        {
            return new MarkdownBlock { Kind = MarkdownBlockKind.Blockquote, Text = text ?? string.Empty, Level = 0 };
        }

        public static MarkdownBlock HorizontalRule()
        {
            return new MarkdownBlock { Kind = MarkdownBlockKind.HorizontalRule, Text = string.Empty, Level = 0 };
        }
    }
}
