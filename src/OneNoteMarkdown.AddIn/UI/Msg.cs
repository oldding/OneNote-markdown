using System.Windows.Forms;

namespace OneNoteMarkdown.UI
{
    public static class Msg
    {
        private const MessageBoxOptions ForegroundTopmost = (MessageBoxOptions)0x40000 | (MessageBoxOptions)0x10000;

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(UiThread.Anchor, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, ForegroundTopmost);
        }
    }
}
