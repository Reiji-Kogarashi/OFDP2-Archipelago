using HarmonyLib;
using MelonLoader;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(ScriptLevelsMain), "ButtonClicked", new Type[] { typeof(ScriptLevelPoint) })]
    public static class ModdedLevelClick
    {
        private static void Postfix(ScriptLevelsMain __instance)
        {
            int mapNumber = PlayerSave.mapNumber;
            int mapLevel = PlayerSave.mapLevel;

            GameControl.control.textPanel.textText[0].SetText("Play this level?");

            Melon<Core>.Logger.Msg($"Selected level {mapNumber}-{mapLevel}");


            ScriptLevelsMain.LevelTo[] levelTo = __instance.map[mapNumber].levelPositionStats[mapLevel].levelTo;
            int numLevelTo = levelTo.Length;
            for (int i = 0; i < numLevelTo; i++)
            {
                Melon<Core>.Logger.Msg($"LevelTo = {levelTo[i].to}");
            }

            Melon<Core>.Logger.Msg($"Tutorial Level = {PlayerSave.tutorialLevel}");
        }
    }
}
