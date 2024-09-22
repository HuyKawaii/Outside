using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform inGameMenuPanel;
    [SerializeField] private Button saveGameButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button closeInGameMenuButton;
    [SerializeField] private RectTransform settingMenuPanel;

    private void Start()
    {
        closeInGameMenuButton.onClick.AddListener(() =>
        {
            inGameMenuPanel.gameObject.SetActive(false);
        });

        saveGameButton.onClick.AddListener(() =>
        {
            GameSavingManager.Instance.SaveGameData();
        });

        settingButton.onClick.AddListener(() =>
        {
            settingMenuPanel.gameObject.SetActive(true);
        });

        exitGameButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ExitGame();
        });

        inGameMenuPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inGameMenuPanel.gameObject.SetActive(!inGameMenuPanel.gameObject.activeSelf);

            if (inGameMenuPanel.gameObject.activeSelf)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
