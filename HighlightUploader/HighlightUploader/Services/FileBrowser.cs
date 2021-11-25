using HighlightUploader.DTOs;
using HighlightUploader.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace HighlightUploader.Services
{
    public static class FileBrowser
    {
        public static Response<string> DeleteFile(string filepath)
        {
            var response = new Response<string> { Success = true };

            try
            {
                File.Delete(filepath);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                Logger.Log(ex.Message, LogArea.FileIO, LogType.Error, ex);
            }

            return response;
        }

        public static Response<string> GetLatestFile()
        {
            var response = new Response<string>{ Success = true };

            try
            {
                var directoryPath = ConfigurationManager.AppSettings["VideoDirectory"];
                var includeSubDirectories = bool.Parse(ConfigurationManager.AppSettings["IncludeSubDirectories"]);

                var filepath = GetLatestDirectoryFilePath(directoryPath, includeSubDirectories);

                response.Value = filepath;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                Logger.Log(ex.Message, LogArea.FileIO, LogType.Error, ex);
            }

            return response;
        }

        private static string GetLatestDirectoryFilePath(string path, bool includeSubDirectories)
        {
            var filePaths = Directory.GetFiles(path).ToList();

            if (includeSubDirectories)
            {
                var subDirectories = Directory.GetDirectories(path);

                foreach (var sub in subDirectories)
                {
                    var oldestFilePath = GetLatestDirectoryFilePath(sub, includeSubDirectories);

                    filePaths.Add(oldestFilePath);
                }
            }

            return GetOldestFile(filePaths, "mp4");
        }

        private static string GetOldestFile(List<string> filePaths, string extension)
        {
            DateTime latestWrite = DateTime.MinValue;
            string latestFilePath = null;

            foreach (var f in filePaths)
            {
                if (f == null) continue;

                if (!f.ToLower().EndsWith(string.Format(".{0}", extension))) continue;

                var lastWrite = File.GetLastWriteTime(f);

                if (lastWrite > latestWrite)
                {
                    latestWrite = lastWrite;
                    latestFilePath = f;
                }
            }

            return latestFilePath;
        }
    }
}
