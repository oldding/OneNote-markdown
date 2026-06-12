using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote.Interop;
using OneNoteMarkdown.Rendering;
using OneNoteMarkdown.Settings;

namespace OneNoteMarkdown.OneNote
{
    public class PageWriter
    {
        private static readonly XNamespace OneNs = "http://schemas.microsoft.com/office/onenote/2013/onenote";
        private static readonly Regex InlineCodeRegex = new Regex("(?<!`)`([^`\\r\\n]+?)`(?!`)", RegexOptions.Compiled);
        private static readonly Regex BoldRegex = new Regex("\\*\\*([^*\\r\\n]+?)\\*\\*", RegexOptions.Compiled);
        private static readonly Regex StrikeRegex = new Regex("~~([^~\\r\\n]+?)~~", RegexOptions.Compiled);
        private static readonly Regex ItalicRegex = new Regex("(?<!\\*)\\*([^*\\r\\n]+?)\\*(?!\\*)", RegexOptions.Compiled);
        private static readonly Regex InlineLatexRegex = new Regex("(?<!\\$)\\$([^$\\r\\n]+?)\\$(?!\\$)", RegexOptions.Compiled);
        private static readonly Regex HighlightRegex = new Regex("==([^=\\r\\n]+?)==", RegexOptions.Compiled);
        private static readonly Regex UnderlineRegex = new Regex("(?<!\\+)\\+\\+([^+\\r\\n]+?)\\+\\+(?!\\+)", RegexOptions.Compiled);

        // Cached settings: loaded once per add-in lifetime, re-loaded if null (e.g. first use).
        // Access via GetTheme() only; do not read _theme directly in instance methods.
        private static volatile ThemeSettings _theme;
        private static readonly object _themeLock = new object();

        private readonly IOneNoteApplication _app;

        public PageWriter()
        {
            object raw;
            try { raw = System.Runtime.InteropServices.Marshal.GetActiveObject("OneNote.Application"); }
            catch { raw = AddIn.Connect.Instance?.OneNoteApp; }
            if (raw == null) throw new InvalidOperationException("OneNote application not available.");
            _app = raw as IOneNoteApplication ?? throw new InvalidOperationException("Cast to IOneNoteApplication failed.");
        }

        /// <summary>
        /// Returns the cached ThemeSettings, loading from disk only on the first call
        /// or if the cache has been explicitly invalidated.
        /// </summary>
        private static ThemeSettings GetTheme()
        {
            ThemeSettings t = _theme;
            if (t != null) return t;
            lock (_themeLock)
            {
                if (_theme == null) _theme = ThemeSettings.Load();
                return _theme;
            }
        }

        /// <summary>
        /// Invalidates the settings cache so the next operation re-reads theme.ini.
        /// Call this after the user edits and saves theme.ini.
        /// </summary>
        public static void InvalidateThemeCache()
        {
            _theme = null;
        }

        public void AppendOutline(string pageId, string content, string heading)
        {
            ValidatePageId(pageId);
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be null or empty.", nameof(content));

            List<MarkdownBlock> blocks = new List<MarkdownBlock> { MarkdownBlock.Paragraph(content) };
            AppendBlocksInternal(pageId, blocks, heading);
        }

        public void AppendParagraphs(string pageId, IEnumerable<string> paragraphs, string heading)
        {
            ValidatePageId(pageId);
            if (paragraphs == null) throw new ArgumentNullException(nameof(paragraphs));

            List<MarkdownBlock> blocks = paragraphs
                .Where(delegate(string p) { return p != null; })
                .Select(delegate(string p) { return MarkdownBlock.Paragraph(p.Trim()); })
                .ToList();

            if (blocks.Count == 0) throw new ArgumentException("Paragraphs cannot be empty.", nameof(paragraphs));

            AppendBlocksInternal(pageId, blocks, heading);
        }

        /// <summary>
        /// Structured block append. Heading blocks are emitted via OneNote's
        /// QuickStyle table; paragraph blocks fall back to the normal style. The
        /// inner text is always HtmlEncoded before being placed inside &lt;one:T&gt;.
        /// </summary>
        internal void AppendBlocks(string pageId, IList<MarkdownBlock> blocks, string heading)
        {
            AppendBlocksInternal(pageId, blocks, heading);
        }

