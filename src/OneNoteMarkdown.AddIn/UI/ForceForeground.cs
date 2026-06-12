using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneNoteMarkdown.UI
{
    internal static class ForceForeground
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOW = 5;

        public static void Apply(Form form)
        {
            if (form == null || form.IsDisposed)
            {
                return;
            }

            try
            {
                IntPtr hWnd = form.Handle;
                ShowWindow(hWnd, SW_SHOW);
                BringWindowToTop(hWnd);

                IntPtr fgWnd = GetForegroundWindow();
                if (fgWnd == IntPtr.Zero || fgWnd == hWnd)
                {
                    SetForegroundWindow(hWnd);
                    form.Activate();
                    return;
                }

                uint fgThread = GetWindowThreadProcessId(fgWnd, out _);
                uint ourThread = GetCurrentThreadId();
                if (fgThread == ourThread)
                {
                    SetForegroundWindow(hWnd);
                    form.Activate();
                    return;
                }

                if (AttachThreadInput(ourThread, fgThread, true))
                {
                    try
                    {
                        BringWindowToTop(hWnd);
                        SetForegroundWindow(hWnd);
                        form.Activate();
                    }
                    finally
                    {
                        AttachThreadInput(ourThread, fgThread, false);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
