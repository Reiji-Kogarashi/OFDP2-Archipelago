using HarmonyLib;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(GameControl), "Load")]
    public static class ModdedGameControlLoad
    {
        private static void Postfix(GameControl __instance)
        {
            int numGroups = __instance.levelDataGroup.Length;
            for (int i = 0; i < numGroups; i++)
            {
                int numLevels = __instance.levelDataGroup[i].levelData.Length;
                for (int j = 0; j < numLevels; j++)
                {
                    if (__instance.levelDataGroup[i].levelData[j] == null)
                    {
                        continue;
                    }

                    __instance.levelDataGroup[i].levelData[j].revengeToken = false;
                    __instance.levelDataGroup[i].levelData[j].skillPoint = false;
                }
            }
        }
    }
}
