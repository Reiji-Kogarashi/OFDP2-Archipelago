using System;
using UnityEngine;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using MelonLoader;

[assembly: MelonInfo(typeof(OFDP2_Archipelago.Core), "OFDP2_Archipelago", "0.1.1", "Reiji Kogarashi", null)]
[assembly: MelonGame("Silver Dollar Games", "One Finger Death Punch 2")]

namespace OFDP2_Archipelago
{
    public class Core : MelonMod
    {
        private string m_SyncCheckFilePath = Application.persistentDataPath + "/sync_check.txt";
        private bool m_IsDesync = false;

        ApLoginUI m_LoginUI;

        public override void OnEarlyInitializeMelon()
        {
            CheckForDesync();
            BackupVanillaSavefile();
            CreateArchipelagoFactory();
        }

        private void CheckForDesync()
        {
            m_IsDesync = File.Exists(m_SyncCheckFilePath);
        }

        private void BackupVanillaSavefile()
        {
            // Backup vanilla save to avoid corruption
            string saveFileName = Application.persistentDataPath + "/OFDP2Save.dat";
            if (File.Exists(saveFileName))
            {
                string backupFileName = Application.persistentDataPath + "/OFDP2Save_Backup.dat";
                if (!File.Exists(backupFileName))
                {
                    File.Copy(saveFileName, backupFileName);
                }

                string emergencyBackupFilename = Application.persistentDataPath + "/OFDP2Save_EmergencyBackup.dat";
                if (!File.Exists(emergencyBackupFilename))
                {
                    File.Copy(saveFileName, emergencyBackupFilename, true);
                }

                File.Delete(saveFileName);
            }
        }

        private void CreateArchipelagoFactory()
        {
            ArchipelagoFactory.Create();
            ArchipelagoSaveHandler.Create();
            UIHelper.Create();
            m_LoginUI = new ApLoginUI();
        }

        public override void OnInitializeMelon()
        {
            ArchipelagoSaveHandler.Instance.Initialize();
            UIHelper.Instance.Initialize();
            m_LoginUI.Initialize();
        }

        public override void OnLateInitializeMelon()
        {
            // Create a dummy file that will be deleted on game close
            // If the file exists when we open the game, that means we're in desync.
            File.Create(m_SyncCheckFilePath);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "title" && !ArchipelagoFactory.Instance.IsLoggedIn)
            {
                m_LoginUI.ShowLoginUI();
            }

            if (sceneName == "levels")
            {
                ModdedLevelWin.IsLevelFinished = false;
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.K) && GameControl.control != null & GameControl.control.sceneName == "levels")
            {
                GameControl.control.toggleGamepadDiscovery.isOn = !GameControl.control.toggleGamepadDiscovery.isOn;
                UIHelper.Instance.SetNotificationStateToVisible();
            }

            ArchipelagoFactory.Instance.Update();
            UIHelper.Instance.Update();
        }

        public override void OnLateUpdate()
        {
            m_LoginUI.Update();
        }

        private void RestoreVanillaSave()
        {
            string saveFileName = Application.persistentDataPath + "/OFDP2Save.dat";
            string backupFileName = Application.persistentDataPath + "/OFDP2Save_Backup.dat";
            File.Copy(backupFileName, saveFileName, true);
            File.Delete(backupFileName);
        }

        public override void OnDeinitializeMelon()
        {
            ArchipelagoSaveHandler.Instance.SaveApPlayerData();
            // Restore vanilla save
            RestoreVanillaSave();
            File.Delete(m_SyncCheckFilePath);
        }
    }
}