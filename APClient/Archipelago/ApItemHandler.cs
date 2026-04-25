using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using MelonLoader;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace OFDP2_Archipelago
{
    public class ApItemHandler
    {
        struct ReceivedWeaponInfo
        {
            public string sender;
            public string weaponName;
            public EnumMartialArt martialArt;
        }

        Dictionary<EnumMartialArt, string> m_ListWeapons = new Dictionary<EnumMartialArt, string>
        {
            { EnumMartialArt.Sword, "Sword" },
            { EnumMartialArt.HookSwords, "Pair of Hook Swords" },
            { EnumMartialArt.Staff, "Staff" },
            { EnumMartialArt.Katana, "Katana" },
            { EnumMartialArt.Axe, "Axe" },
            { EnumMartialArt.Nunchuck, "Nunchuck" },
            { EnumMartialArt.Scythe, "Scythe" },
            { EnumMartialArt.Club, "Club" },
            { EnumMartialArt.Tonfa, "Tonfa" },
            { EnumMartialArt.BroadSword, "Broadsword" },
            { EnumMartialArt.GreatSword, "Greatsword" },
            { EnumMartialArt.Spear, "Spear" },
            { EnumMartialArt.Kama, "Kama" },
            { EnumMartialArt.Flail, "Flail" },
            { EnumMartialArt.Parasol, "Parasol" },
            { EnumMartialArt.Broom, "Broom" },
            { EnumMartialArt.Guitar, "Guitar" }
        };

        Queue<ReceivedWeaponInfo> m_ReceivedWeapons = new Queue<ReceivedWeaponInfo>();

        Dictionary<EnumMartialArt, string> m_LightSwordColors = new Dictionary<EnumMartialArt, string>
        {
            { EnumMartialArt.LightSwordRed, "Red Lightsaber" },
            { EnumMartialArt.LightSword, "Green Lightsaber" },
            { EnumMartialArt.LightSwordBlue, "Blue Lightsaber" },
            { EnumMartialArt.LightSwordPurple, "Purple Lightsaber" }
        };

        Queue<ReceivedWeaponInfo> m_ReceivedPowerWeapons = new Queue<ReceivedWeaponInfo>();

        Queue<string> m_LegendaryWeaponSenders = new Queue<string>();

        bool m_LegendaryWeaponTriggered = false;

        float m_FirstActivationTimer = 1f;
        bool m_Active = false;

        public ApItemHandler() { }

        public void Activate()
        {
            m_Active = true;
        }

        public void Update()
        {
            if (!m_Active)
                return;

            if (m_FirstActivationTimer > 0f)
            {
                m_FirstActivationTimer -= Time.unscaledDeltaTime;
                return;
            }

            if (!ModdedLevelWin.IsLevelFinished && ScriptMain.SM != null)
            {
                EnumMartialArt currentMartialArt = ScriptMain.SM.player.martialArtStyle;
                if (!m_LegendaryWeaponTriggered && currentMartialArt == EnumMartialArt.DuxRyu && m_ReceivedWeapons.Count > 0)
                {
                    EquipWeapon();
                }

                if (!m_LegendaryWeaponTriggered && currentMartialArt == EnumMartialArt.DuxRyu && m_ReceivedPowerWeapons.Count > 0 && ScriptMain.SM.roundMod != EnumRoundMod.HorrorShow)
                {
                    EquipPowerWeapon();
                }

                if (m_LegendaryWeaponTriggered && !ScriptMain.SM.glorySwordActive)
                {
                    EquipLegendaryWeapon();
                }
            }
        }

        public void ReceiveItem(ItemInfo itemInfo)
        {
            if (itemInfo.ItemName.Contains("Map #"))
            {
                ReceiveMapItem(itemInfo);
                return;
            }

            switch (itemInfo.ItemName)
            {
                case "Revenge Token":
                    ReceiveRevengeToken(itemInfo);
                    break;

                case "Skill Gem":
                    ReceiveSkillPoint(itemInfo);
                    break;

                case "Fight Me Trap":
                    ReceiveFightMeTrap(itemInfo);
                    break;

                case "Weapon":
                    ReceiveWeapon(itemInfo);
                    break;

                case "Lightsaber":
                    ReceiveLightSword(itemInfo);
                    break;

                case "Chainsaw":
                    ReceiveChainsaw(itemInfo);
                    break;

                case "Legendary Weapon":
                    ReceiveLegendaryWeapon(itemInfo);
                    break;

                default:
                    break;
            }
        }

        void ReceiveRevengeToken(ItemInfo itemInfo)
        {
            PlayerSave.revengeTokens++;
            //AudioManager.audioManager.Play("voice 80");

            if (itemInfo.Player.Name == ArchipelagoFactory.Instance.SlotName)
            {
                UIHelper.Instance.QueueNotification($"You found a Revenge Token (Owned: {PlayerSave.revengeTokens})");
            }
            else
            {
                UIHelper.Instance.QueueNotification($"{itemInfo.Player.Alias} sent you a Revenge Token (Owned: {PlayerSave.revengeTokens})");
            }
        }

        void ReceiveSkillPoint(ItemInfo itemInfo)
        {
            PlayerSave.skillUnusedStars++;

            string message;

            if (itemInfo.Player.Name == ArchipelagoFactory.Instance.SlotName)
            {
                message = "You found a Skill Gem";
            }
            else
            {
                message = $"{itemInfo.Player.Alias} sent you a Skill Gem";
            }

            int requiredGems = ApSlotDataHelper.GetRequiredSkillGems();
            if (requiredGems > 0)
            {
                message += $" (Required: {PlayerSave.skillUnusedStars}/{requiredGems})";
            }

            UIHelper.Instance.QueueNotification(message);
        }

        void ReceiveMapItem(ItemInfo itemInfo)
        {
            bool itemFound = false;

            foreach (KeyValuePair<string, string> itemData in ApPlayerData.mapAccessItems)
            {
                if (itemData.Value == itemInfo.ItemName)
                {
                    ApPlayerData.possessedStageUnlockItems.Add(itemData.Value);

                    string[] stageCoordinatesStr = itemData.Key.Split('-');
                    PlayerSave.mapLevelBeaten[int.Parse(stageCoordinatesStr[0]) - 1, int.Parse(stageCoordinatesStr[1]) - 1] = true;

                    itemFound = true;
                }
            }

            if (itemFound)
            {
                if (itemInfo.Player.Name == ArchipelagoFactory.Instance.SlotName)
                {
                    UIHelper.Instance.QueueNotification($"You found a Map Access Item: {itemInfo.ItemName}");
                }
                else
                {
                    UIHelper.Instance.QueueNotification($"{itemInfo.Player.Alias} sent you a Map Access Item: {itemInfo.ItemName}");
                }
            }
        }

        void ReceiveFightMeTrap(ItemInfo itemInfo)
        {
            if (m_FirstActivationTimer > 0f)
                return;

            ModdedFightMe.QueueFightMeTrap(itemInfo.Player.Alias);
        }

        void ReceiveWeapon(ItemInfo itemInfo)
        {
            if (m_FirstActivationTimer > 0f)
                return;

            int randomWeaponIndex = UnityEngine.Random.Range(0, m_ListWeapons.Count);
            var weapon = m_ListWeapons.ElementAt(randomWeaponIndex);

            ReceivedWeaponInfo newWeapon = new ReceivedWeaponInfo()
            {
                sender = (itemInfo.Player.Name != ArchipelagoFactory.Instance.SlotName) ? itemInfo.Player.Alias : "",
                weaponName = weapon.Value,
                martialArt = weapon.Key
            };

            m_ReceivedWeapons.Enqueue(newWeapon);
        }

        void ReceiveLightSword(ItemInfo itemInfo)
        {
            if (m_FirstActivationTimer > 0f)
                return;

            int randomLightswordIndex = UnityEngine.Random.Range(0, m_LightSwordColors.Count);
            var lightsword = m_LightSwordColors.ElementAt(randomLightswordIndex);

            ReceivedWeaponInfo newWeapon = new ReceivedWeaponInfo()
            {
                sender = (itemInfo.Player.Name != ArchipelagoFactory.Instance.SlotName) ? itemInfo.Player.Alias : "",
                weaponName = lightsword.Value,
                martialArt = lightsword.Key
            };

            m_ReceivedPowerWeapons.Enqueue(newWeapon);
        }

        void ReceiveChainsaw(ItemInfo itemInfo)
        {
            if (m_FirstActivationTimer > 0f)
                return;

            ReceivedWeaponInfo newWeapon = new ReceivedWeaponInfo()
            {
                sender = (itemInfo.Player.Name != ArchipelagoFactory.Instance.SlotName) ? itemInfo.Player.Alias : "",
                weaponName = "Chainsaw",
                martialArt = EnumMartialArt.Chainsaw
            };

            m_ReceivedPowerWeapons.Enqueue(newWeapon);
        }

        void ReceiveLegendaryWeapon(ItemInfo itemInfo)
        {
            if (m_FirstActivationTimer > 0f || GameControl.control == null)
                return;

            m_LegendaryWeaponTriggered = true;
            m_LegendaryWeaponSenders.Enqueue((itemInfo.Player.Name != ArchipelagoFactory.Instance.SlotName) ? itemInfo.Player.Alias : "");
        }

        void EquipWeapon()
        {
            if (!m_LegendaryWeaponTriggered && ScriptMain.SM != null)
            {
                ReceivedWeaponInfo weaponInfo = m_ReceivedWeapons.Dequeue();

                ScriptMain.SM.player.martialArtStyleTarget = weaponInfo.martialArt;
                ScriptMain.SM.player.weaponHitCounter = 50;

                if (weaponInfo.sender == "")
                {
                    UIHelper.Instance.QueueNotification($"You found a {weaponInfo.weaponName}");
                    return;
                }

                UIHelper.Instance.QueueNotification($"{weaponInfo.sender} sent you a {weaponInfo.weaponName}");
            }
        }

        void EquipPowerWeapon()
        {
            if (ScriptMain.SM != null)
            {
                ReceivedWeaponInfo weaponInfo = m_ReceivedPowerWeapons.Dequeue();

                ScriptMain.SM.player.martialArtStyleTarget = weaponInfo.martialArt;
                ScriptMain.SM.player.weaponHitCounter = 100;

                if (weaponInfo.sender == "")
                {
                    UIHelper.Instance.QueueNotification($"You found a {weaponInfo.weaponName}");
                    return;
                }

                UIHelper.Instance.QueueNotification($"{weaponInfo.sender} sent you a {weaponInfo.weaponName}");
            }
        }

        void EquipLegendaryWeapon()
        {
            if (ScriptMain.SM != null)
            {
                ScriptMain.SM.StartCoroutine("SpecialGlorySword");
                PlayGlobal.revenge = true;
                PlayerSave.revengeTokens++;
                m_LegendaryWeaponTriggered = false;

                string sender = m_LegendaryWeaponSenders.Dequeue();

                if (sender == "")
                {
                    UIHelper.Instance.QueueNotification($"You found a Legendary Weapon!!");
                    return;
                }

                UIHelper.Instance.QueueNotification($"{sender} has sent you a Legendary Weapon!!");
            }
        }
    }
}
