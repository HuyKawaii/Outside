using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        gameoverPanel.SetActive(false);
    }

    private void Start()
    {
        PlayerManager.Instance.player.GetComponent<PlayerCombat>().onPlayerDieCallback += GameOver;
        respawnButton.onClick.AddListener(GameOver);
        exitButton.onClick.AddListener(() =>
        {
            ConfirmationWindowUI.OpenConfirmationWindow("Are you sure you want to exit ?", GameManager.Instance.ExitGame);
        });
        respawnButton.onClick.AddListener(Respawn);
    }

    private void GameOver()
    {
        gameoverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void Respawn()
    {
        PlayerManager.Instance.RespawnPlayer();
        gameoverPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
