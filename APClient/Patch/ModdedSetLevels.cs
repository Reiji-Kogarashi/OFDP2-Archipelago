using HarmonyLib;
using MelonLoader;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(ScriptLevelsMain), "SetLevels")]
    public static class ModdedSetLevels
    {
        private static void Postfix(ScriptLevelsMain __instance)
        {
            // Enable/disable exits
            int numLevels = __instance.map[PlayerSave.mapNumber].levelPositionStats.Length;
            for (int j = 0; j < numLevels; j++)
            {
                string stageCoordinates = (PlayerSave.mapNumber + 1).ToString() + "-" + (j + 1).ToString();
                if (ApPlayerData.mapAccessItems.ContainsKey(stageCoordinates))
                {
                    string mapAccessItem = ApPlayerData.mapAccessItems[stageCoordinates];
                    __instance.map[PlayerSave.mapNumber].levelPositionStats[j].isExit = ApPlayerData.possessedStageUnlockItems.Contains(mapAccessItem);
                }
            }  
        }
    }
}
