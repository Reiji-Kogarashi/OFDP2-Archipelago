using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace OFDP2_Archipelago
{
    [HarmonyPatch(typeof(ScriptMain), "TwitchFightMe", new Type[] { typeof(ScriptMain.EnemyList) })]
    public static class ModdedFightMe
    {
        public enum FightMeType
        {
            BATALLION,
            BRAWLER
        }

        private struct FightMeTrapInfo
        {
            public FightMeType Type;
            public string PlayerName;
        }

        private static Queue<FightMeTrapInfo> m_TrapsInQueue = new Queue<FightMeTrapInfo>();

        private static string m_BatallionName = "";
        private static int m_RemainingBatallionMembers = 0;

        public static void QueueFightMeTrap(string playerName)
        {
            FightMeTrapInfo newTrap = new FightMeTrapInfo();
            newTrap.Type = (UnityEngine.Random.Range(0,2) == 1 ? FightMeType.BRAWLER : FightMeType.BATALLION);
            newTrap.PlayerName = playerName;

            m_TrapsInQueue.Enqueue(newTrap);
        }

        private static void Postfix(ScriptMain __instance, ScriptMain.EnemyList enemy)
        {
            if (enemy.script.specialEnemy != EnumSpecialEnemy.None || enemy.script.isThrower)
            {
                return;
            }

            if (m_RemainingBatallionMembers > 0)
            {
                CreateEnemyTrap(enemy, m_BatallionName);
                m_RemainingBatallionMembers--;
            }
            else if (m_TrapsInQueue.Count > 0)
            {
                FightMeTrapInfo trapInfo = m_TrapsInQueue.Peek();

                if (trapInfo.Type == FightMeType.BATALLION)
                {
                    m_BatallionName = trapInfo.PlayerName + "'s gang";
                    m_RemainingBatallionMembers = UnityEngine.Random.Range(20, 50);
                    UIHelper.Instance.QueueNotification($"{trapInfo.PlayerName}: \"Get'em, boys! (Trap)\"", true);
                }
                else
                {
                    bool isBrawler = enemy.script.isBrawler && trapInfo.Type == FightMeType.BRAWLER;

                    if (isBrawler)
                    {
                        CreateEnemyTrap(enemy, trapInfo.PlayerName);
                        UIHelper.Instance.QueueNotification($"{trapInfo.PlayerName}: \"You! Fight me! (Trap)\"", true);
                    }
                }

                m_TrapsInQueue.Dequeue();
            }
        }

        private static void CreateEnemyTrap(ScriptMain.EnemyList enemy, string playerName)
        {
            if (enemy.script.isBrawler)
            {
                enemy.script.isNemesis = true;
            }

            enemy.script.fightMe = true;
            enemy.script.canvasSign.enabled = true;
            enemy.script.imageSign.color = Color.blue;
            enemy.script.textSign.text = playerName;
        }
    }
}
