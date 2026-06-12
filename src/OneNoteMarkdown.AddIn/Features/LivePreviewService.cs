using System;
using OneNoteMarkdown.Logging;

namespace OneNoteMarkdown.Features
{
    /// <summary>
    /// Controls live-preview mode. When enabled, the global keyboard hook
    /// (EnterHook) fires RenderCurrentLineCommand for every line the user commits.
    /// </summary>
    internal static class LivePreviewService
    {
        private static readonly object Gate = new object();
        private static bool _enabled;

        public static bool IsEnabled
        {
            get { lock (Gate) { return _enabled; } }
        }

        public static void Toggle()
        {
            bool nowEnabled;
            lock (Gate)
            {
                _enabled = !_enabled;
                nowEnabled = _enabled;
            }

            // Install/uninstall the hook OUTSIDE the lock — Uninstall() may block
            // briefly joining the pump thread, and we must not hold Gate meanwhile.
            if (nowEnabled)
            {
                EnterHook.Install();
                Logger.Info("Live preview started");
            }
            else
            {
                EnterHook.Uninstall();
                Logger.Info("Live preview stopped");
            }
        }

        public static void ForceRefresh()
        {
            // Force-render the current line on demand (e.g. Ribbon button).
            try { RenderCurrentLineCommand.Execute(); }
            catch (Exception ex) { Logger.Error("LivePreview ForceRefresh failed", ex); }
        }
    }
}
