using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;
using OneNoteMarkdown.OneNote.Interop;
using OneNoteMarkdown.OneNote.Models;

namespace OneNoteMarkdown.OneNote
{
    public class OneNoteProvider
    {
        private static readonly XNamespace OneNs = "http://schemas.microsoft.com/office/onenote/2013/onenote";
        private static readonly Regex BreakRegex = new Regex("<br\\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BlockCloseRegex = new Regex("</(p|div|li|h[1-6])>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TagRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
        private readonly IOneNoteApplication _app;

        public OneNoteProvider()
        {
            object raw;
            try { raw = System.Runtime.InteropServices.Marshal.GetActiveObject("OneNote.Application"); }
            catch { raw = AddIn.Connect.Instance?.OneNoteApp; }
            if (raw == null) throw new InvalidOperationException("OneNote application not available.");
            _app = raw as IOneNoteApplication ?? throw new InvalidOperationException("Cast to IOneNoteApplication failed.");
        }

        public PageContent GetCurrentPage()
        {
            string pageId = GetCurrentPageId();
            if (string.IsNullOrWhiteSpace(pageId)) throw new InvalidOperationException("Current page is not available.");
            return GetPage(pageId);
        }

        public string GetCurrentPageId()
        {
            Windows windows = _app.GetWindows();
            if (windows == null) return string.Empty;
            Window currentWindow = windows.CurrentWindow;
            if (currentWindow == null) return string.Empty;
            return currentWindow.CurrentPageId ?? string.Empty;
        }

        public PageContent GetPage(string pageId)
        {
            if (string.IsNullOrWhiteSpace(pageId)) throw new ArgumentException("Page ID cannot be null or empty.", nameof(pageId));
            return PageParser.Parse(GetPageXml(pageId));
        }

        public string GetPageXml(string pageId)
        {
            if (string.IsNullOrWhiteSpace(pageId)) throw new ArgumentException("Page ID cannot be null or empty.", nameof(pageId));
            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piBasic, XMLSchema.xs2013);
            return xml;
        }

        public string GetCurrentSelectionText()
        {
            string pageId = GetCurrentPageId();
            if (string.IsNullOrWhiteSpace(pageId)) return string.Empty;

            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piSelection, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) return string.Empty;

            XDocument doc;
            try { doc = XDocument.Parse(xml); }
            catch { return string.Empty; }

            List<string> texts = new List<string>();
            bool hasSelectedNode = doc.Descendants().Any(delegate(XElement e)
            {
                string sel = (string)e.Attribute("selected");
                return string.Equals(sel, "all", StringComparison.OrdinalIgnoreCase);
            });

            foreach (XElement t in doc.Descendants(OneNs + "T"))
            {
                if (hasSelectedNode && !IsInsideSelectedSubtree(t))
                {
                    continue;
                }
                string plain = HtmlToPlainText(t.Value, true);
                if (!string.IsNullOrWhiteSpace(plain)) texts.Add(plain);
            }

            return string.Join("\n", texts).Trim();
        }

        public string GetCurrentPageTextForRender()
        {
            string pageId = GetCurrentPageId();
            if (string.IsNullOrWhiteSpace(pageId)) return string.Empty;

            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) return string.Empty;

            XDocument doc;
            try { doc = XDocument.Parse(xml); }
            catch { return string.Empty; }

            List<string> texts = new List<string>();
            List<XElement> outlines = doc.Root == null
                ? new List<XElement>()
                : doc.Root.Elements(OneNs + "Outline").ToList();
            for (int oi = 0; oi < outlines.Count; oi++)
            {
                XElement outline = outlines[oi];
                if (IsManagedOutline(outline)) continue;
                foreach (XElement t in outline.Descendants(OneNs + "T"))
                {
                    string plain = HtmlToPlainText(t.Value, true);
                    if (!string.IsNullOrWhiteSpace(plain)) texts.Add(plain);
                }
            }

