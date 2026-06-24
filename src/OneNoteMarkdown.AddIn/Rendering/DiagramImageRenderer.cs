using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OneNoteMarkdown.Logging;

namespace OneNoteMarkdown.Rendering
{
    internal sealed class DiagramImageRenderer
    {
        public string TryRenderToDataUri(string diagramType, string source)
        {
            string type = (diagramType ?? "diagram").Trim();
            string body = (source ?? string.Empty).Trim();
            if (body.Length == 0) return null;

            try
            {
                string[] lines = body.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
                List<string> drawLines = new List<string>();
                drawLines.Add("[" + type + "]");
                for (int i = 0; i < lines.Length && i < 50; i++)
                {
                    drawLines.Add(lines[i]);
                }

                Size size = Measure(drawLines);
                using (Bitmap bmp = new Bitmap(size.Width, size.Height))
                using (Graphics g = Graphics.FromImage(bmp))
                using (Brush fg = new SolidBrush(Color.Black))
                using (Pen border = new Pen(Color.FromArgb(150, 150, 150), 1f))
                using (Pen rule = new Pen(Color.FromArgb(220, 220, 220), 1f))
                using (Font titleFont = new Font("Consolas", 11f, FontStyle.Bold, GraphicsUnit.Point))
                using (Font bodyFont = new Font("Consolas", 10f, FontStyle.Regular, GraphicsUnit.Point))
                {
                    g.Clear(Color.White);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    Rectangle box = new Rectangle(8, 8, size.Width - 16, size.Height - 16);
                    g.DrawRectangle(border, box);
                    using (Brush headerBg = new SolidBrush(Color.FromArgb(248, 248, 248)))
                    {
                        g.FillRectangle(headerBg, new Rectangle(box.Left + 1, box.Top + 1, box.Width - 1, 26));
                    }
                    g.DrawLine(rule, box.Left, box.Top + 28, box.Right, box.Top + 28);

                    g.DrawString("[" + type + "] preview", titleFont, fg, new RectangleF(box.Left + 8, box.Top + 6, box.Width - 16, 22));

                    float y = box.Top + 34;
                    for (int i = 1; i < drawLines.Count; i++)
                    {
                        g.DrawString(drawLines[i], bodyFont, fg, new RectangleF(box.Left + 8, y, box.Width - 16, 18));
                        y += 16f;
                        if (y > box.Bottom - 18) break;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        string b64 = Convert.ToBase64String(ms.ToArray());
                        return "data:image/png;base64," + b64;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DiagramImageRenderer render failed for type=" + type, ex);
                return null;
            }
        }

        private static Size Measure(List<string> lines)
        {
            int maxChars = 20;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] != null && lines[i].Length > maxChars) maxChars = lines[i].Length;
            }
            int width = 24 + Math.Min(120, maxChars) * 8;
            int height = 52 + Math.Min(60, lines.Count) * 16;
            if (width < 280) width = 280;
            if (height < 120) height = 120;
            if (width > 1800) width = 1800;
            if (height > 1200) height = 1200;
            return new Size(width, height);
        }
    }
}
