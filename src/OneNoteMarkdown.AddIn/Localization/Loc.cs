namespace OneNoteMarkdown.Localization
{
    /// <summary>
    /// All localizable UI strings. Access via Loc.S("key") which returns the
    /// string for the current language. Keys are organized by area.
    /// </summary>
    internal static partial class Loc
    {
        /// <summary>
        /// Returns the localized string for the given key.
        /// Falls back to key itself if not found.
        /// </summary>
        public static string S(string key)
        {
            string lang = LocalizationManager.CurrentLanguage;
            if (lang == "en")
            {
                string val;
                if (EnStrings.TryGetValue(key, out val)) return val;
            }
            {
                string val;
                if (ZhStrings.TryGetValue(key, out val)) return val;
            }
            return key;
        }

        /// <summary>
        /// Returns the localized string with one format argument.
        /// </summary>
        public static string S(string key, object arg0)
        {
            return string.Format(S(key), arg0);
        }
    }
}
