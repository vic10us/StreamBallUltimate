# StreamBallUltimate Game

_A game that is an overlay for twitch streams. It connects to the client and chat can play a series of minigames using marble._

## Game Settings and Configuration

On first launch the application will generate a `global.cfg` file in the _Application.persistentDataPath_ folder.

This is a standard INI file and it is used to configure the different aspects of the game.

_The location of Application.persistentDataPath folder depends on the OS that is running the game. Check out this [Link](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) to find out more._

### Twitch

To configure the Twich configuration options add a new `[twitch]` section in the `global.cfg` and set the appropriate values to match your particular setup.

Example:

```(ini)
[twitch]
bot_name=REPLACE_THIS_WITH_THE_USERNAME_OF_THE_BOT
channel_name=REPLACE_WITH_THE_NAME_OF_THE_TWITCH_CHANNEL
bot_access_token=REPLACE_THIS_WITH_YOUR_AUTH_TOKEN
client_id=NOT_USED_YET
client_secret=NOT_USED_YET
bot_refresh_token=NOT_USED_YET
```

### Marbles

The game comes with one (1) default marble called default, but the game requires that there be at least three (3). If the game starts with less, it will fill the remaining slots with another default marble and give it a randomly generated name, a cost of 9999 and a rarity of 4.

The default marble is free and is given to all users who join the game in chat.

Adding additional custom marbles is quite simple. Create a folder on the computer and place all the marble images (see image requirements below) in it.

Make sure that the path is added to the `global.cfg` file in the `[marbles]` section and make sure to set `LoadExternalMarbles=true` or the game will not try to load anthing.

Example:

```(ini)
[marbles]
ExternalPath=D:\Media\CustomMarbles
LoadExternalMarbles=true
```

When the game loads, it will scan this folder for PNG images and it will generate a `meta-data.json` in the same folder. This is where you can adjust the cost, rarity and CommonName in the store.

### Marble Images

For the best success and optimal performance, make sure to follow these rules when creating new marble images for the game.

1. The dimensions should be 500px by 500px
2. The file type should be PNG
3. The file should be saved with transparency
4. Ideally the image should be round-ish _(It's not critical, but it makes things look bettter)_

### Player Data

The player data file `PlayerData.json` will be stored in the same folder as the `global.cfg` and will be generated on first launch or when the first player joins. This will track player related data like, in-game money and purchased items like marbles.
