## BEFORE INSTALLING
### Your vanilla save data
One Finger Death Punch 2 does not have a multi-slot save system. So in order to leave your vanilla save intact, this mod will automatically make a backup. On top of that, it will also create a different savefile per Archipelago seed and slot name, which allows you to... well, play as different player slots and seeds at will.
Here's some things to know about your save data:
- The savefile the game uses is located in `AppData/LocalLow/Silver Dollar Games/One Finger Death Punch 2/OFDP2Save.dat`
- The mod will automatically create a backup of your save and replace your current save with the corresponding archipelago seed/slot data, or create a new one in case of a new generation.
  - After closing the game, your save data from your current AP session will be saved in `AppData/LocalLow/Silver Dollar Games/One Finger Death Punch 2/ApSession/AP_{seed}_{slot_name}.dat`. That's the save that will be loaded the next time you log in.
- When you launch the Archipelago mod for the first time, a second backup file will also be created, named `AppData/LocalLow/Silver Dollar Games/One Finger Death Punch 2/OFDP2Save_Emergencybackup.dat`. This is your original vanilla savefile, in case something goes horribly wrong to your data. If that's the case, use it to replace `OFDP2Save.dat`

## One Finger Death Punch 2 in an Archipelago run
- Goal:
  - Clear `Stage 9-14`
  - Optional: Have at least X amount of `Skill Gems` and Clear `Stage 9-14`.
- Checks:
  - Each level clear in "Levels" Mode is a check.
    - You can choose to unlock all levels from the start (default) or not.
    - Coordinates are displayed next to each stage to help you identify locations
    - The coordinates will be written in GREEN if the location check is complete.
- Items:
  - Map unlocks
  - Skill Gems (aka Skill Points)
  - Weapon (filler)
  - Legendary Weapon (Golden-Ringed Sword or Guand Dao) (filler)
  - Revenge Token (filler)
 
## How to install
- Download and Install [Melon Loader](https://melonwiki.xyz/#/?id=automated-installation).
  - MelonLoader should detect your game automatically. If not, then the game's directory should be `C:\Program Files (x86)\Steam\steamapps\common\One Finger Death Punch 2`
  - Recommended version: 0.7.2
- Launch then close the game to finalize MelonLoader installation.
- Download the `.zip` file from the [Release page], then extract its content to the `Mods` folder in your game's directory.
- Launch the game. You should be welcomes with a (pretty ugly) login screen

## How to uninstall
Just delete the `Mods/OFDP2-Archipelago` folder and voilà.
