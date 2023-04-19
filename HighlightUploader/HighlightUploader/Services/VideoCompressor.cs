using HighlightUploader.DTOs;
using HighlightUploader.Types;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;

namespace HighlightUploader.Services
{
    public static class VideoCompressor
    {
        public static Response<string> Compress(string filepath)
        {
            var response = new Response<string> { Success = true };

            try
            {
                var extensionIndex = filepath.LastIndexOf('.');

                var outputFilePath = string.Format("{0}_compressed.{1}", filepath.Substring(0, extensionIndex), filepath.Substring(extensionIndex + 1));

                var inputFile = new MediaFile { Filename = filepath };
                var outputFile = new MediaFile { Filename = outputFilePath };

                var options = new ConversionOptions
                {
                    VideoSize = VideoSize.Hd480,
                    VideoAspectRatio = VideoAspectRatio.R16_9,
                    VideoFps = 20,
                    VideoBitRate = 4000
                };

                using (var engine = new Engine())
                {
                    engine.Convert(inputFile, outputFile, options);
                }

                response.Value = outputFilePath;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                Logger.Log(ex.Message, LogArea.VideoCompressing, LogType.Error);
            }

            return response;
        }
    }
}
