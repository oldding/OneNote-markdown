using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OneNoteMarkdown.OneNote.Models;

namespace OneNoteMarkdown.OneNote
{
    public static class PageParser
    {
        private static readonly XNamespace OneNs = "http://schemas.microsoft.com/office/onenote/2013/onenote";
        private static readonly Regex BreakRegex = new Regex("<br\\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BlockCloseRegex = new Regex("</(p|div|li|h[1-6])>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TagRegex = new Regex("<[^>]+>", RegexOptions.Compiled);

        public static PageContent Parse(string pageXml)
        {
            if (string.IsNullOrWhiteSpace(pageXml)) throw new ArgumentException("Page XML cannot be null or empty.", nameof(pageXml));
            XDocument document = XDocument.Parse(pageXml);
            XElement pageElement = document.Root;
            if (pageElement == null) throw new InvalidOperationException("Page XML does not contain a root element.");

            PageContent page = new PageContent
            {
                PageId = (string)pageElement.Attribute("ID") ?? string.Empty,
                Title = GetPageTitle(pageElement),
                DateCreated = ParseDate((string)pageElement.Attribute("dateTime")),
                DateModified = ParseDate((string)pageElement.Attribute("lastModifiedTime"))
            };

            foreach (XElement outlineElement in pageElement.Elements(OneNs + "Outline"))
            {
                OutlineContent outline = new OutlineContent
                {
                    OutlineId = (string)outlineElement.Attribute("objectID") ?? (string)outlineElement.Attribute("ID") ?? string.Empty
                };

                XElement childrenElement = outlineElement.Element(OneNs + "OEChildren");
                if (childrenElement != null)
                {
                    foreach (XElement oeElement in childrenElement.Elements(OneNs + "OE"))
                    {
                        ParseOeElement(oeElement, outline, 0);
                    }
                }

                page.Outlines.Add(outline);
            }

            return page;
        }

        private static void ParseOeElement(XElement oeElement, OutlineContent outline, int indentLevel)
        {
            if (oeElement == null) return;
            string markdownSource = ReadMarkdownSource(oeElement);
            bool isContinuation = HasMeta(oeElement, "md-continuation");
            XElement textElement = oeElement.Element(OneNs + "T");
            if (textElement != null || !string.IsNullOrEmpty(markdownSource))
            {
                string rawHtml = textElement == null ? string.Empty : (textElement.Value ?? string.Empty);
                string plainText = StripHtml(rawHtml);
                if (!string.IsNullOrWhiteSpace(plainText) || !string.IsNullOrEmpty(markdownSource))
                {
                    outline.TextBlocks.Add(new TextBlock
                    {
                        ElementId = (string)oeElement.Attribute("objectID") ?? (string)oeElement.Attribute("ID") ?? string.Empty,
                        RawHtml = rawHtml,
                        Text = plainText,
                        MarkdownSource = markdownSource,
                        IsMarkdownContinuation = isContinuation,
                        IndentLevel = indentLevel
                    });
                }
            }

            XElement childrenElement = oeElement.Element(OneNs + "OEChildren");
            if (childrenElement == null) return;
            foreach (XElement childOe in childrenElement.Elements(OneNs + "OE"))
            {
                ParseOeElement(childOe, outline, indentLevel + 1);
            }
        }

        private static string GetPageTitle(XElement pageElement)
        {
            string title = (string)pageElement.Attribute("name");
            if (!string.IsNullOrWhiteSpace(title)) return title;
            XElement titleElement = pageElement.Element(OneNs + "Title");
            if (titleElement == null) return string.Empty;
            XElement textElement = titleElement.Descendants(OneNs + "T").FirstOrDefault();
            return textElement == null ? string.Empty : StripHtml(textElement.Value);
        }

        private static DateTime ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DateTime.MinValue;
            DateTime parsed;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsed)) return parsed;
            return DateTime.MinValue;
        }

        internal static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            string normalized = BreakRegex.Replace(html, "\n");
            normalized = BlockCloseRegex.Replace(normalized, "\n");
            string withoutTags = TagRegex.Replace(normalized, string.Empty);
            return WebUtility.HtmlDecode(withoutTags).Trim();
        }

        private static string ReadMarkdownSource(XElement oeElement)
        {
            XElement meta = oeElement.Elements(OneNs + "Meta")
                .FirstOrDefault(delegate(XElement element)
                {
                    return string.Equals((string)element.Attribute("name"), "md-src", StringComparison.Ordinal);
                });
            string encoded = meta == null ? null : (string)meta.Attribute("content");
            if (string.IsNullOrEmpty(encoded)) return string.Empty;

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool HasMeta(XElement oeElement, string name)
        {
            return oeElement.Elements(OneNs + "Meta")
                .Any(delegate(XElement element)
                {
                    return string.Equals((string)element.Attribute("name"), name, StringComparison.Ordinal);
                });
        }
    }
}
