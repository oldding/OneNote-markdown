using System;
using System.Drawing;
using System.Windows.Forms;

namespace OneNoteMarkdown.UI
{
    internal sealed class FileDialogHost : Form
    {
        public FileDialogHost()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(-32000, -32000);
            Size = new Size(1, 1);
            Opacity = 0;
            TopMost = true;
        }

        public static DialogResult ShowOpen(OpenFileDialog dialog)
        {
            using (FileDialogHost host = new FileDialogHost())
            {
                host.Show(UiThread.Anchor);
                ForceForeground.Apply(host);
                DialogResult result = dialog.ShowDialog(host);
                host.Close();
                return result;
            }
        }

        public static DialogResult ShowSave(SaveFileDialog dialog)
        {
            using (FileDialogHost host = new FileDialogHost())
            {
                host.Show(UiThread.Anchor);
                ForceForeground.Apply(host);
                DialogResult result = dialog.ShowDialog(host);
                host.Close();
                return result;
            }
        }
    }
}
