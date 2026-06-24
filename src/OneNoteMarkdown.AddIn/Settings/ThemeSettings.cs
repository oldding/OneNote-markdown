using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OneNoteMarkdown.Logging;

namespace OneNoteMarkdown.Settings
{
    internal sealed class ThemeSettings
    {
        private static readonly string SettingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OneNoteMarkdown",
            "settings");
        private static readonly string SettingsPath = Path.Combine(SettingsDir, "theme.ini");

        public string DefaultFontFamily { get; private set; }
        public string MonospaceFontFamily { get; private set; }
        public string MathFontFamily { get; private set; }
        public double ParagraphFontSize { get; private set; }
        public double CodeFontSize { get; private set; }
        public bool EnableLatexToImage { get; private set; }
        public bool EnableCodeLineNumber { get; private set; }

        private ThemeSettings()
        {
            DefaultFontFamily = "Calibri";
            MonospaceFontFamily = "Consolas";
            MathFontFamily = "Cambria Math";
            ParagraphFontSize = 11.0;
            CodeFontSize = 10.0;
            EnableLatexToImage = true;
            EnableCodeLineNumber = false;
        }

        public static ThemeSettings Load()
        {
            ThemeSettings s = new ThemeSettings();
            EnsureDefaultFile();
            if (!File.Exists(SettingsPath)) return s;

            string[] lines;
            try { lines = File.ReadAllLines(SettingsPath); }
            catch { return s; }

            Dictionary<string, string> kv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = (lines[i] ?? string.Empty).Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";")) continue;
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                string key = line.Substring(0, eq).Trim();
                string val = line.Substring(eq + 1).Trim();
                if (key.Length == 0) continue;
                kv[key] = val;
            }

            string value;
            if (kv.TryGetValue("font.family", out value) && !string.IsNullOrWhiteSpace(value)) s.DefaultFontFamily = value;
            if (kv.TryGetValue("font.monospace", out value) && !string.IsNullOrWhiteSpace(value)) s.MonospaceFontFamily = value;
            if (kv.TryGetValue("font.math", out value) && !string.IsNullOrWhiteSpace(value)) s.MathFontFamily = value;
            if (kv.TryGetValue("font.size.paragraph", out value)) s.ParagraphFontSize = ParseDouble(value, s.ParagraphFontSize);
            if (kv.TryGetValue("font.size.code", out value)) s.CodeFontSize = ParseDouble(value, s.CodeFontSize);
            if (kv.TryGetValue("enable.latex.image", out value)) s.EnableLatexToImage = ParseBool(value, s.EnableLatexToImage);
            if (kv.TryGetValue("enable.code.lineNumber", out value)) s.EnableCodeLineNumber = ParseBool(value, s.EnableCodeLineNumber);

            return s;
        }

        public static string EnsureDefaultFile()
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);
                if (!File.Exists(SettingsPath))
                {
                    File.WriteAllText(SettingsPath,
                        "# OneNote Markdown theme settings\r\n" +
                        "# Edit values and re-render page to apply\r\n" +
                        "font.family=Calibri\r\n" +
                        "font.monospace=Consolas\r\n" +
                        "font.math=Cambria Math\r\n" +
                        "font.size.paragraph=11\r\n" +
                        "font.size.code=10\r\n" +
                        "enable.latex.image=true\r\n" +
                        "enable.code.lineNumber=false\r\n");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ThemeSettings: failed to create default file", ex);
            }
            return SettingsPath;
        }

        private static double ParseDouble(string value, double fallback)
        {
            double parsed;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed) && parsed > 0.0)
            {
                return parsed;
            }
            return fallback;
        }

        private static bool ParseBool(string value, bool fallback)
        {
            if (string.IsNullOrWhiteSpace(value)) return fallback;
            bool parsed;
            if (bool.TryParse(value, out parsed)) return parsed;
            string v = value.Trim();
            if (string.Equals(v, "1", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(v, "0", StringComparison.OrdinalIgnoreCase)) return false;
            if (string.Equals(v, "yes", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(v, "no", StringComparison.OrdinalIgnoreCase)) return false;
            return fallback;
        }
    }
}
