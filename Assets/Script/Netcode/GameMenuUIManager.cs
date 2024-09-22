using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUIManager : MonoBehaviour
{
    public static GameMenuUIManager Instance;
    [Header("Menu panel")]
    [SerializeField] private RectTransform menuPanel;
    

    [Header("Setting panel")]
    [SerializeField] private RectTransform settingPanel;

    [Header("Select world panel")]
    [SerializeField] private RectTransform selectWorldPanel;

    [Header("Create world panel")]
    [SerializeField] private RectTransform createWorldPanel;

    [Header("Select character panel")]
    [SerializeField] private RectTransform selectCharacterPanel;

    [Header("Create character panel")]
    [SerializeField] private RectTransform createCharacterPanel;

    [Header("Join room panel")]
    [SerializeField] private RectTransform joinLobbyPanel;

    [Header("Join room panel")]
    [SerializeField] private RectTransform lobbyPanel;

    [Header("Loading screen")]
    [SerializeField] private RectTransform loadingScreen;

    [Header("Error Message")]
    [SerializeField] private RectTransform errorMessagePanel;
    [SerializeField] private Button closeErrorMessageButton;
    [SerializeField] private TextMeshProUGUI errorMessage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        closeErrorMessageButton.onClick.AddListener(() =>
        {
            errorMessagePanel.gameObject.SetActive(false);
        });
        
    }

    private void Start()
    {
        joinLobbyPanel.gameObject.SetActive(false);

        LobbyManager.Instance.onLobbyCreatedCallback += DisplayLobby;

        LobbyManager.Instance.onInitialConnectionComplete += CloseLoadingScreen;

        LobbyManager.Instance.onClientCharacterCreationComplete += DisplayLobbyForClient;
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyCreatedCallback -= DisplayLobby;

        LobbyManager.Instance.onInitialConnectionComplete -= CloseLoadingScreen;

        LobbyManager.Instance.onClientCharacterCreationComplete -= DisplayLobbyForClient;
    }

    public void OpenMainMenu()
    {
        menuPanel.gameObject.SetActive(true);
        CloseLobbyMenu();
    }

    public void CloseMainMenu()
    {
        menuPanel.gameObject.SetActive(false);
    }

    public void OpenSelectWorldMenu()
    {
        selectWorldPanel.gameObject.SetActive(true);
    }

    public void CloseSelectWorldMenu()
    {
        selectWorldPanel.gameObject.SetActive(false);
    }

    public void OpenCreateWorldMenu()
    {
        createWorldPanel.gameObject.SetActive(true);
    }

    public void CloseCreateWorldMenu()
    {
        createWorldPanel.gameObject.SetActive(false);
    }

    public void OpenJoinLobbyMenu()
    {
        joinLobbyPanel.gameObject.SetActive(true);
    }

    public void CloseJoinLobbyMenu()
    {
        joinLobbyPanel.gameObject.SetActive(false);
    }

    public void OpenSettingMenu()
    {
        settingPanel.gameObject.SetActive(true);
    }

    public void CloseSettingMenu()
    {
        settingPanel.gameObject.SetActive(false);
    }
   
    public void OpenSelectCharacterMenu()
    {
        selectCharacterPanel.gameObject.SetActive(true);
    }

    public void CloseSelectCharacterMenu()
    {
        selectCharacterPanel.gameObject.SetActive(false);
    }

    public void OpenCreateCharacterMenu()
    {
        createCharacterPanel.gameObject.SetActive(true);
    }

    public void CloseCreateCharacterMenu()
    {
        createCharacterPanel.gameObject.SetActive(false);
    }

    private void CreateRoomFail()
    {
        menuPanel.gameObject.SetActive(true);
        DisplayErrorMessage("Can not create room.\nTry again latter.");
    }

    public void DisplayErrorMessage(string message)
    {
        errorMessagePanel.gameObject.SetActive(true);
        errorMessage.text = message;
    }

    public void DisplayLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
    }

    public void CloseLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }

    private void DisplayLobby(string roomCode = "")
    {
        CloseLoadingScreen();
        CloseMainMenu();
        OpenLobbyMenu();
    }

    private void DisplayLobbyForClient()
    {
        DisplayLobby();
    }

    private void OpenLobbyMenu()
    {
        lobbyPanel.gameObject.SetActive(true);
    }

    private void CloseLobbyMenu()
    {
        lobbyPanel.gameObject.SetActive(false);
    }
}
