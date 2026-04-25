using HarmonyLib;
using UnityEngine;
using MelonLoader;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(ScriptLevelPoint), "Update")]
    public static class ModdedLevelPointStart
    {
        private static void Postfix(ScriptLevelPoint __instance)
        {
            if (__instance.textLevelNumber == null || __instance.textLevelNumber.enabled)
            {
                return;
            }

            string stageCoordinates = (PlayerSave.mapNumber + 1).ToString() + "-" + (__instance.levelNumber + 1).ToString();
            string stageLocationName = "Stage " + stageCoordinates;

            if (ApPlayerData.completedLocations.Contains(stageLocationName))
            {
                __instance.textLevelNumber.color = new Color(0f, 0.5f, 0f);
            }
            else
            {
                __instance.textLevelNumber.color = Color.white;
            }

            __instance.textLevelNumber.text = stageCoordinates;
            __instance.textLevelNumber.enabled = true;
        }
    }
}
