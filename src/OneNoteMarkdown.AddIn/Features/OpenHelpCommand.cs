using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
                string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string[] searchPaths = new string[]
                {
                    Path.Combine(assemblyDir ?? string.Empty, "HELP.md"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "HELP.md"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OneNoteMarkdown", "HELP.md"),
                };

                string helpPath = null;
                foreach (string path in searchPaths)
                {
                    if (File.Exists(path))
                    {
                        helpPath = path;
                        break;
                    }
                }

                if (helpPath == null)
                {
                    Msg.Show("未找到帮助文件。", "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = "\"" + helpPath.Replace("\"", "") + "\"",
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Logger.Error("Open help failed", ex);
                Msg.Show("打开帮助失败：" + ex.Message, "OneNote Markdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
