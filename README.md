# HighlightUploader
This program uploads videos recorded in a specific directory to Imgur and then posts the content to Discord for your friends to see

Whenever the .exe is ran, the most-recent video in the target directory will be shared to your friends on Discord

There are a few steps required to get this working

1. Make sure you have a program that can already record highlights to a specific directory (i.e. GeForce Experience)

  a. Find this directory and put it in the app.config "VideoDirectory"
  b. Make sure to turn "IncludeSubDirectories" OFF if you only want to find top-level videos

2. Register a Client app with Imgur for hosting your videos (https://apidocs.imgur.com/?version=latest)

  a. Populate the Client Id and Refresh Token fields in your app.config

3. Create a Webhook on the Discord channel you want your videos to output to (will need Admin access on the server)

  a. Populate the Webhook URL in the app.config
  
4. Create some sort of 'trigger' to run the EXE on command (I use a Streamdeck, but keyboard macros work great too)