        internal void UpsertManagedBlocks(string pageId, string role, IList<MarkdownBlock> blocks, string heading)
        {
            ValidatePageId(pageId);
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role cannot be null or empty.", nameof(role));
            if (string.IsNullOrWhiteSpace(heading)) throw new ArgumentException("Heading cannot be null or empty.", nameof(heading));
            if (blocks == null) throw new ArgumentNullException(nameof(blocks));

            List<MarkdownBlock> effective = new List<MarkdownBlock>(blocks.Count);
            for (int i = 0; i < blocks.Count; i++)
            {
                MarkdownBlock block = blocks[i];
                if (block == null) continue;
                effective.Add(block);
            }
            if (effective.Count == 0) throw new ArgumentException("Blocks cannot be empty.", nameof(blocks));

            XDocument pageDoc = GetPageDocument(pageId);
            XElement pageElement = pageDoc.Root;
            HashSet<string> needed = CollectNeededStyleNames(heading, effective);
            Dictionary<string, int> nameToIndex = EnsurePageQuickStyles(pageElement, needed);

            XElement existing = FindManagedOutlineByHeading(pageElement, heading);
            double y = ReadOutlineY(existing, CalculateNextOutlineY(pageDoc));
            XElement outlineElement = CreateOutlineElement(y, heading, effective, nameToIndex);

            if (existing == null)
            {
                pageElement.Add(outlineElement);
            }
            else
            {
                existing.ReplaceWith(outlineElement);
            }

            UpdatePage(pageDoc);
        }

        internal void UpsertManagedSource(string pageId, string role, string markdown, string heading)
        {
            string normalized = (markdown ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            List<MarkdownBlock> blocks = new List<MarkdownBlock>(lines.Length == 0 ? 1 : lines.Length);
            if (lines.Length == 0)
            {
                blocks.Add(MarkdownBlock.Blank());
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i] ?? string.Empty;
                    blocks.Add(line.Length == 0 ? MarkdownBlock.Blank() : MarkdownBlock.Paragraph(line));
                }
            }

            UpsertManagedBlocks(pageId, role, blocks, heading);
        }

        private void AppendBlocksInternal(string pageId, IList<MarkdownBlock> blocks, string heading)
        {
            ValidatePageId(pageId);
            if (blocks == null) throw new ArgumentNullException(nameof(blocks));

            List<MarkdownBlock> effective = new List<MarkdownBlock>(blocks.Count);
            for (int i = 0; i < blocks.Count; i++)
            {
                MarkdownBlock block = blocks[i];
                if (block == null) continue;
                effective.Add(block);
            }
            if (effective.Count == 0) throw new ArgumentException("Blocks cannot be empty.", nameof(blocks));

            XDocument pageDoc = GetPageDocument(pageId);
            XElement pageElement = pageDoc.Root;

            // QuickStyleDef elements live at <one:Page> level, NOT inside <one:Outline>.
            // Ensure each style this append needs exists on the page, reusing any
            // pre-existing definition with the same name to avoid index collisions.
            HashSet<string> needed = CollectNeededStyleNames(heading, effective);
            Dictionary<string, int> nameToIndex = EnsurePageQuickStyles(pageElement, needed);

            double nextY = CalculateNextOutlineY(pageDoc);
            XElement outlineElement = CreateOutlineElement(nextY, heading, effective, nameToIndex);
            pageElement.Add(outlineElement);
            UpdatePage(pageDoc);
        }

        private static HashSet<string> CollectNeededStyleNames(string heading, IList<MarkdownBlock> blocks)
        {
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            names.Add("p");
            for (int i = 0; i < blocks.Count; i++)
            {
                MarkdownBlock block = blocks[i];
                if (block.Kind == MarkdownBlockKind.Heading)
                {
                    int level = block.Level;
                    if (level < 1) level = 1;
                    if (level > 6) level = 6;
                    names.Add("h" + level.ToString(CultureInfo.InvariantCulture));
                }
                else if (block.Kind == MarkdownBlockKind.CodeBlock)
                {
                    names.Add("code");
                }
                else if (block.Kind == MarkdownBlockKind.DiagramBlock)
                {
                    names.Add("diagram");
                }
                else if (block.Kind == MarkdownBlockKind.LatexBlock)
                {
                    names.Add("latex");
                }
                else if (block.Kind == MarkdownBlockKind.Toc)
                {
                    names.Add("toc");
                }
            }
            return names;
        }

        private static Dictionary<string, int> EnsurePageQuickStyles(XElement pageElement, HashSet<string> neededNames)
        {
            Dictionary<string, int> nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int maxIndex = -1;

            // First, harvest existing QuickStyleDef entries on the page.
            List<XElement> existing = pageElement.Elements(OneNs + "QuickStyleDef").ToList();
            for (int i = 0; i < existing.Count; i++)
            {
                XElement def = existing[i];
                string name = (string)def.Attribute("name");
                int idx;
                if (!int.TryParse((string)def.Attribute("index"), NumberStyles.Integer, CultureInfo.InvariantCulture, out idx)) continue;
                if (idx > maxIndex) maxIndex = idx;
                if (!string.IsNullOrEmpty(name) && !nameToIndex.ContainsKey(name))
                {
                    nameToIndex[name] = idx;
                }
            }

            // Then, add any missing definitions. New entries must be inserted in a
            // position OneNote accepts: QuickStyleDef belongs near the top of <one:Page>,
            // before <one:Outline> children. We insert as the last QuickStyleDef so order
            // among definitions is preserved.
            XElement insertAfter = existing.Count > 0 ? existing[existing.Count - 1] : null;

            foreach (string name in neededNames)
            {
                if (nameToIndex.ContainsKey(name)) continue;
                maxIndex++;
                XElement def = BuildQuickStyleDef(name, maxIndex);
                if (insertAfter != null)
                {
                    insertAfter.AddAfterSelf(def);
                }
                else
                {
                    // No existing QuickStyleDef on the page. Insert before the first
                    // Outline (or before the first child) so the schema-required
                    // ordering is preserved.
                    XElement firstOutline = pageElement.Elements(OneNs + "Outline").FirstOrDefault();
                    if (firstOutline != null)
                    {
                        firstOutline.AddBeforeSelf(def);
                    }
                    else
                    {
                        pageElement.Add(def);
                    }
                }
                insertAfter = def;
                nameToIndex[name] = maxIndex;
            }

            return nameToIndex;
        }

