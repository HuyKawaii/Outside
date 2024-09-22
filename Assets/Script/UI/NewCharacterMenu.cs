using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterMenu : MonoBehaviour
{
    [SerializeField] private RectTransform createNewCharacterPanel;
    [SerializeField] private Button createNewCharacterButton;
    [SerializeField] private Button closeNewCharacterMenuButton;
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Start()
    {
        closeNewCharacterMenuButton.onClick.AddListener(() =>
        {
            createNewCharacterPanel.gameObject.SetActive(false);
            GameMenuUIManager.Instance.OpenSelectCharacterMenu();
        });
        createNewCharacterButton.onClick.AddListener(() =>
        {
            OnCharacterCreated();
           
        });
    }

    private void OnCharacterCreated()
    {
        if (playerNameInputField.text == "")
            return;

        if (!SaveSelectionManager.Instance.CheckCharacterName(playerNameInputField.text))
        {
            GameMenuUIManager.Instance.DisplayErrorMessage("Character name existed. Choose a different name");
            return;
        }

        if (!NetworkManager.Singleton.IsClient)
        {
            createNewCharacterPanel.gameObject.SetActive(false);
            GameMenuUIManager.Instance.DisplayLoadingScreen();

            GameManager.Instance.CreateGameFromSave(playerNameInputField.text);
        }
        else
        {
            createNewCharacterPanel.gameObject.SetActive(false);
            GameMenuUIManager.Instance.DisplayLoadingScreen();

            GameManager.Instance.ClientCreateNewCharacter(playerNameInputField.text);
        }
    }
}
