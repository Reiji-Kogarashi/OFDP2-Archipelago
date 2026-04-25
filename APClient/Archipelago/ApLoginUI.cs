using MelonLoader;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OFDP2_Archipelago
{
    public class ApLoginUI
    {
        GameObject m_LoginPopup;
        Transform m_LoginCanvas;

        InputField m_HostAddressField;
        InputField m_SlotNameField;
        InputField m_PasswordField;

        Button m_DeathLinkButton;
        Button m_LoginButton;

        Image m_DeathLinkButtonImage;
        TMP_Text m_DeathLinkButtonText;

        bool m_DeathLinkValue = false;

        public ApLoginUI() { }

        public void Initialize()
        {
            ArchipelagoFactory.Instance.OnLoginSuccess.Subscribe(OnLoginSuccessful);
        }

        public void ShowLoginUI()
        {
            if (ScriptMainTitle.M != null)
            {
                GameObject twitchPanel = ScriptMainTitle.M.twitchPanel;
                m_LoginPopup = GameObject.Instantiate(twitchPanel, twitchPanel.transform.parent);
                m_LoginPopup.name = "Archipelago Login Panel";
                m_LoginPopup.SetActive(true);

                m_LoginCanvas = m_LoginPopup.transform.GetChild(0);

                Transform hostAddressTransform = m_LoginCanvas.Find("InputField");

                if (hostAddressTransform)
                {
                    string [] previousLoginInfo = new string[] { "archipelago.gg:38281", "Slot Name", "False" };

                    if (File.Exists(Application.persistentDataPath + "/previous_login.txt"))
                    {
                        previousLoginInfo = File.ReadAllLines(Application.persistentDataPath + "/previous_login.txt");
                    }

                    m_HostAddressField = hostAddressTransform.GetComponent<InputField>();
                    m_HostAddressField.placeholder.GetComponent<Text>().text = "archipelago.gg:38281";
                    

                    GameObject fieldToClone = hostAddressTransform.gameObject;
                    m_SlotNameField = GameObject.Instantiate<InputField>(m_HostAddressField, m_LoginCanvas);
                    m_SlotNameField.name = "SlotNameInputField";
                    m_SlotNameField.transform.localPosition = new Vector3(-37.5f, 26f, 0f);
                    m_SlotNameField.placeholder.GetComponent<Text>().text = "Slot Name";


                    m_PasswordField = GameObject.Instantiate<InputField>(m_HostAddressField, m_LoginCanvas);
                    m_PasswordField.name = "PasswordInputField";
                    m_PasswordField.transform.localPosition = new Vector3(-37.5f, -63.68f, 0f);
                    m_PasswordField.placeholder.GetComponent<Text>().text = "Password";

                    Transform loginButtonTransform = m_LoginCanvas.Find("Button");

                    if (loginButtonTransform)
                    {
                        m_LoginButton = loginButtonTransform.GetComponent<Button>();

                        m_DeathLinkButton = GameObject.Instantiate<Button>(m_LoginButton, m_LoginCanvas);
                        m_DeathLinkButton.name = "DeathLinkButton";
                        m_DeathLinkButtonImage = m_DeathLinkButton.GetComponent<Image>();
                        m_DeathLinkButtonText = m_DeathLinkButton.GetComponentInChildren<TMP_Text>();

                        m_DeathLinkButton.onClick.AddListener(OnToggleDeathLink);

                        Button quitButton = GameObject.Instantiate<Button>(m_LoginButton, m_LoginCanvas);
                        quitButton.name = "QuitButton";
                        quitButton.transform.localPosition = new Vector3(226.74f, -63.68f, 0f);
                        Image quitButtonImage = quitButton.GetComponent<Image>();
                        TMP_Text quitButtonText = quitButton.GetComponentInChildren<TMP_Text>();

                        quitButtonImage.color = Color.grey;
                        quitButtonText.text = "Exit game";

                        quitButton.onClick.AddListener(OnQuit);

                        m_LoginButton.transform.localPosition = new Vector3(226.74f, 27.294f, 0f);
                        m_LoginButton.onClick.AddListener(OnValidate);
                    }

                    m_LoginCanvas.Find("Emotes").gameObject.SetActive(false);
                    m_LoginCanvas.Find("Fight Me").gameObject.SetActive(false);

                    // Data prefill
                    m_HostAddressField.text = previousLoginInfo[0];
                    m_SlotNameField.text = previousLoginInfo[1];
                    SetDeathLinkValue(previousLoginInfo[2] == "True");
                }
            }
            else
            {
                Melon<Core>.Logger.Warning("ScriptMainTitle instance does not exist yet");
            }
        }

        public void Update()
        {
            if (m_LoginPopup && m_LoginPopup.activeSelf)
            {
                GameControl.control.toggleGamepadDiscovery.isOn = false;
                ScriptMainTitle.M.director.Stop();
                GameControl.control.showOffTimer = 60f;
                PlayGlobal.engagement = true;
            }
        }

        private void OnToggleDeathLink()
        {
            SetDeathLinkValue(!m_DeathLinkValue);
        }

        private void SetDeathLinkValue(bool value)
        {
            m_DeathLinkValue = value;
            if (m_DeathLinkButtonImage != null)
            {
                m_DeathLinkButtonImage.color = m_DeathLinkValue ? Color.red : Color.grey;
            }
            if (m_DeathLinkButtonText != null)
            {
                m_DeathLinkButtonText.text = "Death Link: " + (m_DeathLinkValue ? "ON" : "OFF");
            }
        }

        private void OnValidate()
        {
            string hostValue = m_HostAddressField.text;
            string slotNameValue = m_SlotNameField.text;
            string passwordValue = m_PasswordField.text;

            ArchipelagoFactory.Instance.Connect(hostValue, slotNameValue, passwordValue, m_DeathLinkValue);
        }

        private void OnLoginSuccessful()
        {
            string path = Application.persistentDataPath + "/previous_login.txt";
            File.WriteAllLines(path, new string[] { m_HostAddressField.text, m_SlotNameField.text, m_DeathLinkValue.ToString() });

            if (m_LoginPopup)
            {
                GameObject.Destroy(m_LoginPopup);

                m_LoginPopup = null;
            }

            PlayGlobal.engagement = false;
            GameControl.control.toggleGamepadDiscovery.isOn = true;

            if (ScriptMainTitle.M.gameobjectFight.activeSelf)
            {
                ScriptMainTitle.M.director.Play();
            }
        }

        private void OnQuit()
        {
            Application.Quit();
        }
    }
}
