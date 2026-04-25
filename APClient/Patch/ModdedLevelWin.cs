using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(ScriptMain), "LevelWin")]
    public static class ModdedLevelWin
    {
        public struct ApGoalStage
        {
            public int mapNumber;
            public int mapLevel;
        }

        /// <summary>
        ///  TODO: Different goal stages
        /// </summary>
        public static ApGoalStage goalStage = new ApGoalStage() { mapNumber = 9, mapLevel = 14 };

        public static bool IsLevelFinished = false;

        private static void Postfix(ScriptMain __instance)
        {
            // World 1 : Stage 18 to 47 - Gate to map 2 at level 0-13
            // World 2 : Stage 48 to 77 - Gate to map 3 => 1-27
            // World 3 : Stage 78 to 107 - Gate to map 4 => 2-13; Gate to map 5 => 2-28
            // World 4 : Stage 108 to 137 - Gate to map 6 => 3-14
            // World 5 : Stage 138 to 167 - Gate to map 7 => 4-14
            // World 6 : Stage 168 to 197 - Gate to map 8 => 5-13
            // World 7 : Stage 198 to 227 - Gate to map 8 => 6-13
            // World 8 : Stage 228 to 256 - Gate to map 9 => 7-12
            // World 9 : Stage 257 to 286

            IsLevelFinished = true;

            if (PlayGlobal.multiRound > 1)
            {
                return;
            }

            __instance.scriptUIPlay.gameobjectButtonSkills.SetActive(true);

            int mapNumber = PlayerSave.mapNumber + 1;
            int mapLevel = PlayerSave.mapLevel + 1;

            if (mapNumber == goalStage.mapNumber && mapLevel == goalStage.mapLevel && PlayerSave.skillUnusedStars >= ApSlotDataHelper.GetRequiredSkillGems())
            {
                ArchipelagoFactory.Instance.ReportGoal();
                return;
            }

            string finishedLevel = mapNumber.ToString() + "-" + mapLevel.ToString();

            if (ApPlayerData.mapAccessItems.ContainsKey(finishedLevel))
            {
                string mapAccessItem = ApPlayerData.mapAccessItems[finishedLevel];
                PlayerSave.mapLevelBeaten[PlayerSave.mapNumber, PlayerSave.mapLevel] = ApPlayerData.possessedStageUnlockItems.Contains(mapAccessItem);
            }

            string locationName = "Stage " + finishedLevel;
            ArchipelagoFactory.Instance.ReportCheckCompletion(locationName);
        }
    }
}

