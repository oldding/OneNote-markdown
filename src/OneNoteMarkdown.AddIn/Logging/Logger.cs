using System;
using System.IO;
using System.Text;

namespace OneNoteMarkdown.Logging
{
    public static class Logger
    {
        private const long MaxLogFileSizeBytes = 5L * 1024L * 1024L;
        private static readonly object SyncRoot = new object();
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OneNoteMarkdown",
            "logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "onenotemarkdown.log");
        private static readonly string BackupLogFilePath = Path.Combine(LogDirectory, "onenotemarkdown.log.bak");
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        public static void Initialize()
        {
            lock (SyncRoot)
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        public static void Info(string message)
        {
            WriteLog("INFO", message, null);
        }

        public static void Warn(string message)
        {
            WriteLog("WARN", message, null);
        }

        public static void Error(string message, Exception ex)
        {
            WriteLog("ERROR", message, ex);
        }

        private static void WriteLog(string level, string message, Exception ex)
        {
            string safeMessage = string.IsNullOrWhiteSpace(message) ? string.Empty : message;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = string.Format("[{0}] [{1}] {2}", timestamp, level, safeMessage);
            if (ex != null)
            {
                logEntry += Environment.NewLine + ex;
            }

            try
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(LogDirectory);
                    RotateIfNeeded();
                    AppendLogEntry(logEntry);
                }
            }
            catch
            {
            }
        }

        private static void RotateIfNeeded()
        {
            FileInfo fileInfo = new FileInfo(LogFilePath);
            if (!fileInfo.Exists || fileInfo.Length <= MaxLogFileSizeBytes)
            {
                return;
            }

            if (File.Exists(BackupLogFilePath))
            {
                File.Delete(BackupLogFilePath);
            }

            File.Move(LogFilePath, BackupLogFilePath);
        }

        private static void AppendLogEntry(string logEntry)
        {
            using (FileStream stream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(stream, Utf8NoBom))
            {
                writer.WriteLine(logEntry);
            }
        }
    }
}
