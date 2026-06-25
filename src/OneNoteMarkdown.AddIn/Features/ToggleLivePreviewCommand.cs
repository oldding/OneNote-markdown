using System;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
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
                    Msg.Show(nowEnabled ? Loc.S("Msg.LiveOn") : Loc.S("Msg.LiveOff"),
                        Loc.S("Common.AppTitle"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Toggle live preview failed", ex);
                Msg.Show(Loc.S("Msg.LiveToggleFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
