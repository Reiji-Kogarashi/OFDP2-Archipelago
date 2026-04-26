using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace OFDP2_Archipelago
{
    public static class ModdedDataHelper
    {
        public static void InitNewPlayerSave()
        {
            PlayerSave.tutorialLevel = 15;

            int numTutorial = PlayerSave.tutorialLevelBeaten.Length;
            for (int i = 0; i < numTutorial; i++)
            {
                PlayerSave.tutorialLevelBeaten[i] = true;
            }

            PlayerSave.skillUnusedStars = 0;

            if (ApSlotDataHelper.ShouldStartWithLevelsUnlocked())
            {
                UnlockAllLevels();
            }

            if (GameControl.control != null)
            {
                GameControl.control.StartCoroutine("SaveData");
            }
        }

        private static void UnlockAllLevels()
        {
            int numMaps = PlayerSave.mapLevelBeaten.GetLength(0);
            int numLevels = PlayerSave.mapLevelBeaten.GetLength(1);
            for (int i = 0; i < numMaps; i++)
            {
                for (int j = 0; j < numLevels; j++)
                {
                    PlayerSave.mapLevelBeaten[i, j] = true;
                }
            }
        }
    }
}
