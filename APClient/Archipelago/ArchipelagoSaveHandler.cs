using MelonLoader;
using UnityEngine;

namespace OFDP2_Archipelago
{
    public class ArchipelagoSaveHandler
    {
        private static ArchipelagoSaveHandler m_Instance = null;
        public static ArchipelagoSaveHandler Instance
        {
            get { return m_Instance; }
        }

        private string m_SessionId;
        private string m_SaveDirectory;

        private MelonPreferences_Category m_SaveCategory;
        private MelonPreferences_Entry<List<string>> m_PossessedItems;

        private bool m_ValidSave = false;

        public ArchipelagoSaveHandler() { }

        public static void Create()
        {
            Melon<Core>.Logger.Msg("Creating ArchipelagoSaveHandler");
            m_Instance = new ArchipelagoSaveHandler();
        }

        public void Initialize()
        {
            InitializeSaveStructure();
            ArchipelagoFactory.Instance.OnLoginSuccess.Subscribe(InitSessionSave);
        }

        private void InitializeSaveStructure()
        {
            m_SaveCategory = MelonPreferences.CreateCategory("ApSessionSave");

            // Create entries here
            m_PossessedItems = m_SaveCategory.CreateEntry<List<string>>("PossessedItems", null);
        }

        public void InitSessionSave()
        {
            string seed = ArchipelagoFactory.Instance.Seed;
            string slotName = ArchipelagoFactory.Instance.SlotName;

            m_SessionId = "AP_" + seed + "_" + slotName;
            m_SaveDirectory = Application.persistentDataPath + "/ApSession/" + m_SessionId;
            string saveFilename = m_SaveDirectory + "/" + m_SessionId + ".cfg";
            string ofdp2savename = m_SaveDirectory + "/" + m_SessionId + ".dat";

            if (!Directory.Exists(m_SaveDirectory))
            {
                Directory.CreateDirectory(m_SaveDirectory);

                ModdedDataHelper.InitNewPlayerSave();
            }
            else if (File.Exists(ofdp2savename))
            {
                File.Copy(ofdp2savename, Application.persistentDataPath + "/OFDP2Save.dat", true);

                GameControl.control.Load();

                Melon<Core>.Logger.Msg("DAT file loaded");
            }

            m_SaveCategory.SetFilePath(saveFilename);

            Melon<Core>.Logger.Msg($"Setting session savefile {saveFilename}");

            LoadSessionData();

            m_ValidSave = true;
        }

        private void LoadSessionData()
        {
            
        }

        public void SaveApPlayerData()
        {
            if (!m_ValidSave)
            {
                return;
            }

            string saveFileName = Application.persistentDataPath + "/OFDP2Save.dat";
            string apSaveFilename = m_SaveDirectory + "/" + m_SessionId + ".dat";

            File.Copy(saveFileName, apSaveFilename, true);
        }
    }
}
