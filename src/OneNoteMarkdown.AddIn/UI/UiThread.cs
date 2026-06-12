using System;
using System.Threading;
using System.Windows.Forms;
using OneNoteMarkdown.Logging;

namespace OneNoteMarkdown.UI
{
    public static class UiThread
    {
        private static Thread _thread;
        private static Form _pumpForm;
        private static volatile SynchronizationContext _syncContext;
        private static readonly object _gate = new object();
        private static ManualResetEventSlim _ready;

        /// <summary>
        /// Start the background STA pump thread WITHOUT blocking the caller.
        /// Safe to call from OnConnection (OneNote's STA thread).
        /// </summary>
        public static void EnsureStarted()
        {
            lock (_gate)
            {
                if (_thread != null && _thread.IsAlive) return;

                _ready = new ManualResetEventSlim(false);
                _thread = new Thread(ThreadProc)
                {
                    IsBackground = true,
                    Name = "OneNoteMarkdown.UiThread"
                };
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();
                // Do NOT Wait() here — blocking OneNote's STA thread causes crashes.
            }
        }

        private static void ThreadProc()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _pumpForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar   = false,
                StartPosition   = FormStartPosition.Manual,
                Location        = new System.Drawing.Point(-32000, -32000),
                Size            = new System.Drawing.Size(1, 1),
                Opacity         = 0,
                Visible         = false
            };

            _pumpForm.Load += delegate
            {
                _pumpForm.Visible = false;
                _syncContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
                _ready.Set();
            };

            IntPtr handle = _pumpForm.Handle;
            GC.KeepAlive(handle);
            if (_syncContext == null)
            {
                _syncContext = new WindowsFormsSynchronizationContext();
                _ready.Set();
            }

            Application.Run(_pumpForm);
        }

        /// <summary>
        /// Post an action to the UiThread.  Blocks the caller briefly (up to
        /// 5 s) only when the thread is still initialising — this never happens
        /// on OneNote's STA thread because EnsureStarted() is called well before
        /// the first Post().
        /// </summary>
        public static void Post(Action action)
        {
            if (action == null) return;
            EnsureStarted();
            // Wait until the pump is ready (happens in milliseconds normally).
            ManualResetEventSlim ready = _ready;
            if (ready != null) ready.Wait(5000);
            SynchronizationContext ctx = _syncContext;
            if (ctx == null)
            {
                // Pump failed to initialise; run inline as a last resort.
                try { action(); } catch { }
                return;
            }
            ctx.Post(_ => action(), null);
        }

        public static IWin32Window Anchor
        {
            get
            {
                EnsureStarted();
                ManualResetEventSlim ready = _ready;
                if (ready != null) ready.Wait(5000);
                return _pumpForm;
            }
        }

        /// <summary>
        /// Signals the pump thread to exit and waits for it to finish.
        /// Called during add-in shutdown (OnDisconnection) so the background
        /// thread does not prevent the process from exiting cleanly.
        /// </summary>
        public static void Shutdown()
        {
            lock (_gate)
            {
                if (_pumpForm != null && !_pumpForm.IsDisposed)
                {
                    try { _pumpForm.Invoke((Action)(() => _pumpForm.Close())); }
                    catch { }
                }

                if (_thread != null && _thread.IsAlive)
                {
                    try { _thread.Join(3000); }
                    catch { }
                }

                _thread = null;
                _pumpForm = null;
                _syncContext = null;
                Logger.Info("UiThread shut down");
            }
        }
    }
}