        /// <summary>
        /// Builds a fully-populated &lt;one:QuickStyleDef&gt; element. OneNote's schema
        /// requires the font* attributes as well as the spacing/highlight attributes;
        /// supplying only index+name results in "缺少必需属性 'font'".
        /// The values below mirror OneNote's built-in defaults for each style name.
        /// </summary>
        private static XElement BuildQuickStyleDef(string name, int index)
        {
            string fontSize;
            string fontColor = "automatic";
            string highlightColor = "automatic";
            string spaceBefore = "0.0";
            string spaceAfter = "0.0";
            string font = GetTheme().DefaultFontFamily;
            string paraSize = GetTheme().ParagraphFontSize.ToString("0.###", CultureInfo.InvariantCulture);
            string codeSize = GetTheme().CodeFontSize.ToString("0.###", CultureInfo.InvariantCulture);

            switch (name.ToLowerInvariant())
            {
                case "pagetitle": fontSize = "20.0"; break;
                case "h1": fontSize = "16.0"; break;
                case "h2": fontSize = "14.0"; break;
                case "h3": fontSize = "12.0"; break;
                case "h4": fontSize = paraSize; break;
                case "h5": fontSize = paraSize; break;
                case "h6": fontSize = paraSize; break;
                case "code": fontSize = codeSize; font = GetTheme().MonospaceFontFamily; fontColor = "#1f1f1f"; highlightColor = "#f5f5f5"; break;
                case "diagram": fontSize = codeSize; font = GetTheme().MonospaceFontFamily; break;
                case "latex": fontSize = paraSize; font = GetTheme().MathFontFamily; break;
                case "toc": fontSize = "10.5"; break;
                case "p":
                default: fontSize = paraSize; break;
            }

            return new XElement(OneNs + "QuickStyleDef",
                new XAttribute("index", index.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("name", name),
                new XAttribute("fontColor", fontColor),
                new XAttribute("highlightColor", highlightColor),
                new XAttribute("font", font),
                new XAttribute("fontSize", fontSize),
                new XAttribute("spaceBefore", spaceBefore),
                new XAttribute("spaceAfter", spaceAfter));
        }

        private XDocument GetPageDocument(string pageId)
        {
            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) throw new InvalidOperationException("OneNote returned empty page XML.");
            return XDocument.Parse(xml);
        }

        private static XElement FindManagedOutlineByHeading(XElement pageElement, string heading)
        {
            if (pageElement == null || string.IsNullOrWhiteSpace(heading)) return null;
            return pageElement.Elements(OneNs + "Outline").FirstOrDefault(delegate(XElement outline)
            {
                XElement firstT = outline.Descendants(OneNs + "T").FirstOrDefault();
                if (firstT == null) return false;
                string plain = StripHtml(firstT.Value);
                return string.Equals((plain ?? string.Empty).Trim(), heading.Trim(), StringComparison.Ordinal);
            });
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            string withoutTags = Regex.Replace(html, "<[^>]+>", string.Empty);
            return WebUtility.HtmlDecode(withoutTags).Trim();
        }

        private double ReadOutlineY(XElement outline, double fallback)
        {
            if (outline == null) return fallback;
            XElement pos = outline.Element(OneNs + "Position");
            if (pos == null) return fallback;
            return ParseDouble((string)pos.Attribute("y"), fallback);
        }

        private void UpdatePage(XDocument pageDoc)
        {
            _app.UpdatePageContent(pageDoc.ToString(SaveOptions.DisableFormatting), DateTime.MinValue, XMLSchema.xs2013, false);
        }

        private double CalculateNextOutlineY(XDocument pageDoc)
        {
            double maxBottom = 0d;
            bool hasExistingOutline = false;
            foreach (XElement outlineElement in pageDoc.Descendants(OneNs + "Outline"))
            {
                XElement positionElement = outlineElement.Element(OneNs + "Position");
                if (positionElement == null) continue;
                double y = ParseDouble((string)positionElement.Attribute("y"), double.NaN);
                if (double.IsNaN(y)) continue;
                XElement sizeElement = outlineElement.Element(OneNs + "Size");
                double height = ParseDouble(sizeElement == null ? null : (string)sizeElement.Attribute("height"), double.NaN);
                if (double.IsNaN(height) || height <= 0d) height = 80d;
                double bottom = y + height;
                if (!hasExistingOutline || bottom > maxBottom)
                {
                    maxBottom = bottom;
                    hasExistingOutline = true;
                }
            }

            return hasExistingOutline ? maxBottom + 40d : 200d;
        }

