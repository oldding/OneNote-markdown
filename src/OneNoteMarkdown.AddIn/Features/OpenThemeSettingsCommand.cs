using System;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class OpenThemeSettingsCommand
    {
        public static void Execute()
        {
            try
            {
                using (SettingsDialog dlg = new SettingsDialog())
                {
                    dlg.ShowDialog(UiThread.Anchor);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Open theme settings failed", ex);
                Msg.Show(Loc.S("Msg.SettingsFailed", ex.Message), Loc.S("Common.AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
