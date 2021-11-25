using HighlightUploader.DTOs;
using HighlightUploader.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace HighlightUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Log("--- Starting Program ---");

            var fileSeekerResponse = FileBrowser.GetLatestFile();

            if (!fileSeekerResponse.Success) return;

            string game = null;

            var pathTokens = fileSeekerResponse.Value.Split('\\').ToList();

            var parseGame = bool.Parse(ConfigurationManager.AppSettings["ParentDirectoryIsGameName"]);

            if (parseGame && pathTokens.Count > 2)
            {
                game = pathTokens[pathTokens.Count - 2];
            }

            var shouldCompress = bool.Parse(ConfigurationManager.AppSettings["CompressVideo"]);

            var uploadFilePath = fileSeekerResponse.Value;

            if (shouldCompress)
            {
                var compressResponse = VideoCompressor.Compress(fileSeekerResponse.Value);

                if (!compressResponse.Success) return;

                uploadFilePath = compressResponse.Value;
            }

            var imgurResponse = ImgurHelper.UploadFile(uploadFilePath, true);

            if (!imgurResponse.Success) return;

            var webhookUrl = ConfigurationManager.AppSettings["Discord:WebhookUrl"];
            var username = ConfigurationManager.AppSettings["Username"];

            var url = imgurResponse.Value.data.link;

            //Format Discord Message

            var discordContentFormat = "Check out this new Clip!\nPlayer:    `{0}`\n{1}\nUrl:          {2}";

            var gameContent = string.Empty;

            if (game != null && game != "Desktop")
            {
                var currDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                var aliasFilePath = currDirectory + "\\FolderAliases.json";

                if (File.Exists(aliasFilePath))
                {
                    var jsonAliases = File.ReadAllText(aliasFilePath);

                    var aliases = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonAliases);

                    if (aliases.ContainsKey(game))
                    {
                        game = aliases[game];
                    }
                }

                gameContent = string.Format("Game:    `{0}`\n", game);
            }
            
            var discordContent = string.Format(discordContentFormat, username, gameContent, url);
            
            var discordBody = new DiscordMessage
            {
                content = discordContent
            };

            var discordResponse = Discord.PostMessage(webhookUrl, discordBody);

            if (!discordResponse.Success) return;

            if (shouldCompress)
            {
                FileBrowser.DeleteFile(uploadFilePath);
            }
        }
    }
}