        private XElement CreateOutlineElement(double y, string heading, IList<MarkdownBlock> blocks, Dictionary<string, int> nameToIndex)
        {
            XElement outline = new XElement(OneNs + "Outline",
                new XElement(OneNs + "Position",
                    new XAttribute("x", FormatDouble(36d)),
                    new XAttribute("y", FormatDouble(y))));

            XElement childrenElement = new XElement(OneNs + "OEChildren");
            int normalIndex = LookupStyle(nameToIndex, "p");
            if (!string.IsNullOrWhiteSpace(heading))
            {
                childrenElement.Add(CreateStyledOe(heading.Trim(), normalIndex));
            }

            // Tracks the most recent list item per indent level so nested list items
            // can be placed under the previous level's OEChildren.
            List<XElement> lastListItemByIndent = new List<XElement>();

            for (int i = 0; i < blocks.Count; i++)
            {
                MarkdownBlock block = blocks[i];
                int styleIndex;
                string text;

                if (block.Kind == MarkdownBlockKind.ListItem)
                {
                    AppendListItem(childrenElement, lastListItemByIndent, block, normalIndex);
                    continue;
                }

                // Any non-list block ends the current list run.
                lastListItemByIndent.Clear();

                switch (block.Kind)
                {
                    case MarkdownBlockKind.Heading:
                        int level = block.Level;
                        if (level < 1) level = 1;
                        if (level > 6) level = 6;
                        styleIndex = LookupStyle(nameToIndex, "h" + level.ToString(CultureInfo.InvariantCulture));
                        text = block.Text ?? string.Empty;
                        childrenElement.Add(CreateStyledOe(text, styleIndex, false, true));
                        break;
                    case MarkdownBlockKind.Blank:
                        styleIndex = normalIndex;
                        text = string.Empty;
                        childrenElement.Add(CreateStyledOe(text, styleIndex));
                        break;
                    case MarkdownBlockKind.CodeBlock:
                        styleIndex = LookupStyle(nameToIndex, "code");
                        foreach (XElement codeOe in CreateCodeOes(block, styleIndex))
                        {
                            childrenElement.Add(codeOe);
                        }
                        break;
                    case MarkdownBlockKind.DiagramBlock:
                        styleIndex = LookupStyle(nameToIndex, "diagram");
                        text = BuildDiagramPlaceholder(block);
                        XElement diagramOe = CreateDiagramOe(block, styleIndex, text);
                        childrenElement.Add(diagramOe);
                        break;
                    case MarkdownBlockKind.LatexBlock:
                        styleIndex = LookupStyle(nameToIndex, "latex");
                        text = BuildLatexPlaceholder(block);
                        XElement latexOe = CreateLatexOe(block, styleIndex, text);
                        childrenElement.Add(latexOe);
                        break;
                    case MarkdownBlockKind.Toc:
                        styleIndex = LookupStyle(nameToIndex, "toc");
                        text = BuildTocPlaceholder(blocks);
                        childrenElement.Add(CreateStyledOe(text, styleIndex, false, false));
                        break;
                    case MarkdownBlockKind.Blockquote:
                        styleIndex = normalIndex;
                        text = "❝ " + (block.Text ?? string.Empty);
                        childrenElement.Add(CreateStyledOe(text, styleIndex, false, true));
                        break;
                    case MarkdownBlockKind.HorizontalRule:
                        styleIndex = normalIndex;
                        childrenElement.Add(CreateStyledOe(new string('─', 30), styleIndex, false, false));
                        break;
                    default:
                        styleIndex = normalIndex;
                        text = block.Text ?? string.Empty;
                        childrenElement.Add(CreateStyledOe(text, styleIndex, false, true));
                        break;
                }
            }

            outline.Add(childrenElement);
            return outline;
        }

        private static int LookupStyle(Dictionary<string, int> nameToIndex, string name)
        {
            int idx;
            if (nameToIndex != null && nameToIndex.TryGetValue(name, out idx)) return idx;
            // Should not happen because EnsurePageQuickStyles seeds every needed name,
            // but fall back to -1 (omit attribute) rather than throw.
            return -1;
        }

