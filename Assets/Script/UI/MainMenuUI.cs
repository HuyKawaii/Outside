using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main menu")]
    [SerializeField] private RectTransform mainMenuPanel;
    [SerializeField] private Button offlineButton;
    [SerializeField] private Button onlineButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;

    [Header("Online menu")]
    [SerializeField] private RectTransform onlineMenuPanel;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button backButton;

    private void Start()
    {
        offlineButton.onClick.AddListener(() =>
        {
            GameMenuUIManager.Instance.OpenSelectWorldMenu();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
            GameManager.Instance.isPlayingOnline = false;
        });

        onlineButton.onClick.AddListener(() =>
        {
            mainMenuPanel.gameObject.SetActive(false);
            onlineMenuPanel.gameObject.SetActive(true);
        });

        settingButton.onClick.AddListener(() => {
            GameMenuUIManager.Instance.OpenSettingMenu();
        });

        createLobbyButton.onClick.AddListener(() => {
            GameMenuUIManager.Instance.OpenSelectWorldMenu();
            GameManager.Instance.isPlayingOnline = true;
        });

        joinLobbyButton.onClick.AddListener(() => {
            GameMenuUIManager.Instance.OpenJoinLobbyMenu();
            GameManager.Instance.isPlayingOnline = true;
        });

        backButton.onClick.AddListener(() =>
        {
            mainMenuPanel.gameObject.SetActive(true);
            onlineMenuPanel.gameObject.SetActive(false);
        });

        exitGameButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
