using HighlightUploader.DTOs;
using HighlightUploader.Types;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HighlightUploader.Services
{
    public static class ImgurHelper
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

                Logger.Log(ex.Message, LogArea.Imgur, LogType.Error, ex);
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

        private static string ClientSecret()
        {
            var secret = ConfigurationManager.AppSettings["Imgur:ClientSecret"];

            return secret;
        }

        private static string RefreshToken()
        {
            var refreshToken = ConfigurationManager.AppSettings["Imgur:RefreshToken"];

            return refreshToken;
        }

        private static string AccessToken()
        {
            var currToken = ConfigurationManager.AppSettings["Imgur:CurrentAccessToken"];

            if (currToken != null)
            {
                var tokenExpiration = DateTime.Parse(ConfigurationManager.AppSettings["Imgur:CurrentAccessTokenExpiration"]);

                if (DateTime.Now < tokenExpiration)
                {
                    return currToken;
                }
            }

            var newToken = GenerateAccessToken();

            return newToken;
        }

        private static string GenerateAccessToken()
        {
            var tokenUrl = "https://api.imgur.com/oauth2/token";

            var httpClient = new HttpClient();

            var body = new
            {
                refresh_token = RefreshToken(),
                client_id = ClientId(),
                client_secret = ClientSecret(),
                grant_type = "refresh_token"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(tokenUrl))
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            };

            var httpResponse = httpClient.SendAsync(request).Result;

            if (!httpResponse.IsSuccessStatusCode) throw new ArgumentException(string.Format("Error Generating Access Token: {0}", httpResponse.StatusCode));

            var res = httpResponse.Content.ReadAsStringAsync();
            var tokenObj = JsonConvert.DeserializeObject<ImgurTokenResponse>(res.Result);

            ConfigurationManager.AppSettings.Set("Imgur:CurrentAccessToken", tokenObj.access_token);
            ConfigurationManager.AppSettings.Set("Imgur:CurrentAccessTokenExpiration", DateTime.Now.AddSeconds(tokenObj.expires_in).ToString("yyyy-MM-dd hh:mm:ss"));

            return tokenObj.access_token;
        }
    }
}
