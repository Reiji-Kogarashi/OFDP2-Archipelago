using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using MelonLoader;
using System.Collections.ObjectModel;
using UnityEngine;

namespace OFDP2_Archipelago
{
    public class ArchipelagoFactory
    {
        private static ArchipelagoFactory m_Instance = null;

        public static ArchipelagoFactory Instance
        {
            get { return m_Instance; }
        }

        // Game Info
        private const string GAME_NAME = "One Finger Death Punch 2";

        // Item Handler
        private ApItemHandler m_ItemHandler = null;

        // Connected Session
        private ArchipelagoSession m_Session = null;
        private bool m_IsLoggedIn = false;

        // Slot Data
        private Dictionary<string, object> m_SlotData;

        // Death Link
        private DeathLinkService m_DeathLinkService = null;
        private bool m_DeathLinkTriggered = false;
        private float m_DeathLinkCooldown = 1f;
        private float m_CurrentDeathLinkTimer = 0f;

        // Events
        public MelonEvent OnLoginSuccess = new MelonEvent();

        // Getters
        public bool IsLoggedIn
        { 
            get { return m_IsLoggedIn; }
        }

        public string Seed
        {
            get 
            {
                if (m_Session == null)
                    return "";

                return m_Session.RoomState.Seed; 
            }
        }

        public string SlotName
        {
            get
            {
                if (m_Session == null)
                    return "";

                return m_Session.Players.ActivePlayer.Name;
            }
        }

        public string PlayerAlias
        {
            get
            {
                if (m_Session == null)
                    return "";

                return m_Session.Players.ActivePlayer.Alias;
            }
        }

        public ArchipelagoFactory() { }

        public static void Create() 
        {
            Melon<Core>.Logger.Msg("Creating ArchipelagoFactory");
            m_Instance = new ArchipelagoFactory();
            
            if (m_Instance != null)
            {
                m_Instance.m_ItemHandler = new ApItemHandler();
            }
        }

        public void Deinitialize()
        {
            Melon<Core>.Logger.Msg("Deactivating ArchipelagoFactory");
            m_Instance = null;
            m_ItemHandler = null;
        }