        private void AppendListItem(XElement rootChildren, List<XElement> lastListItemByIndent, MarkdownBlock block, int normalStyleIndex)
        {
            int indent = block.IndentLevel;
            if (indent < 0) indent = 0;

            while (lastListItemByIndent.Count > indent + 1)
            {
                lastListItemByIndent.RemoveAt(lastListItemByIndent.Count - 1);
            }

            XElement item = CreateStyledOe(BuildListMarker(block) + (block.Text ?? string.Empty), normalStyleIndex, false, true);

            if (indent == 0)
            {
                rootChildren.Add(item);
            }
            else
            {
                XElement parent = indent - 1 < lastListItemByIndent.Count ? lastListItemByIndent[indent - 1] : null;
                if (parent == null)
                {
                    rootChildren.Add(item);
                }
                else
                {
                    EnsureOeChildren(parent).Add(item);
                }
            }

            while (lastListItemByIndent.Count < indent + 1)
            {
                lastListItemByIndent.Add(null);
            }
            lastListItemByIndent[indent] = item;
        }

        private string BuildListMarker(MarkdownBlock block)
        {
            switch (block.ListKind)
            {
                case MarkdownListKind.Ordered:
                    int n = block.OrderedNumber < 1 ? 1 : block.OrderedNumber;
                    return n.ToString(CultureInfo.InvariantCulture) + ". ";
                case MarkdownListKind.Task:
                    return block.IsTaskChecked ? "☑ " : "☐ ";
                default:
                    return "• ";
            }
        }

        private static XElement EnsureOeChildren(XElement oe)
        {
            XElement children = oe.Element(OneNs + "OEChildren");
            if (children != null) return children;
            children = new XElement(OneNs + "OEChildren");
            oe.Add(children);
            return children;
        }

        private static string BuildDiagramPlaceholder(MarkdownBlock block)
        {
            string type = string.IsNullOrWhiteSpace(block.CodeLanguage) ? "diagram" : block.CodeLanguage.Trim().ToLowerInvariant();
            string body = (block.Text ?? string.Empty).Trim();
            if (body.Length == 0)
            {
                return "[" + type + "]";
            }
            return "[" + type + "]\n" + body;
        }

        private string BuildCodeDisplayText(MarkdownBlock block)
        {
            string body = block == null ? string.Empty : (block.Text ?? string.Empty);
            if (!GetTheme().EnableCodeLineNumber)
            {
                return body;
            }

            string[] lines = body.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            if (lines.Length == 0)
            {
                return body;
            }

            int pad = lines.Length.ToString(CultureInfo.InvariantCulture).Length;
            List<string> withNo = new List<string>(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                string no = (i + 1).ToString(CultureInfo.InvariantCulture).PadLeft(pad);
                withNo.Add(no + " | " + lines[i]);
            }
            return string.Join("\n", withNo);
        }

        private static string BuildLatexPlaceholder(MarkdownBlock block)
        {
            string body = (block.Text ?? string.Empty).Trim();
            if (body.Length == 0)
            {
                return "$$ $$";
            }
            return "$$\n" + body + "\n$$";
        }

        private XElement CreateLatexOe(MarkdownBlock block, int styleIndex, string fallbackText)
        {
            if (!GetTheme().EnableLatexToImage)
            {
                return CreateStyledOe(fallbackText, styleIndex, true, false);
            }

            string latex = block == null ? string.Empty : (block.Text ?? string.Empty);
            if (latex.Trim().Length > 0)
            {
                LatexImageRenderer renderer = new LatexImageRenderer();
                byte[] pngBytes;
                int pixelWidth;
                int pixelHeight;
                if (renderer.TryRenderToPng(latex, GetTheme().MathFontFamily, out pngBytes, out pixelWidth, out pixelHeight))
                {
                    return CreateImageOe(pngBytes, pixelWidth, pixelHeight, styleIndex, "LaTeX");
                }
                Logger.Warn("CreateLatexOe: WpfMath render failed, fallback to plain text.");
            }

            return CreateStyledOe(fallbackText, styleIndex, true, false);
        }

