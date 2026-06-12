using System;
using System.Windows.Forms;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class ToggleLivePreviewCommand
    {
        public static void Execute()
        {
            try
            {
                bool wasEnabled = LivePreviewService.IsEnabled;
                LivePreviewService.Toggle();
                bool nowEnabled = LivePreviewService.IsEnabled;
                Logger.Info("ToggleLivePreviewCommand: " + (nowEnabled ? "on" : "off"));

                if (wasEnabled != nowEnabled)
                {
                    Msg.Show(nowEnabled ? "实时模式已开启。" : "实时模式已关闭。",
                        "OneNote Markdown",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Toggle live preview failed", ex);
                Msg.Show("切换实时模式失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
