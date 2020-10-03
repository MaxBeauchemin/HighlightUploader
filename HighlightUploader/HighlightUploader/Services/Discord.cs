using HighlightUploader.DTOs;
using HighlightUploader.Types;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace HighlightUploader.Services
{
    public static class Discord
    {
        public static Response<string> PostMessage(string webhookUrl, DiscordMessage message)
        {
            var response = new Response<string> { Success = true };

            try
            {
                var httpClient = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(webhookUrl))
                {
                    Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json")
                };

                var httpResponse = httpClient.SendAsync(request).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    var res = httpResponse.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                Logger.Log(ex.Message, LogArea.Discord, LogType.Error);
            }

            return response;
        }
    }
}
