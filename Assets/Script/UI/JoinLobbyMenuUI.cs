using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private Button closeJoinLobbyButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Button confirmJoinLobbyButton;

    void Start()
    {
        closeJoinLobbyButton.onClick.AddListener(() =>
        {
            GameMenuUIManager.Instance.CloseJoinLobbyMenu();
        });
        confirmJoinLobbyButton.onClick.AddListener(() =>
        {
            OnJoinGameButtonClick(lobbyCodeInput.text);
        });
        lobbyCodeInput.onValueChanged.AddListener((string newValue) =>
        {
            lobbyCodeInput.text = newValue.ToUpper();
        });
    }

    private async void OnJoinGameButtonClick(string gameCode)
    {
        GameMenuUIManager.Instance.DisplayLoadingScreen();
        GameMenuUIManager.Instance.CloseJoinLobbyMenu();
        
        if (!await LobbyManager.Instance.JoinGameAsClient(gameCode))
        {
            JoinRoomFail();
            return;
        }
    }

    private void JoinRoomFail()
    {
        GameMenuUIManager.Instance.CloseLoadingScreen();
        GameMenuUIManager.Instance.OpenMainMenu();
        GameMenuUIManager.Instance.DisplayErrorMessage("Can not join room.\nWrong code or try again later.");
    }

}
