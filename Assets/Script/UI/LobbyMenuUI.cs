using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveLobbyButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
        });
        leaveLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.ExitLobby();
        });

        LobbyManager.Instance.onLobbyCreatedCallback += (string roomCode) =>
        {
            if (roomCode != "")
            {
                lobbyCodeText.text = "Room code: " + roomCode;
                lobbyCodeText.gameObject.SetActive(true);
            }
            else
                lobbyCodeText.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
        };

        LobbyManager.Instance.onLobbyJoinCallback += (string roomCode) =>
        {
            lobbyCodeText.text = "Room code: " + roomCode;
            startGameButton.gameObject.SetActive(false);
        };
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.onLobbyCreatedCallback = null;
        LobbyManager.Instance.onLobbyJoinCallback = null;
    }
}
