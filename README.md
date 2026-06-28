# Lockout Bingo Retro Generator

Prototype for a desktop app which generates a JSON file for [Lockout Bingo](https://lockout.live/) using games from the [Retro Achievements API](https://api-docs.retroachievements.org/). Built using Avalonia UI.

## Video Demo

[Video Demo Here](https://youtu.be/rx_dcnlgaWk) 

## Running

You need to run `RetroAchievementBingoGenerator.exe` inside the `Binaries` folder. You may need .NET installed. I only pushed up the Windows 64-bit files, so it won't run on any other OS. 

You will need an Retro Achievements API Key to use this app. Details for getting your key can be found [here](https://api-docs.retroachievements.org/getting-started.html#get-your-web-api-key), but basically you just need an account on the site and then go to [this page](https://retroachievements.org/settings). Your API Key will be listed there as "Web API Key".

You can make the API Key available to the program in two ways:
* Create a System Environment Variable with the name `RETRO_BINGO_API_KEY`. The value should be your API key.
* Paste it into the API Key box once you launch the program.

## Usage

Usage is simple:

1. Click "Grab Systems".
2. Checkoff any systems you want to retrieve games for.
3. Click "Grab Games".
4. Checkoff any games you want to retrieve achievements for.
5. Click "Grab Achievements".
7. Click "Copy JSON to Clipboard".
8. Navigate to the [Lockout.live JSON Editor](https://lockout.live/json-editor).
9. Paste your copied JSON into the editor.
10. Click "Play" in the top right corner.

That's the basic usage. There are some other features available as well.

First, there's some filters for games and achivements. Note that filtering does NOT uncheck/exclude a game/achievement from being included in JSON. It's merely a visual tool to make finding the game/achivement you want easier.

Filters are:
* Filter Games by Name: There's a search box to filter games by name. Currently unlabeled, but it's directly under the list of games.
* Filter Achievments by Goal and Tooltip: There's a search box to filter achievements by their goal and tooltip. Currently unlabeled, but it's directly under the list of achievements
* Filter Achievements by Game: There's a dropdown directly below the list of achievements which will filter the list to only the selected game.

There are also a few additional options available for achivements:
* Weight: Changes the `Weighting` value used in board generation. This value determines the odds the item will be included in the pool of goals used to generate the board. It's tricky to explain, so just read [this page on Bingo generation](https://wiki.lockout.live/lockout/creators/board-gen). Must be a value between 1-100 (if you go outside this range, the app will fix it during JSON generation).
* Early / Mid / Late / Endgame: Used by some Bingo modes  to determine the position on the board the goal will appear. This does NOT work for regular Bingo. I believe this is used by all other Bingo modes.

Finally, "RetroPoints" under Achievements is a value from Retro Achievements to represent how difficult that achievement is to earn. A higher number means a harder  achievement. Retro Achievements calculates using an algorithim which considers the base "Points" an achievement rewards versus how many people have earned that achievement. I included this to give you some idea how hard an achievement is. I may add in a button later that'll automatically assign progresion zones based on the "RetroPoints".

   
