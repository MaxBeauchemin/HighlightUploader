using HighlightUploader.DTOs;
using HighlightUploader.Types;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HighlightUploader.Services
{
    public static class Imgur
    {
        public static Response<ImgurResponse<ImgurFileResponse>> UploadFile(string filepath, bool waitForProcessing = false)
        {
            var response = new Response<ImgurResponse<ImgurFileResponse>> { Success = true };

            try
            {
                var httpClient = new HttpClient();

                var timeout = new TimeSpan(0, 5, 0);

                httpClient.Timeout = timeout;

                var uploadUrl = "https://api.imgur.com/3/upload";

                var content = new MultipartFormDataContent();

                var streamContent = new StreamContent(File.Open(filepath, FileMode.Open));
                content.Add(streamContent, "video", filepath);

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(uploadUrl))
                {
                    Content = content
                };

                var authValue = string.Format("Client-ID {0},Bearer {1}", ClientId(), AccessToken());

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", authValue);

                var httpResponse = httpClient.SendAsync(request).Result;

                if (!httpResponse.IsSuccessStatusCode) throw new ArgumentException(string.Format("HTTP Status: {0}", httpResponse.StatusCode));

                var res = httpResponse.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ImgurResponse<ImgurFileResponse>>(res.Result);

                response.Value = obj;

                if (waitForProcessing)
                {
                    var pollingRateSeconds = Int32.Parse(ConfigurationManager.AppSettings["Imgur:ProcessingSecondsPollRate"]);

                    while (!FileDoneProcessing(obj.data.id))
                    {
                        System.Threading.Thread.Sleep(pollingRateSeconds * 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                Logger.Log(ex.Message, LogArea.Imgur, LogType.Error);
            }

            return response;
        }

        private static bool FileDoneProcessing(string idHash)
        {
            var statusUrl = string.Format("https://api.imgur.com/3/image/{0}", idHash);

            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(statusUrl));

            var authValue = string.Format("Bearer {0}", AccessToken());

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", authValue);

            var httpResponse = httpClient.SendAsync(request).Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var res = httpResponse.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ImgurResponse<ImgurFileResponse>>(res.Result);

                return obj.data.processing.status == "completed";
            }

            return false;
        }

        private static string ClientId()
        {
            var imgurClientId = ConfigurationManager.AppSettings["Imgur:ClientId"];

            return imgurClientId;
        }

        private static string AccessToken()
        {
            var imgurAccessToken = ConfigurationManager.AppSettings["Imgur:AccessToken"];

            return imgurAccessToken;
        }

        private static string GenerateAccessToken()
        {
            //TODO!!!!!

            return null;
        }
    }
}
