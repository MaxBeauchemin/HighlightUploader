using HighlightUploader.Types;
using System;
using System.Configuration;
using System.IO;

namespace HighlightUploader.Services
{
    public static class Logger
    {
        public static void Log(string message, LogArea logArea = LogArea.General, LogType type = LogType.Info)
        {
            var logDirectory = ConfigurationManager.AppSettings["LoggingDirectory"];

            if (string.IsNullOrWhiteSpace(logDirectory)) return;

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var now = DateTime.Now;

            var fileName = string.Format("{0}_Log_{1}.txt", now.ToString("yyyyMMdd"), logArea);

            using (var sw = new StreamWriter(Path.Combine(logDirectory, fileName)))
            {
                var line = string.Format("{0} [{1}] {2}", now.ToString("yyyy/MM/dd HH:mm:ss"), type.ToString(), message);

                sw.WriteLine(line);
            }
        }
    }
}
