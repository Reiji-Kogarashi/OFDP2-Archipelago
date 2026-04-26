using System;
using System.Collections.Generic;
using System.Text;

namespace OFDP2_Archipelago
{
    public static class ApPlayerData
    {
        // public static string[] gateLevels = ["0-13", "1-27", "2-13", "2-28", "3-14", "4-14", "5-13", "6-13", "7-12"];

        public static Dictionary<string, string> mapAccessItems = new Dictionary<string, string>
        {
            { "1-14", "Map #2: Swamps" },
            { "2-14", "Map #3: Lava Lands" },
            { "2-28", "Map #3: Lava Lands" },
            { "3-14", "Map #4: Ice Lands" },
            { "6-1", "Map #4: Ice Lands" },
            { "3-29", "Map #5: Alien Crash Site" },
            { "7-1", "Map #5: Alien Crash Site" },
            { "4-15", "Map #6: Sky Lands" },
            { "8-14", "Map #6: Sky Lands" },
            { "5-15", "Map #7: Wastelands" },
            { "8-1", "Map #7: Wastelands" },
            { "6-14", "Map #8: Land of Temples" },
            { "7-14", "Map #8: Land of Temples" },
            { "8-13", "Map #9: ???" }
        };

        public static List<string> possessedStageUnlockItems = new List<string>();

        public static List<string> completedLocations = new List<string>();
    }
}
