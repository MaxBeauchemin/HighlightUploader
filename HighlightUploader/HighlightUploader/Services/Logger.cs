using HighlightUploader.Types;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace HighlightUploader.Services
{
    public static class Logger
    {
        public static void Log(string message, LogArea logArea = LogArea.General, LogType type = LogType.Info, object logData = null)
        {
            var logDirectory = ConfigurationManager.AppSettings["LoggingDirectory"];

            if (string.IsNullOrWhiteSpace(logDirectory)) return;

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var now = DateTime.Now;

            var fileName = string.Format("{0}_Log_{1}.txt", now.ToString("yyyyMMdd"), logArea);

            var fullPath = Path.Combine(logDirectory, fileName);

            var sb = new StringBuilder();

            sb.AppendLine(string.Format("{0} [{1}] {2}", now.ToString("yyyy/MM/dd HH:mm:ss"), type.ToString(), message));

            if (logData != null)
            {
                var json = JsonConvert.SerializeObject(logData, Formatting.Indented);

                sb.AppendLine("Data: ");
                sb.AppendLine("-------------");
                sb.AppendLine(json);
                sb.AppendLine("-------------");
                sb.AppendLine();
            }

            File.AppendAllText(fullPath, sb.ToString());
        }
    }
}
