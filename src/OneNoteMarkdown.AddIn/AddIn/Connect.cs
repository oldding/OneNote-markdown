using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Extensibility;
using Microsoft.Office.Core;
using OneNoteMarkdown.Features;
using OneNoteMarkdown.Logging;
using OneNoteMarkdown.UI;

namespace OneNoteMarkdown.AddIn
{
    [ComVisible(true)]
    [Guid("0A92B61B-98B8-4E5D-BE2D-48EDB01ED177")]
    [ProgId("OneNoteMarkdown.Connect")]
    public class Connect : IDTExtensibility2, IRibbonExtensibility
    {
        private const string EmptyRibbonXml = "<customUI xmlns=\"http://schemas.microsoft.com/office/2006/01/customui\"></customUI>";
        private const string RibbonResourceName = "OneNoteMarkdown.Ribbon.CustomUI.xml";

        private static Connect _instance;
        private static string _addInDirectory;
        private static uint _oneNoteUiThreadId;

        private object _oneNoteApp;

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        /// <summary>
        /// The Win32 thread id of OneNote's UI/editing thread, captured during
        /// OnConnection (which the host invokes on that thread). Used to install
        /// the thread-local keyboard hook on OneNote's own thread.
        /// </summary>
        public static uint OneNoteUiThreadId
        {
            get { return _oneNoteUiThreadId; }
        }

        static Connect()
        {
            _addInDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public Connect()
        {
            _instance = this;
            try
            {
                Logger.Initialize();
                Logger.Info("Connect ctor invoked");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Logger init failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Posts an action for execution on the UiThread (which has a proper
        /// WinForms message pump).  OneNote's COM server is out-of-process, so
        /// COM calls issued from any STA thread are automatically marshalled by
        /// COM — no need to be on OneNote's own thread.
        /// </summary>
        public static void PostToOneNoteThread(Action action)
        {
            UI.UiThread.Post(action);
        }

        public static Connect Instance
        {
            get { return _instance; }
        }

        public object OneNoteApp
        {
            get { return _oneNoteApp; }
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                AssemblyName assemblyName = new AssemblyName(args.Name);
                string candidatePath = Path.Combine(_addInDirectory, assemblyName.Name + ".dll");
                if (File.Exists(candidatePath))
                {
                    return Assembly.LoadFrom(candidatePath);
                }
            }
            catch
            {
            }

            return null;
        }

        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            try
            {
                _instance = this;
                _oneNoteApp = application;
                _oneNoteUiThreadId = GetCurrentThreadId();
                Logger.Initialize();
                Logger.Info("OneNoteMarkdown connected (" + connectMode + "), uiThread=" + _oneNoteUiThreadId);

                // Restore LoadBehavior=3 each time we connect successfully.
                // OneNote downgrades it to 2 after a crash; this self-heals on next successful load.
                try
                {
                    Microsoft.Win32.Registry.SetValue(
                        @"HKEY_CURRENT_USER\Software\Microsoft\Office\OneNote\AddIns\OneNoteMarkdown.Connect",
                        "LoadBehavior", 3, Microsoft.Win32.RegistryValueKind.DWord);
                }
                catch { /* non-fatal */ }

                // Start the UI pump thread early (used for COM marshalling and
                // for WM_HOTKEY delivery). Non-blocking.
                UiThread.EnsureStarted();

                // Enable live preview (WH_KEYBOARD_LL on a background pump thread).
                try
                {
                    if (!LivePreviewService.IsEnabled)
                    {
                        LivePreviewService.Toggle();
                        RibbonState.InvalidateRibbon();
                    }
                }
                catch (Exception exLp) { Logger.Error("Auto-enable live preview failed", exLp); }

                // Register global hotkeys asynchronously — the hidden window must
                // live on the UI pump thread (Application.Run) to receive WM_HOTKEY.
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    try { HotkeyRouter.EnsureRegistered(); }
                    catch (Exception exHk) { Logger.Error("HotkeyRouter async init failed", exHk); }
                });
            }
            catch (Exception ex)
            {
                try
                {
                    Logger.Error("OnConnection failed", ex);
                }
                catch
                {
                }
                // Never throw here; a startup exception can block OneNote launch.
            }
        }

        public void OnStartupComplete(ref Array custom)
        {
        }

        public void OnDisconnection(ext_DisconnectMode removeMode, ref Array custom)
        {
            try
            {
                // Tear down keyboard hook and hotkeys before releasing COM,
                // so nothing references OneNote afterwards.
                EnterHook.Uninstall();
                HotkeyRouter.Unregister();
                UiThread.Shutdown();
            }
            catch (Exception ex)
            {
                try { Logger.Error("OnDisconnection cleanup failed", ex); } catch { }
            }
            finally
            {
                // CRITICAL for clean shutdown: release the OneNote COM reference
                // and force RCW finalization. Without this the add-in keeps a
                // live reference to OneNote's COM server, so ONENOTE.EXE cannot
                // terminate — it lingers as a wedged process and the NEXT launch
                // hits OneNote's crash-recovery ("无法顺利启动") loop. The stable
                // base add-in (OneNote copilot) does exactly this.
                try
                {
                    object app = _oneNoteApp;
                    if (app != null && System.Runtime.InteropServices.Marshal.IsComObject(app))
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                    }
                }
                catch (Exception ex) { try { Logger.Error("ReleaseComObject failed", ex); } catch { } }

                _oneNoteApp = null;
                if (ReferenceEquals(_instance, this))
                {
                    _instance = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                try { Logger.Info("OneNoteMarkdown disconnected (" + removeMode + ")"); } catch { }
            }
        }

        public void OnAddInsUpdate(ref Array custom)
        {
        }

        public void OnBeginShutdown(ref Array custom)
        {
        }

        public string GetCustomUI(string ribbonId)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(RibbonResourceName))
                {
                    if (stream == null)
                    {
                        Logger.Warn("Ribbon resource not found: " + RibbonResourceName);
                        return EmptyRibbonXml;
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetCustomUI failed", ex);
                return EmptyRibbonXml;
            }
        }

        public void OnRibbonLoad(IRibbonUI ribbonUI)
        {
            RibbonState.RibbonUI = ribbonUI;
        }

        public void OnImportMarkdown(IRibbonControl control)
        {
            UiThread.Post(ImportMarkdownCommand.Execute);
        }

        public void OnExportMarkdown(IRibbonControl control)
        {
            UiThread.Post(ExportMarkdownCommand.Execute);
        }

        public void OnExportMarkdownClipboard(IRibbonControl control)
        {
            UiThread.Post(ExportMarkdownToClipboardCommand.Execute);
        }

        public void OnRenderSelection(IRibbonControl control)
        {
            UiThread.Post(RenderSelectionMarkdownCommand.Execute);
        }

        public void OnRenderPage(IRibbonControl control)
        {
            UiThread.Post(RenderPageMarkdownCommand.Execute);
        }

        public void OnSettings(IRibbonControl control)
        {
            UiThread.Post(OpenThemeSettingsCommand.Execute);
        }

        public void OnHelp(IRibbonControl control)
        {
            UiThread.Post(OpenHelpCommand.Execute);
        }

        public void OnToggleLivePreview(IRibbonControl control)
        {
            UiThread.Post(delegate
            {
                ToggleLivePreviewCommand.Execute();
                RibbonState.InvalidateRibbon();
            });
        }

        public bool GetLivePreviewPressed(IRibbonControl control)
        {
            return LivePreviewService.IsEnabled;
        }
    }
}
