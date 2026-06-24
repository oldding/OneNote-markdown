using System;
using System.Windows.Forms;
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
                Msg.Show("打开帮助失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
