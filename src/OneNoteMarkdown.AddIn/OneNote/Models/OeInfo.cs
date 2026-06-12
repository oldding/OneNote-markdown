namespace OneNoteMarkdown.OneNote.Models
{
    /// <summary>
    /// Carries the identifiers and Markdown source text of a single OneNote
    /// Outline Element (OE) — the unit that "Enter" renders.
    /// </summary>
    public class OeInfo
    {
        /// <summary>The page that owns this OE.</summary>
        public string PageId { get; set; }

        /// <summary>The objectID attribute of the &lt;one:OE&gt; element.</summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Raw Markdown source for this line.
        /// Recovered from the hidden &lt;one:Meta name="md-src"&gt; if present,
        /// otherwise reconstructed from the visible text of the OE.
        /// </summary>
        public string MarkdownSource { get; set; }
    }
}
