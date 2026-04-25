using System;
using System.Collections.Generic;
using System.Text;

namespace OFDP2_Archipelago
{
    public static class ApSlotDataHelper
    {
        public static int GetRequiredSkillGems()
        {
            object dataValue = ArchipelagoFactory.Instance.GetSlotData("required_skill_gems");

            if (dataValue != null)
            {
                return Convert.ToInt32(dataValue);
            }

            return 0;
        }

        public static bool ShouldStartWithLevelsUnlocked()
        {
            object dataValue = ArchipelagoFactory.Instance.GetSlotData("unlock_all_levels");
            if (dataValue != null)
            {
                return Convert.ToBoolean(dataValue);
            }
            return false;
        }
    }
}
