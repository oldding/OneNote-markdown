using OneNoteMarkdown.Markdown;
using OneNoteMarkdown.OneNote;

namespace OneNoteMarkdown.Features
{
    internal static class MarkdownExportService
    {
        public static string ExportCurrentPageMarkdown(out string pageTitle)
        {
            OneNoteProvider provider = new OneNoteProvider();
            var page = provider.GetCurrentPage();
            pageTitle = page == null ? string.Empty : (page.Title ?? string.Empty);
            return MarkdownExporter.Export(page);
        }
    }
}