            return string.Join("\n\n", texts).Trim();
        }

        public string GetManagedOutlineText(string pageId, string role)
        {
            if (string.IsNullOrWhiteSpace(pageId) || string.IsNullOrWhiteSpace(role)) return string.Empty;

            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) return string.Empty;

            XDocument doc;
            try { doc = XDocument.Parse(xml); }
            catch { return string.Empty; }

            string heading = ResolveManagedHeading(role);
            if (string.IsNullOrWhiteSpace(heading)) return string.Empty;

            XElement outline = doc.Root == null
                ? null
                : doc.Root.Elements(OneNs + "Outline").FirstOrDefault(delegate(XElement o)
                {
                    XElement firstT = o.Descendants(OneNs + "T").FirstOrDefault();
                    if (firstT == null) return false;
                    string plain = HtmlToPlainText(firstT.Value, false);
                    return string.Equals((plain ?? string.Empty).Trim(), heading, StringComparison.Ordinal);
                });
            if (outline == null) return string.Empty;

            List<string> lines = new List<string>();
            List<XElement> nodes = outline.Descendants(OneNs + "T").ToList();
            for (int i = 1; i < nodes.Count; i++)
            {
                string plain = HtmlToPlainText(nodes[i].Value, true);
                if (!string.IsNullOrWhiteSpace(plain)) lines.Add(plain);
            }

            return string.Join("\n", lines).Trim();
        }

        private static bool IsInsideSelectedSubtree(XElement node)
        {
            for (XElement cur = node; cur != null; cur = cur.Parent)
            {
                string sel = (string)cur.Attribute("selected");
                if (string.Equals(sel, "all", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsManagedOutline(XElement outline)
        {
            if (outline == null) return false;
            XElement firstT = outline.Descendants(OneNs + "T").FirstOrDefault();
            if (firstT == null) return false;
            string plain = HtmlToPlainText(firstT.Value, false);
            if (string.IsNullOrWhiteSpace(plain)) return false;
            string text = plain.Trim();
            return string.Equals(text, "Markdown 源码（实时）", StringComparison.Ordinal)
                || string.Equals(text, "Markdown 预览（实时）", StringComparison.Ordinal);
        }

        private static string ResolveManagedHeading(string role)
        {
            if (string.Equals(role, "LiveSource", StringComparison.OrdinalIgnoreCase)) return "Markdown 源码（实时）";
            if (string.Equals(role, "LivePreview", StringComparison.OrdinalIgnoreCase)) return "Markdown 预览（实时）";
            return string.Empty;
        }

        /// <summary>
        /// Returns info about the OE (Outline Element) that currently contains the cursor.
        /// Uses piSelection to find the selected/active OE on the current page.
        /// Returns null if no active OE is found or the page cannot be read.
        /// </summary>
        public OeInfo GetCurrentOeInfo()
        {
            string pageId = GetCurrentPageId();
            if (string.IsNullOrWhiteSpace(pageId)) return null;

            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piSelection, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) return null;

            XDocument doc;
            try { doc = XDocument.Parse(xml); }
            catch { return null; }

            // Find the OE that is selected or contains selected text.
            // OneNote marks the active OE (or its T children) with selected="all" or selected="partial".
            XElement activeOe = null;

            foreach (XElement oe in doc.Descendants(OneNs + "OE"))
            {
                // Check if this OE or any of its T children are selected.
                bool selected = false;
                string oeSel = (string)oe.Attribute("selected");
                if (string.Equals(oeSel, "all", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(oeSel, "partial", StringComparison.OrdinalIgnoreCase))
                {
                    selected = true;
                }
                if (!selected)
                {
                    foreach (XElement t in oe.Elements(OneNs + "T"))
                    {
                        string tSel = (string)t.Attribute("selected");
                        if (string.Equals(tSel, "all", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(tSel, "partial", StringComparison.OrdinalIgnoreCase))
                        {
                            selected = true;
                            break;
                        }
                    }
                }
                if (selected)
                {
                    activeOe = oe;
                    break;
                }
            }

            if (activeOe == null) return null;

            string objectId = (string)activeOe.Attribute("objectID");
            string mdSrc = ExtractOeMarkdown(activeOe);

            // When the user presses Enter at the end of a line, OneNote first
            // inserts a NEW empty OE and moves the cursor into it, so piSelection
            // reports that empty OE as active. In that case the line we actually
            // want to render is the PREVIOUS sibling OE (the one just committed).
            // Resolve it against the full page XML, since piSelection may not
            // include sibling content.
            if (string.IsNullOrWhiteSpace(mdSrc) && !string.IsNullOrWhiteSpace(objectId))
            {
                XElement prevOe = FindPreviousSiblingOe(pageId, objectId);
                if (prevOe != null)
                {
                    // Skip if the previous OE is already rendered (has md-src Meta);
                    // re-rendering it would be redundant and could disturb the cursor.
                    bool alreadyRendered = prevOe.Elements(OneNs + "Meta")
                        .Any(delegate(XElement m)
                        {
                            return string.Equals((string)m.Attribute("name"), "md-src", StringComparison.Ordinal);
                        });
                    string prevSrc = alreadyRendered ? null : ExtractOeMarkdown(prevOe);
                    if (!string.IsNullOrWhiteSpace(prevSrc) && LooksLikeMarkdown(prevSrc))
                    {
                        return new OeInfo
                        {
                            PageId = pageId,
                            ObjectId = (string)prevOe.Attribute("objectID") ?? string.Empty,
                            MarkdownSource = prevSrc
                        };
                    }
                }
            }

            return new OeInfo
            {
                PageId   = pageId,
                ObjectId = objectId ?? string.Empty,
                MarkdownSource = mdSrc ?? string.Empty
            };
        }

        /// <summary>
        /// Heuristic: returns true if the line contains Markdown syntax worth
        /// rendering on Enter. Plain prose (no markup) returns false so that
        /// ordinary line breaks are left untouched.
        /// </summary>
        private static bool LooksLikeMarkdown(string src)
        {
            if (string.IsNullOrWhiteSpace(src)) return false;
            string t = src.TrimStart();

            // Block-level markers.
            if (t.StartsWith("#")) return true;                 // headings
            if (Regex.IsMatch(t, @"^[-+*]\s+\S")) return true;   // - + * list
            if (Regex.IsMatch(t, @"^\d+[.)]\s+\S")) return true; // 1. ordered list
            if (Regex.IsMatch(t, @"^\[( |x|X)\]\s+\S")) return true; // [ ] task
            if (t.StartsWith(">")) return true;                  // blockquote
            if (t.StartsWith("```") || t.StartsWith("~~~")) return true; // code fence
            if (t == "$$" || t.StartsWith("$$")) return true;    // latex block
            if (t.StartsWith("---") || t.StartsWith("***") || t.StartsWith("___")) return true; // hr
            if (t.StartsWith("|")) return true;                  // table row

            // Inline emphasis / code / link / image.
            if (Regex.IsMatch(src, @"\*\*[^*]+\*\*")) return true;     // bold
            if (Regex.IsMatch(src, @"(?<!\*)\*[^*]+\*(?!\*)")) return true; // italic
            if (Regex.IsMatch(src, @"__[^_]+__")) return true;          // bold
            if (Regex.IsMatch(src, @"~~[^~]+~~")) return true;          // strikethrough
            if (Regex.IsMatch(src, @"==[^=]+==")) return true;          // highlight
            if (Regex.IsMatch(src, @"\+\+[^+]+\+\+")) return true;      // underline
            if (Regex.IsMatch(src, @"`[^`]+`")) return true;            // inline code
            if (Regex.IsMatch(src, @"\[[^\]]+\]\([^)]+\)")) return true; // link/image

            return false;
        }

        /// <summary>
        /// Reads the Markdown source of an OE: first the md-src Meta tag, then the
        /// visible text as fallback.
        /// </summary>
        private static string ExtractOeMarkdown(XElement oe)
        {
            if (oe == null) return string.Empty;

            XElement meta = oe.Elements(OneNs + "Meta")
                .FirstOrDefault(delegate(XElement m)
                {
                    return string.Equals((string)m.Attribute("name"), "md-src", StringComparison.Ordinal);
                });
            if (meta != null)
            {
                string encoded = (string)meta.Attribute("content");
                if (!string.IsNullOrEmpty(encoded))
                {
                    try { return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)); }
                    catch { /* fall through to plain text */ }
                }
            }

            List<string> parts = new List<string>();
            foreach (XElement t in oe.Descendants(OneNs + "T"))
            {
                string plain = HtmlToPlainText(t.Value, true);
                if (plain.Length > 0) parts.Add(plain);
            }
            return string.Join("", parts).Trim();
        }

        /// <summary>
        /// Locates the OE identified by objectId in the full page XML and returns
        /// its immediately preceding sibling OE (skipping non-OE elements such as
        /// Meta). Returns null if not found or there is no previous sibling.
        /// </summary>
        private XElement FindPreviousSiblingOe(string pageId, string objectId)
        {
            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) return null;

            XDocument doc;
            try { doc = XDocument.Parse(xml); }
            catch { return null; }

            XElement target = doc.Descendants(OneNs + "OE")
                .FirstOrDefault(delegate(XElement e)
                {
                    return string.Equals((string)e.Attribute("objectID"), objectId, StringComparison.Ordinal);
                });
            if (target == null) return null;

            XElement prev = null;
            for (XElement sib = target.PreviousNode as XElement; sib != null; sib = sib.PreviousNode as XElement)
            {
                if (sib.Name == OneNs + "OE") { prev = sib; break; }
            }
            return prev;
        }

        /// <summary>
        /// Returns true if the OE identified by objectId has a &lt;one:Meta name="md-src"&gt; element,
        /// indicating it is in rendered (not raw source) state.
        /// </summary>
        public bool CurrentOeHasMdMeta(string pageId, string objectId)
        {
            if (string.IsNullOrWhiteSpace(pageId) || string.IsNullOrWhiteSpace(objectId)) return false;
            string xml;
            _app.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            if (string.IsNullOrWhiteSpace(xml)) return false;
            XDocument doc;
            try { doc = XDocument.Parse(xml); }
            catch { return false; }
            XElement oe = doc.Descendants(OneNs + "OE")
                .FirstOrDefault(delegate(XElement e)
                {
                    return string.Equals((string)e.Attribute("objectID"), objectId, StringComparison.Ordinal);
                });
            if (oe == null) return false;
            return oe.Elements(OneNs + "Meta")
                .Any(delegate(XElement m)
                {
                    return string.Equals((string)m.Attribute("name"), "md-src", StringComparison.Ordinal);
                });
        }

        private static string HtmlToPlainText(string html, bool preserveBreaks)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            string value = html;
            if (preserveBreaks)
            {
                value = BreakRegex.Replace(value, "\n");
                value = BlockCloseRegex.Replace(value, "\n");
            }
            value = TagRegex.Replace(value, string.Empty);
            value = WebUtility.HtmlDecode(value ?? string.Empty);
            value = value.Replace("\r\n", "\n").Replace('\r', '\n');
            return value.Trim();
        }
    }
}
