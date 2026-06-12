using System;
using System.Diagnostics;
using System.Windows.Forms;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.Settings;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    public static class OpenThemeSettingsCommand
    {
        public static void Execute()
        {
            try
            {
                string path = ThemeSettings.EnsureDefaultFile();
                if (string.IsNullOrWhiteSpace(path))
                {
                    Msg.Show("无法定位主题配置文件。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process.Start("notepad.exe", "\"" + path + "\"");
            }
            catch (Exception ex)
            {
                Logger.Error("Open theme settings failed", ex);
                Msg.Show("打开主题配置失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
