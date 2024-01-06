# HighlightUploader

This program uploads videos recorded in a specific directory to Imgur and then posts the content to Discord for your friends to see

Whenever the .exe is ran, the most-recent video in the target directory will be shared to your friends on Discord

![image](https://github.com/MaxBeauchemin/HighlightUploader/assets/12040012/5cee2b19-57a7-4cbf-a73d-9227d0ff6fff)

## Build

This repository contains the Source Code necessary for the program to be built, but does not house the `.exe` itself.

You'll need to use Visual Studio Community, or MSBuild to compile the code to a useable `.exe`

Once built, you should find the program (and the `.config` file you'll need to modify in the next step) in the `bin` directory

![image](https://github.com/MaxBeauchemin/HighlightUploader/assets/12040012/a5f40347-661b-4678-aa6a-811cd710b8b6)

## Config

- Make sure you have a program that can already record highlights to a specific directory (i.e. GeForce Experience)
  - Find this directory and put it in the app.config `VideoDirectory`
- Register a Client app with [Imgur]([https://apidocs.imgur.com/?version=latest](https://apidocs.imgur.com/#authorization-and-oauth:~:text=Registration%20Quickstart)) for hosting your videos
  - Registration gives you a Client ID and Secret, you can use these in Postman to obtain a Refresh Token
  - Populate the Client ID, Client Secret, and Refresh Token fields in your app.config
  - You may need to use Postman or Curl to get a refresh token from your client credentials
  - ⚠️ **Warning** - The Imgur API is very sensitive to being called often, and will IP-block you with `HTTP 429 - Too Many Requests` for multiple days at a time if you aren't authenticated properly
- Create a Webhook on the Discord channel you want your videos to output to (will need Admin access on the server)
  - Populate the Webhook URL in the `.config`

Sample Config File

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2"/>
  </startup>
  <appSettings>
    <!--General-->
    <add key="CompressVideo" value="false"/>
    <add key="Username" value="Cloud Gamer 2024"/>
    <add key="LoggingDirectory" value="C:\Users\bryan\Documents\Highlight Uploader Logs"/>
    <!--Highlight Video-->
    <add key="VideoDirectory" value="C:\Game Clips"/>
    <add key="IncludeSubDirectories" value="true"/>
    <add key="ParentDirectoryIsGameName" value="true"/>
    <!--Imgur-->
    <add key="Imgur:ClientId" value="abcd1234"/>
    <add key="Imgur:ClientSecret" value="abcd1234"/>
    <add key="Imgur:RefreshToken" value="abcd1234"/>
    <add key="Imgur:ProcessingSecondsPollRate" value="10"/>
    <add key="Imgur:CurrentAccessToken" value=""/>
    <add key="Imgur:CurrentAccessTokenExpiration" value=""/>
    <!--Discord-->
    <add key="Discord:WebhookUrl" value="https://discord.com/api/webhooks/1234/abcd"/>
  </appSettings>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral"/>
        <bindingRedirect oldVersion="0.0.0.0-12.0.0.0" newVersion="12.0.0.0"/>
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```

### Optional - Folder Aliasing

If you don't like a folder name and want to change what will be used as the Game name, you can include a file named `FolderAliases.json` in the directory the `.exe` resides in

The file should look like this:

```json
{
	"Volume{adsfa-123-51}": "Halo Infinite"
}
```

## Usage

Now all you need to do is add a "Trigger" to run the `.exe` whenever you want. (I use a Streamdeck, but keyboard macros work great too)

Next time you capture a clip you want to share with your friends, simply wait a few seconds for the clip to be saved and press your shortcut!

## Contribution

If you'd like to contribute to this project, you can create a branch off the `master` branch and then open up a Pull Request for me to review

Please make sure you do not accidentally upload any secrets keys that you have as this would expose them
