using System;
using System.Windows.Forms;
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
                Msg.Show("打开主题配置失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
