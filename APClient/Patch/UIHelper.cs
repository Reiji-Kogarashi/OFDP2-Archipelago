using MelonLoader;
using System.Collections.Generic;
using UnityEngine;

namespace OFDP2_Archipelago
{
    public class UIHelper
    {
        private static UIHelper m_Instance;
        public static UIHelper Instance
        {
            get { return m_Instance; }
        }

        private Queue<string> m_NotificationQueue;

        private float m_Duration = 5f;
        private float m_CurrentMessageVisibilityTime = 0f;

        private float m_DelayBeforeFirstActivation = 1f;
        private float m_CurrentActivationTime = 0f;

        private bool m_IsPriorityMessage = false;
        private float m_PriorityMessageTimer = 0f;

        private bool m_IsNotificationVisible = false;

        private bool m_IsActive = false;

        public UIHelper()
        {
            Melon<Core>.Logger.Msg("Creating UIHelper");
            m_NotificationQueue = new Queue<string>();
        }

        public static void Create()
        {
            m_Instance = new UIHelper();
        }

        public void Initialize()
        {
            Melon<Core>.Logger.Msg("Initializing UIHelper");
            ArchipelagoFactory.Instance.OnLoginSuccess.Subscribe(Activate);
        }

        public void Activate()
        {
            m_IsActive = true;
        }

        public void QueueNotification(string message, bool isPriority = false)
        {
            if (m_CurrentActivationTime >= m_DelayBeforeFirstActivation)
            {
                m_NotificationQueue.Enqueue(message);
                m_IsPriorityMessage = true;
            }
        }

        public void Update()
        {
            if (!m_IsActive)
                return;

            DeactivateControlLocks();

            if (m_CurrentActivationTime < m_DelayBeforeFirstActivation)
            {
                m_CurrentActivationTime += Time.unscaledDeltaTime;
                return;
            }

            if (m_PriorityMessageTimer > 0f)
            {
                m_PriorityMessageTimer -= Time.unscaledDeltaTime;
                return;
            }

            if (m_NotificationQueue.Count <= 0 || GameControl.control == null || GameControl.control.sceneName == "" || GameControl.control.sceneName == "not a story")
                return;

            GameControl.control.CreateTextPanel(new List<string> { "THIS TEXT WON'T WORK. USE SETTEXT() BELOW" }, "Archipelago Notification", m_Duration, "", "", pause: false, haltEnemies: false, highlightAccept: false, new List<string> { "" });
            GameControl.control.textPanel.textText[0].SetText(m_NotificationQueue.Dequeue());

            SetNotificationStateToVisible();

            if (m_IsPriorityMessage)
            {
                m_IsPriorityMessage = false;
                m_PriorityMessageTimer = m_Duration;
            }
        }

        private void DeactivateControlLocks()
        {
            // This function is important because in vanilla, the notifications disables all controls from the player.

            if (!m_IsNotificationVisible || GameControl.control == null)
                return;

            GameControl.control.textPanel.active = false;

            m_CurrentMessageVisibilityTime += Time.unscaledDeltaTime;

            if (m_CurrentMessageVisibilityTime >= m_Duration)
            {
                m_IsNotificationVisible = false;
                m_CurrentMessageVisibilityTime = 0f;
            }
            
        }

        public void SetNotificationStateToVisible()
        {
            m_IsNotificationVisible = true;
            m_CurrentMessageVisibilityTime = 0f;
        }
    }
}
