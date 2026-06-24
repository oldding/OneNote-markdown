using System;
using System.IO;
using OneNoteMarkdown.Logging;
using WpfMath;
using WpfMath.Parsers;
using XamlMath.Exceptions;

namespace OneNoteMarkdown.Rendering
{
    internal sealed class LatexImageRenderer
    {
        private const double RenderScale = 20.0;
        private static readonly object ParserLock = new object();

        public bool TryRenderToPng(string latex, string textFontFamily, out byte[] pngBytes, out int pixelWidth, out int pixelHeight)
        {
            pngBytes = null;
            pixelWidth = 0;
            pixelHeight = 0;

            string text = Normalize(latex);
            if (text.Length == 0) return false;

            string font = string.IsNullOrWhiteSpace(textFontFamily) ? "Cambria Math" : textFontFamily.Trim();

            try
            {
                XamlMath.TexFormula formula;
                lock (ParserLock)
                {
                    formula = WpfTeXFormulaParser.Instance.Parse(text, "text");
                }

                byte[] bytes = Extensions.RenderToPng(formula, RenderScale, 0.0, 0.0, font);
                if (bytes == null || bytes.Length == 0)
                {
                    return false;
                }

                int width;
                int height;
                if (!TryReadPngSize(bytes, out width, out height))
                {
                    return false;
                }

                pngBytes = bytes;
                pixelWidth = width;
                pixelHeight = height;
                return true;
            }
            catch (TexException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("LatexImageRenderer unexpected error", ex);
                return false;
            }
        }

        private static string Normalize(string latex)
        {
            string text = (latex ?? string.Empty).Trim();
            if (text.Length == 0)
            {
                return string.Empty;
            }

            if (text.StartsWith("$$", StringComparison.Ordinal) && text.EndsWith("$$", StringComparison.Ordinal) && text.Length > 4)
            {
                text = text.Substring(2, text.Length - 4).Trim();
            }

            return text.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        }

        private static bool TryReadPngSize(byte[] png, out int width, out int height)
        {
            width = 0;
            height = 0;
            if (png == null || png.Length < 24) return false;

            // PNG signature + first chunk should contain IHDR width/height.
            if (!(png[0] == 0x89 && png[1] == 0x50 && png[2] == 0x4E && png[3] == 0x47)) return false;
            if (!(png[12] == 0x49 && png[13] == 0x48 && png[14] == 0x44 && png[15] == 0x52)) return false;

            width = (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
            height = (png[20] << 24) | (png[21] << 16) | (png[22] << 8) | png[23];
            return width > 0 && height > 0;
        }
    }
}
