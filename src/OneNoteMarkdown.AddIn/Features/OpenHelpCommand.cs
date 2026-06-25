using System;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class OpenHelpCommand
    {
        public static void Execute()
        {
            try
            {
                using (HelpWindow dlg = new HelpWindow())
                {
                    dlg.ShowDialog(UiThread.Anchor);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Open help failed", ex);
                Msg.Show(Loc.S("Msg.HelpFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