        public void Connect(string hostAddress, string playerSlotName, string password, bool enableDeathLink)
        {
            m_Session = ArchipelagoSessionFactory.CreateSession(hostAddress);

            m_DeathLinkService = m_Session.CreateDeathLinkService();

            // Events subsription
            m_Session.Socket.SocketClosed += Socket_SocketClosed;
            m_Session.Socket.ErrorReceived += Socket_ErrorReceived;

            m_Session.Items.ItemReceived += OnReceiveItem;
            m_Session.Locations.CheckedLocationsUpdated += OnUpdateCheckedLocations;

            if (m_DeathLinkService != null)
            {
                m_DeathLinkService.OnDeathLinkReceived += OnDeathLink;
            }
            

            LoginResult result;

            try
            {
                string[] tags = null;

                if (enableDeathLink)
                {
                    tags = new string[] { "DeathLink" };
                }

                result = m_Session.TryConnectAndLogin(GAME_NAME, playerSlotName, ItemsHandlingFlags.AllItems, null, tags, null, password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                

                Melon<Core>.Logger.Error(errorMessage);
                return; // Did not connect, show the user the contents of `errorMessage`
            }

            LoginSuccessful success = (LoginSuccessful)result;

            m_SlotData = success.SlotData;

            m_IsLoggedIn = true;
            Melon<Core>.Logger.Msg("Login SUCCESS");
            m_ItemHandler.Activate();

            OnLoginSuccess.Invoke();
        }

        private void Socket_SocketClosed(string reason)
        {
            Melon<Core>.Logger.Msg($"Socket closed. Reason: {reason}");
        }

        private void Socket_ErrorReceived(Exception e, string message)
        {
            string errorDetails = $"Socket error: {message}";
            if (e != null)
            {
                errorDetails += $"\nException: {e.GetType().FullName}";
                errorDetails += $"\nMessage: {e.Message}";
                errorDetails += $"\nStackTrace: {e.StackTrace}";
                if (e.InnerException != null)
                {
                    errorDetails += $"\nInnerException: {e.InnerException.GetType().FullName}";
                    errorDetails += $"\nInnerException Message: {e.InnerException.Message}";
                    errorDetails += $"\nInnerException StackTrace: {e.InnerException.StackTrace}";
                }
            }
            Melon<Core>.Logger.Error(errorDetails);
        }

        public void Update()
        {
            m_ItemHandler.Update();

            if (m_DeathLinkTriggered)
            {
                m_DeathLinkTriggered = false;

                if (m_CurrentDeathLinkTimer <= 0f && ScriptMain.SM != null)
                {
                    m_CurrentDeathLinkTimer = m_DeathLinkCooldown;
                    ScriptMain.SM.PlayerDie(ScriptMain.SM.player.actor.transform.position.x);
                }
            }

            if (m_CurrentDeathLinkTimer > 0f)
            {
                m_CurrentDeathLinkTimer -= Time.unscaledDeltaTime;
            }
        }

        private void OnReceiveItem(ReceivedItemsHelper receivedItemsHelper)
        {
            if (m_ItemHandler != null)
            {
                ItemInfo receivedItem = receivedItemsHelper.DequeueItem();
                m_ItemHandler.ReceiveItem(receivedItem);
            }
        }

        public void ReportCheckCompletion(string locationName)
        {
            long locationId = m_Session.Locations.GetLocationIdFromName(GAME_NAME, locationName);

            Melon<Core>.Logger.Msg($"Attempting to send check for location {locationName} (ID: {locationId})");

            m_Session.Locations.CompleteLocationChecks(locationId);

            NotifySentItem(locationId);
        }

        private async void NotifySentItem(long locationId)
        {
            Dictionary<long, ScoutedItemInfo> scoutedItems = await m_Session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, locationId);

            if (scoutedItems != null)
            {
                foreach (KeyValuePair<long, ScoutedItemInfo> item in scoutedItems)
                {
                    Melon<Core>.Logger.Msg($"ID: {item.Key}; Item: {item.Value.ItemName}; For: {item.Value.Player}");

                    if (item.Value.Player != null && item.Value.Player.Name != m_Session.Players.ActivePlayer.Name)
                    {
                        UIHelper.Instance.QueueNotification($"You sent {item.Value.ItemDisplayName} to {item.Value.Player.Alias}");
                    }
                }
            }
        }

        public void ReportGoal()
        {
            m_Session.SetGoalAchieved();

            UIHelper.Instance.QueueNotification("You reached your goal of beating everyone to death!", true);
        }

        private void OnDeathLink(DeathLink deathLinkObject)
        {
            m_DeathLinkTriggered = true;

            string message = (deathLinkObject.Cause != null ? deathLinkObject.Cause : $"{deathLinkObject.Source} died");

            UIHelper.Instance.QueueNotification(message, true);
        }

        public void ReportDeathLink(string reason)
        {
            if (m_DeathLinkService != null && m_CurrentDeathLinkTimer <= 0f)
            {
                m_DeathLinkService.SendDeathLink(new DeathLink(m_Session.Players.ActivePlayer.Name, reason));
                m_CurrentDeathLinkTimer = m_DeathLinkCooldown;
            }
        }

        public object GetSlotData(string key)
        {
            if (m_SlotData != null && m_SlotData.ContainsKey(key))
            {
                return m_SlotData[key];
            }

            return null;
        }

        public void OnUpdateCheckedLocations(ReadOnlyCollection<long> newCheckedLocationsId)
        {
            foreach (long locationId in newCheckedLocationsId)
            {
                string locationName = m_Session.Locations.GetLocationNameFromId(locationId, GAME_NAME);
                ApPlayerData.completedLocations.Add(locationName);
            }
        }
    }
}
