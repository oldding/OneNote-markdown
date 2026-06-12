using System;
using System.Linq;
using System.Text;
using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote;

namespace OneNoteMarkdown.Tests
{
    internal static class Program
    {
        private const string OneNs = "http://schemas.microsoft.com/office/onenote/2013/onenote";
        private static int _failures;

        private static int Main()
        {
            Run("Markdown renderer", TestMarkdownRenderer);
            Run("HTML line breaks", TestHtmlLineBreaks);
            Run("Markdown source export", TestMarkdownSourceExport);

            Console.WriteLine(_failures == 0
                ? "All regression tests passed."
                : _failures + " regression test(s) failed.");
            return _failures == 0 ? 0 : 1;
        }

        private static void TestMarkdownRenderer()
        {
            var blocks = MarkdownRenderer.RenderToBlocks("# Title\n- [x] done\n```csharp\nvar x = 1;\n```");
            Assert(blocks.Count == 3, "Expected three blocks.");
            Assert(blocks[0].Kind == MarkdownBlockKind.Heading && blocks[0].Level == 1, "Heading was not parsed.");
            Assert(blocks[1].Kind == MarkdownBlockKind.ListItem && blocks[1].IsTaskChecked, "Task item was not parsed.");
            Assert(blocks[2].Kind == MarkdownBlockKind.CodeBlock && blocks[2].CodeLanguage == "csharp", "Code fence was not parsed.");
        }

        private static void TestHtmlLineBreaks()
        {
            string xml = PageXml(
                "<one:OE objectID=\"oe1\"><one:T><![CDATA[first<br>second]]></one:T></one:OE>");
            var page = PageParser.Parse(xml);
            Assert(page.Outlines[0].TextBlocks[0].Text == "first\nsecond", "HTML break was lost.");
        }

        private static void TestMarkdownSourceExport()
        {
            string source = "```csharp\nvar x = 1;\n```";
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
            string xml = PageXml(
                "<one:OE objectID=\"oe1\">" +
                "<one:Meta name=\"md-src\" content=\"" + encoded + "\"/>" +
                "<one:T><![CDATA[var x = 1;]]></one:T></one:OE>" +
                "<one:OE objectID=\"oe2\">" +
                "<one:Meta name=\"md-continuation\" content=\"true\"/>" +
                "<one:T><![CDATA[duplicate continuation]]></one:T></one:OE>");

            var page = PageParser.Parse(xml);
            string markdown = MarkdownExporter.Export(page);
            Assert(markdown.Contains(source), "Stored Markdown source was not restored.");
            Assert(!markdown.Contains("duplicate continuation"), "Continuation text was exported twice.");
        }

        private static string PageXml(string oeXml)
        {
            return "<one:Page xmlns:one=\"" + OneNs + "\" ID=\"page1\" name=\"Test\">" +
                "<one:Outline><one:OEChildren>" + oeXml +
                "</one:OEChildren></one:Outline></one:Page>";
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception ex)
            {
                _failures++;
                Console.WriteLine("FAIL " + name + ": " + ex.Message);
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
