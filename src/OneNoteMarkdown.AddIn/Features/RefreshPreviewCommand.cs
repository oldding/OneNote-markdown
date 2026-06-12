using System;
using OneNoteMarkdown.Logging;

namespace OneNoteMarkdown.Features
{
    public static class RefreshPreviewCommand
    {
        public static void Execute()
        {
            try
            {
                if (LivePreviewService.IsEnabled)
                {
                    LivePreviewService.ForceRefresh();
                }
                else
                {
                    RenderPageMarkdownCommand.Execute();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Refresh preview failed", ex);
            }
        }
    }
}
