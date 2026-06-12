using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using OneNoteMarkdown.AddIn;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.Features
{
    /// <summary>
    /// Registers system-wide hotkeys using RegisterHotKey / WM_HOTKEY.
    ///
    /// The hidden NativeWindow MUST live on a thread that runs a real WinForms
    /// message pump (Application.Run), otherwise WM_HOTKEY messages are never
    /// dispatched to WndProc.  We create it on UiThread, which already runs
    /// Application.Run, so we get reliable delivery.
    ///
    /// Command execution is posted back to the same UiThread (which is also the
    /// thread that owns the OneNote COM objects via SynchronizationContext).
    /// </summary>
    internal static class HotkeyRouter
    {
        private const int WM_HOTKEY  = 0x0312;
        private const int MOD_CONTROL = 0x0002;

        private const int IdRenderPage      = 1001; // F5
        private const int IdExportClipboard = 1002; // F8
        private const int IdToggleLive      = 1003; // Ctrl+\
        private const int IdRenderLine      = 1004; // Ctrl+Enter

        private static HotkeyWindow _window;
        private static readonly object _gate = new object();

        public static void EnsureRegistered()
        {
            lock (_gate)
            {
                if (_window != null) return;

                // Create the window on UiThread so its WndProc runs inside
                // Application.Run and actually receives WM_HOTKEY messages.
                UiThread.Post(() =>
                {
                    try
                    {
                        _window = new HotkeyWindow();

                        bool ok1 = RegisterHotKey(_window.Handle, IdRenderPage,      0,           (int)Keys.F5);
                        bool ok2 = RegisterHotKey(_window.Handle, IdExportClipboard, 0,           (int)Keys.F8);
                        bool ok3 = RegisterHotKey(_window.Handle, IdToggleLive,      MOD_CONTROL, (int)Keys.Oem5);
                        bool ok4 = RegisterHotKey(_window.Handle, IdRenderLine,      MOD_CONTROL, (int)Keys.Return);

                        Logger.Info("HotkeyRouter register: F5=" + ok1 + ", F8=" + ok2
                            + ", Ctrl\\=" + ok3 + ", CtrlEnter=" + ok4);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("HotkeyRouter.EnsureRegistered failed", ex);
                    }
                });
            }
        }

        public static void Unregister()
        {
            lock (_gate)
            {
                if (_window == null) return;

                HotkeyWindow w = _window;
                _window = null;

                UiThread.Post(() =>
                {
                    try
                    {
                        UnregisterHotKey(w.Handle, IdRenderPage);
                        UnregisterHotKey(w.Handle, IdExportClipboard);
                        UnregisterHotKey(w.Handle, IdToggleLive);
                        UnregisterHotKey(w.Handle, IdRenderLine);
                        w.Dispose();
                        Logger.Info("HotkeyRouter unregistered");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("HotkeyRouter.Unregister failed", ex);
                    }
                });
            }
        }

        private static void OnHotkey(int id)
        {
            // Post to OneNote's STA thread — the only thread where COM calls are safe.
            AddIn.Connect.PostToOneNoteThread(() =>
            {
                try
                {
                    switch (id)
                    {
                        case IdRenderPage:      RenderPageMarkdownCommand.Execute();       break;
                        case IdExportClipboard: ExportMarkdownToClipboardCommand.Execute(); break;
                        case IdToggleLive:      ToggleLivePreviewCommand.Execute();        break;
                        case IdRenderLine:      RenderCurrentLineCommand.Execute();        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Hotkey action failed id=" + id, ex);
                }
            });
        }

        private sealed class HotkeyWindow : NativeWindow, IDisposable
        {
            public HotkeyWindow()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    OnHotkey(m.WParam.ToInt32());
                }
                base.WndProc(ref m);
            }

            public void Dispose()
            {
                DestroyHandle();
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
