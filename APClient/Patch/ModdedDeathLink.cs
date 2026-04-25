using HarmonyLib;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(ScriptMain), "PlayerDie")]
    public static class ModdedDeathLink
    {
        private static void Postfix()
        {
            string playerName = ArchipelagoFactory.Instance.PlayerAlias;

            string[] funnyMessages =
            {
                $"{playerName} challenged Chuck Norris. The result was obvious...",
                $"{playerName} thought he was Jackie Chan and forgot to parkour",
                $"{playerName} ain't gonna be in Rush Hour 3...",
                $"{playerName} button mashed to death",
                $"{playerName} was like a board: he didn't hit back"
            };

            int randomIndex = UnityEngine.Random.Range(0, funnyMessages.Length);
            string randomMessage = funnyMessages[randomIndex];

            ArchipelagoFactory.Instance.ReportDeathLink(randomMessage);
        }
    }
}
