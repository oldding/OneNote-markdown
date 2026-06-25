using System;
using System.Globalization;
using System.Threading;
using OneNoteMarkdown.Logging;

namespace OneNoteMarkdown.Localization
{
    /// <summary>
    /// Manages the current UI language. Supports auto (follows system), zh, en.
    /// </summary>
    internal static class LocalizationManager
    {
        private static string _overrideLanguage = "auto";

        /// <summary>
        /// Gets the effective language code ("zh" or "en").
        /// </summary>
        public static string CurrentLanguage
        {
            get
            {
                if (_overrideLanguage == "auto" || string.IsNullOrEmpty(_overrideLanguage))
                {
                    string name = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    return name == "zh" ? "zh" : "en";
                }
                return _overrideLanguage;
            }
        }

        /// <summary>
        /// Gets the raw setting value ("auto", "zh", or "en").
        /// </summary>
        public static string OverrideSetting
        {
            get { return _overrideLanguage; }
        }

        /// <summary>
        /// Sets the language override. Pass "auto" to follow system, or "zh"/"en" for explicit.
        /// </summary>
        public static void SetLanguage(string lang)
        {
            _overrideLanguage = (lang ?? "auto").Trim().ToLowerInvariant();
            if (_overrideLanguage != "auto" && _overrideLanguage != "zh" && _overrideLanguage != "en")
            {
                _overrideLanguage = "auto";
            }

            if (_overrideLanguage != "auto")
            {
                try
                {
                    CultureInfo ci = _overrideLanguage == "zh"
                        ? new CultureInfo("zh-CN")
                        : new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
                catch (Exception ex)
                {
                    Logger.Error("SetLanguage culture failed", ex);
                }
            }

            Logger.Info("Language set to: " + _overrideLanguage + " (effective: " + CurrentLanguage + ")");
        }

        /// <summary>
        /// Initializes from settings. Call during OnConnection.
        /// </summary>
        public static void Initialize(string settingsLanguage)
        {
            SetLanguage(settingsLanguage ?? "auto");
        }
    }
}
