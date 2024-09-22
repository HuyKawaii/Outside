using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterMenuUI : MonoBehaviour
{
    private int selectedCharacterIndexUI;
    [SerializeField] private RectTransform selectCharacterPanel;
    [SerializeField] private RectTransform characterListContentUI;
    [SerializeField] private GameObject characterListEntryUI;
    [SerializeField] private Button closeSelectCharacterButton;
    [SerializeField] private Button selectCharacterButton;
    [SerializeField] private Button deleteCharacterButton;
    [SerializeField] private Button newCharacterButton;

    public delegate void CharacterListEntrySelectCallback(int selectedIndex);
    public CharacterListEntrySelectCallback onCharacterListEntrySelectCallback;
    private void Start()
    {
        selectCharacterPanel.gameObject.SetActive(false);

        closeSelectCharacterButton.onClick.AddListener(() =>
        {
            GameMenuUIManager.Instance.CloseSelectCharacterMenu();
            if (NetworkManager.Singleton.IsClient)
                LobbyManager.Instance.ExitLobby();
        });

        selectCharacterButton.onClick.AddListener(() =>
        {
            OnCharacterSelected();
        });

        deleteCharacterButton.onClick.AddListener(() =>
        {
            DeleteCharacterUI();
        });

        newCharacterButton.onClick.AddListener(() =>
        {
            GameMenuUIManager.Instance.CloseSelectCharacterMenu();
            GameMenuUIManager.Instance.OpenCreateCharacterMenu();
        });

        SaveSelectionManager.Instance.onWorldSelectedCallback += () =>
        {
            InstantiateCharacterListEntries(SaveSelectionManager.Instance.GetSelectedWorld().playerDataArray);
        };

        GameManager.Instance.onSetGameDataForClient += () =>
        {
            InstantiateCharacterListEntries(GameManager.Instance.playerDataArray);
            GameMenuUIManager.Instance.OpenSelectCharacterMenu();
        };

        SaveSelectionManager.Instance.onCharacterDeletedCallback += () =>
        {
            InstantiateCharacterListEntries(SaveSelectionManager.Instance.GetSelectedWorld().playerDataArray);
        };
    }

    private void OnDestroy()
    {
        SaveSelectionManager.Instance.onWorldSelectedCallback = null;
        GameManager.Instance.onSetGameDataForClient = null;
        SaveSelectionManager.Instance.onCharacterDeletedCallback = null;
    }

    

    public void OnCharacterListEntrySelected(int saveIndex)
    {
        selectedCharacterIndexUI = saveIndex;
        if (onCharacterListEntrySelectCallback != null)
        {
            onCharacterListEntrySelectCallback(saveIndex);
        }
    }

    private void OnCharacterSelected()
    {
        if (selectedCharacterIndexUI == -1)
            return;

        if (!NetworkManager.Singleton.IsClient)
        {
            GameMenuUIManager.Instance.CloseSelectCharacterMenu();
            SaveSelectionManager.Instance.SetSelectedCharacterIndex(selectedCharacterIndexUI);
            GameMenuUIManager.Instance.DisplayLoadingScreen();

            GameManager.Instance.CreateGameFromSave();
        }
        else
        {
            GameMenuUIManager.Instance.CloseSelectCharacterMenu();
            GameManager.Instance.SetCharacterForClient(selectedCharacterIndexUI);
        }
       
    }

    private void InstantiateCharacterListEntries(PlayerData[] playerDatas)
    {
        foreach (Transform child in characterListContentUI)
        {
            onCharacterListEntrySelectCallback -= child.GetComponent<SaveListEntryUI>().NewEntryGotSelected;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerDatas.Length; i++)
        {
            GameObject saveListEntry = Instantiate(characterListEntryUI, characterListContentUI);
            onCharacterListEntrySelectCallback += saveListEntry.GetComponent<SaveListEntryUI>().NewEntryGotSelected;
            saveListEntry.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnCharacterListEntrySelected(saveListEntry.GetComponent<SaveListEntryUI>().index);
            });
            saveListEntry.GetComponent<SaveListEntryUI>().SetIndex(i);
            saveListEntry.GetComponent<SaveListEntryUI>().SetName(playerDatas[i].playerName.ToString());
        }
    }

    private void DeleteCharacterUI()
    {
        if (selectedCharacterIndexUI == -1)
            return;
        ConfirmationWindowUI.OpenConfirmationWindow(ConstContainer.DeleteWorldConfirmationMessage, () => { OnConfirmDeleteCharacter(); });
    }

    private void OnConfirmDeleteCharacter()
    {
        SaveSelectionManager.Instance.DeleteCharacterData(selectedCharacterIndexUI);
    }

    public void ResetUI()
    {
        selectedCharacterIndexUI = -1;

        if (!NetworkManager.Singleton.IsClient)
        {
            deleteCharacterButton.gameObject.SetActive(true);
        }
        else
        {
            deleteCharacterButton.gameObject.SetActive(false);
        }
    }
}