        private XElement CreateImageOe(byte[] imageBytes, int pixelWidth, int pixelHeight, int styleIndex, string alt)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("Image bytes cannot be empty.", nameof(imageBytes));
            }

            XElement oe = new XElement(OneNs + "OE");
            if (styleIndex >= 0)
            {
                oe.Add(new XAttribute("quickStyleIndex", styleIndex.ToString(CultureInfo.InvariantCulture)));
            }

            double width = pixelWidth <= 0 ? 1d : (double)pixelWidth;
            double height = pixelHeight <= 0 ? 1d : (double)pixelHeight;

            XElement image = new XElement(OneNs + "Image",
                new XAttribute("format", "png"),
                new XAttribute("alt", string.IsNullOrWhiteSpace(alt) ? "image" : alt),
                new XElement(OneNs + "Size",
                    new XAttribute("width", width.ToString("0.###", CultureInfo.InvariantCulture)),
                    new XAttribute("height", height.ToString("0.###", CultureInfo.InvariantCulture))),
                new XElement(OneNs + "Data", Convert.ToBase64String(imageBytes)));

            oe.Add(image);
            return oe;
        }

        private XElement CreateDiagramOe(MarkdownBlock block, int styleIndex, string fallbackText)
        {
            // Safety-first fallback: keep diagram source as text block.
            // Image injection for diagrams will be re-enabled after schema-
            // level validation on multiple OneNote builds.
            return CreateStyledOe(fallbackText, styleIndex, true, false);
        }

        private static string BuildTocPlaceholder(IList<MarkdownBlock> blocks)
        {
            if (blocks == null || blocks.Count == 0)
            {
                return "目录\n- (无可用标题)";
            }

            List<string> lines = new List<string>();
            lines.Add("目录");
            int count = 0;
            for (int i = 0; i < blocks.Count; i++)
            {
                MarkdownBlock block = blocks[i];
                if (block == null || block.Kind != MarkdownBlockKind.Heading) continue;
                int level = block.Level;
                if (level < 1) level = 1;
                if (level > 6) level = 6;
                string indent = new string(' ', (level - 1) * 2);
                string text = (block.Text ?? string.Empty).Trim();
                if (text.Length == 0) continue;
                lines.Add(indent + "- " + text);
                count++;
            }
            if (count == 0)
            {
                lines.Add("- (无可用标题)");
            }
            return string.Join("\n", lines);
        }

        private XElement CreateStyledOe(string plainText, int quickStyleIndex, bool preserveWhitespace, bool enableInlineFormatting)
        {
            XElement oe = new XElement(OneNs + "OE");
            if (quickStyleIndex >= 0)
            {
                oe.Add(new XAttribute("quickStyleIndex", quickStyleIndex.ToString(CultureInfo.InvariantCulture)));
            }
            oe.Add(new XElement(OneNs + "T", new XCData(SanitizeCData(ToHtmlContent(plainText, preserveWhitespace, enableInlineFormatting)))));
            return oe;
        }

        /// <summary>
        /// Builds the OEs for a code block: one flat, sibling OE per source line.
        /// Returning siblings (rather than nesting children) keeps every line at
        /// the same indent level. CodeHighlighter HtmlEncodes every token, so the
        /// CDATA safety rule holds.
        /// </summary>
        private List<XElement> CreateCodeOes(MarkdownBlock block, int quickStyleIndex)
        {
            string displayText = BuildCodeDisplayText(block);
            string lang = block == null ? string.Empty : (block.CodeLanguage ?? string.Empty);
            List<string> lineHtml = Markdown.CodeHighlighter.HighlightLines(displayText, lang);
            if (lineHtml.Count == 0) lineHtml.Add("&nbsp;");

            List<XElement> oes = new List<XElement>(lineHtml.Count);
            for (int i = 0; i < lineHtml.Count; i++)
            {
                oes.Add(CreateCodeLineOe(lineHtml[i], quickStyleIndex));
            }
            return oes;
        }

        private XElement CreateCodeLineOe(string lineHtml, int quickStyleIndex)
        {
            XElement oe = new XElement(OneNs + "OE");
            if (quickStyleIndex >= 0)
            {
                oe.Add(new XAttribute("quickStyleIndex", quickStyleIndex.ToString(CultureInfo.InvariantCulture)));
            }
            oe.Add(new XElement(OneNs + "T", new XCData(SanitizeCData(lineHtml))));
            return oe;
        }

        private XElement CreateStyledOe(string plainText, int quickStyleIndex)
        {
            // Emergency safety mode: disable inline HTML styling for normal text.
            return CreateStyledOe(plainText, quickStyleIndex, false, false);
        }

        private string ToHtmlContent(string content, bool preserveWhitespace, bool enableInlineFormatting)
        {
            // SAFETY INVARIANT: <one:T> CDATA must never receive raw HTML the
            // renderer did not produce itself. This method always HtmlEncodes the
            // caller's text and only converts line breaks into <br>.
            string normalized = (content ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
            if (!preserveWhitespace)
            {
                normalized = normalized.Trim();
                if (normalized.Length == 0) return string.Empty;
                string encoded = WebUtility.HtmlEncode(normalized);
                if (enableInlineFormatting)
                {
                    encoded = ApplyInlineMarkdownStyles(encoded);
                }
                return encoded.Replace("\n", "<br>");
            }

            normalized = normalized.Replace("\t", "    ");
            string encodedPreserved = WebUtility.HtmlEncode(normalized)
                .Replace(" ", "&nbsp;")
                .Replace("\n", "<br>");
            return encodedPreserved.Length == 0 ? "&nbsp;" : encodedPreserved;
        }

        private static string ApplyInlineMarkdownStyles(string encoded)
        {
            if (string.IsNullOrEmpty(encoded)) return string.Empty;

            List<string> codeReplacements = new List<string>();
            string protectedText = InlineCodeRegex.Replace(encoded, delegate(Match m)
            {
                string inner = m.Groups[1].Value;
                string replacement = "<span style=\"font-family:Consolas;background-color:#f3f3f3;\">" + inner + "</span>";
                codeReplacements.Add(replacement);
                return "@@CODE" + (codeReplacements.Count - 1).ToString(CultureInfo.InvariantCulture) + "@@";
            });

            protectedText = BoldRegex.Replace(protectedText, "<span style=\"font-weight:bold;\">$1</span>");
            protectedText = StrikeRegex.Replace(protectedText, "<span style=\"text-decoration:line-through;\">$1</span>");
            protectedText = HighlightRegex.Replace(protectedText, "<span style=\"background-color:#ffff00;\">$1</span>");
            protectedText = UnderlineRegex.Replace(protectedText, "<span style=\"text-decoration:underline;\">$1</span>");
            protectedText = ItalicRegex.Replace(protectedText, "<span style=\"font-style:italic;\">$1</span>");
            protectedText = InlineLatexRegex.Replace(protectedText, "<span style=\"font-family:'Cambria Math';\">$1</span>");

            for (int i = 0; i < codeReplacements.Count; i++)
            {
                protectedText = protectedText.Replace("@@CODE" + i.ToString(CultureInfo.InvariantCulture) + "@@", codeReplacements[i]);
            }

            return protectedText;
        }

        private string SanitizeCData(string value)
        {
            return (value ?? string.Empty).Replace("]]>", "]]&gt;");
        }

        private static double ParseDouble(string value, double defaultValue)
        {
            double parsed;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed)) return parsed;
            return defaultValue;
        }

        private static string FormatDouble(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Renders a single OE in-place: replaces its content with rendered blocks
        /// and stores the original Markdown source in a hidden Meta element.
        /// If objectId is empty or not found on the page, falls back to appending
        /// a new outline with the rendered content.
        /// </summary>
        internal void ReplaceOeWithRenderedBlocks(string pageId, string objectId, string markdownSource, IList<MarkdownBlock> blocks)
        {
            ValidatePageId(pageId);
            if (blocks == null || blocks.Count == 0) return;

            XDocument pageDoc = GetPageDocument(pageId);
            XElement pageElement = pageDoc.Root;

            // Ensure all needed QuickStyleDef entries exist on the page.
            HashSet<string> needed = CollectNeededStyleNames(string.Empty, blocks);
            Dictionary<string, int> nameToIndex = EnsurePageQuickStyles(pageElement, needed);
            int normalIndex = LookupStyle(nameToIndex, "p");

            // Find the target OE by objectID.
            XElement targetOe = null;
            if (!string.IsNullOrEmpty(objectId))
            {
                targetOe = pageElement.Descendants(OneNs + "OE")
                    .FirstOrDefault(delegate(XElement oe)
                    {
                        return string.Equals((string)oe.Attribute("objectID"), objectId, StringComparison.Ordinal);
                    });
            }

            if (targetOe == null)
            {
                // OE not found — nothing to replace (page may have changed).
                Logger.Warn("ReplaceOeWithRenderedBlocks: OE not found, objectId=" + objectId);
                return;
            }

            // Build the new OE elements.
            List<XElement> newOes = BuildOeElements(blocks, nameToIndex, normalIndex, markdownSource);
            if (newOes.Count == 0) return;

            if (newOes.Count == 1)
            {
                // Single OE output: modify the target in-place, preserving its
                // objectID so OneNote keeps the cursor on this OE instead of
                // jumping to the previous line (Ctrl+Enter) or fresh line (Enter).
                string preservedId = (string)targetOe.Attribute("objectID");
                targetOe.RemoveNodes();
                targetOe.RemoveAttributes();
                foreach (XAttribute attr in newOes[0].Attributes()) targetOe.Add(attr);
                if (!string.IsNullOrEmpty(preservedId)) targetOe.Add(new XAttribute("objectID", preservedId));
                foreach (XNode child in newOes[0].Nodes()) targetOe.Add(child);
            }
            else
            {
                // Multi-OE output (code blocks, etc.): insert new OEs after the
                // target, delete target, then navigate cursor to the first new OE.
                XElement insertAfter = targetOe;
                foreach (XElement oe in newOes)
                {
                    insertAfter.AddAfterSelf(oe);
                    insertAfter = oe;
                }
                targetOe.Remove();
            }

            UpdatePage(pageDoc);
        }

        /// <summary>
        /// Replaces the content of an existing OE in-place with raw Markdown text
        /// (used by source-mode toggle to show the original Markdown for editing).
        /// </summary>
        public void ReplaceOeWithRawSource(string pageId, string objectId, string markdownSource)
        {
            ValidatePageId(pageId);
            if (string.IsNullOrEmpty(objectId)) return;

            XDocument pageDoc = GetPageDocument(pageId);
            XElement pageElement = pageDoc.Root;

            XElement targetOe = pageElement.Descendants(OneNs + "OE")
                .FirstOrDefault(delegate(XElement oe)
                {
                    return string.Equals((string)oe.Attribute("objectID"), objectId, StringComparison.Ordinal);
                });
            if (targetOe == null) return;

            // Clear existing T children and meta, write raw source as plain T.
            targetOe.Elements(OneNs + "T").Remove();
            targetOe.Elements(OneNs + "Meta").Remove();
            targetOe.RemoveAttributes(); // clear quickStyleIndex so it renders as normal paragraph
            targetOe.Add(new XElement(OneNs + "T", new XCData(SanitizeCData(WebUtility.HtmlEncode(markdownSource)))));

            UpdatePage(pageDoc);
        }

        private List<XElement> BuildOeElements(IList<MarkdownBlock> blocks, Dictionary<string, int> nameToIndex, int normalIndex, string markdownSource)
        {
            List<XElement> result = new List<XElement>();
            List<XElement> lastListItemByIndent = new List<XElement>();

            // Encode the Markdown source for storage in Meta.
            string mdBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(markdownSource ?? string.Empty));

            for (int i = 0; i < blocks.Count; i++)
            {
                MarkdownBlock block = blocks[i];
                if (block == null) continue;

                XElement oe = null;

                // Code blocks expand into multiple sibling OEs (one per line).
                if (block.Kind == MarkdownBlockKind.CodeBlock)
                {
                    int codeStyle = LookupStyle(nameToIndex, "code");
                    List<XElement> codeOes = CreateCodeOes(block, codeStyle);
                    for (int c = 0; c < codeOes.Count; c++)
                    {
                        XElement codeOe = codeOes[c];
                        if (result.Count == 0 && !string.IsNullOrEmpty(markdownSource))
                        {
                            codeOe.AddFirst(new XElement(OneNs + "Meta",
                                new XAttribute("name", "md-src"),
                                new XAttribute("content", mdBase64)));
                        }
                        result.Add(codeOe);
                    }
                    continue;
                }

                if (block.Kind == MarkdownBlockKind.ListItem)
                {
                    // For list items in single-line render, produce a flat OE with bullet character.
                    int indent = block.IndentLevel < 0 ? 0 : block.IndentLevel;
                    string marker = block.ListKind == MarkdownListKind.Ordered
                        ? block.OrderedNumber.ToString(CultureInfo.InvariantCulture) + ". "
                        : (block.ListKind == MarkdownListKind.Task
                            ? (block.IsTaskChecked ? "☑ " : "☐ ")
                            : "• ");
                    string listText = new string(' ', indent * 2) + marker + (block.Text ?? string.Empty);
                    oe = CreateStyledOe(listText, normalIndex, false, true);
                }
                else
                {
                    int styleIndex;
                    string text;
                    switch (block.Kind)
                    {
                        case MarkdownBlockKind.Heading:
                            int level = block.Level < 1 ? 1 : (block.Level > 6 ? 6 : block.Level);
                            styleIndex = LookupStyle(nameToIndex, "h" + level.ToString(CultureInfo.InvariantCulture));
                            text = block.Text ?? string.Empty;
                            oe = CreateStyledOe(text, styleIndex, false, true);
                            break;
                        case MarkdownBlockKind.Blank:
                            oe = CreateStyledOe(string.Empty, normalIndex);
                            break;
                        case MarkdownBlockKind.DiagramBlock:
                            styleIndex = LookupStyle(nameToIndex, "diagram");
                            text = BuildDiagramPlaceholder(block);
                            oe = CreateDiagramOe(block, styleIndex, text);
                            break;
                        case MarkdownBlockKind.LatexBlock:
                            styleIndex = LookupStyle(nameToIndex, "latex");
                            text = BuildLatexPlaceholder(block);
                            oe = CreateLatexOe(block, styleIndex, text);
                            break;
                        case MarkdownBlockKind.Toc:
                            styleIndex = LookupStyle(nameToIndex, "toc");
                            text = BuildTocPlaceholder(blocks);
                            oe = CreateStyledOe(text, styleIndex, false, false);
                            break;
                        case MarkdownBlockKind.Blockquote:
                            text = "❝ " + (block.Text ?? string.Empty);
                            oe = CreateStyledOe(text, normalIndex, false, true);
                            break;
                        case MarkdownBlockKind.HorizontalRule:
                            oe = CreateStyledOe(new string('─', 30), normalIndex, false, false);
                            break;
                        default:
                            text = block.Text ?? string.Empty;
                            oe = CreateStyledOe(text, normalIndex, false, true);
                            break;
                    }
                }

                if (oe == null) continue;

                // Attach Markdown source to the first OE only (the "anchor" element).
                // Subsequent OEs in a multi-block render are continuations and don't need their own source.
                if (result.Count == 0 && !string.IsNullOrEmpty(markdownSource))
                {
                    oe.AddFirst(new XElement(OneNs + "Meta",
                        new XAttribute("name", "md-src"),
                        new XAttribute("content", mdBase64)));
                }

                result.Add(oe);
            }

            return result;
        }

        private void ValidatePageId(string pageId)
        {
            if (string.IsNullOrWhiteSpace(pageId)) throw new ArgumentException("Page ID cannot be null or empty.", nameof(pageId));
        }
    }
}
