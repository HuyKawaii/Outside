using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectWorldMenuUI : MonoBehaviour
{
    private int selectedWorldIndexUI;
    [SerializeField] private GameObject worldListEntryUI;
    [SerializeField] private RectTransform worldListContentUI;
    [SerializeField] private Button closeSelectWorldButton;
    [SerializeField] private Button selectWorldButton;
    [SerializeField] private Button deleteWorldButton;
    [SerializeField] private Button newWorldButton;


    public delegate void WorldListEntrySelectCallback(int selectedIndex);
    public WorldListEntrySelectCallback onWorldListEntrySelectCallback;
    private void Start()
    {
        closeSelectWorldButton.onClick.AddListener(() =>
        {
            GameMenuUIManager.Instance.CloseSelectWorldMenu();
        });

        selectWorldButton.onClick.AddListener(() =>
        {
            OnWorldSelected();
        });

        newWorldButton.onClick.AddListener(() =>
        {
            CreateNewWorldUI();
        });

        deleteWorldButton.onClick.AddListener(() =>
        {
            DeleteWorldUI();
        });

        InstantiateWorldListEntries();

        SaveSelectionManager.Instance.onDataLoadedCallback += InstantiateWorldListEntries;

        GameMenuUIManager.Instance.CloseSelectWorldMenu();
    }

    private void OnEnable()
    {
        OnWorldListEntrySelected(-1);
    }

    private void OnDestroy()
    {
        SaveSelectionManager.Instance.onDataLoadedCallback -= InstantiateWorldListEntries;
    }

    public void OnWorldListEntrySelected(int saveIndex)
    {
        selectedWorldIndexUI = saveIndex;
        if (onWorldListEntrySelectCallback != null)
        {
            onWorldListEntrySelectCallback(saveIndex);
        }
    }

    private void CreateNewWorldUI()
    {
        GameMenuUIManager.Instance.CloseSelectWorldMenu();
        GameMenuUIManager.Instance.OpenCreateWorldMenu();
    }

    private void DeleteWorldUI()
    {
        if (selectedWorldIndexUI == -1)
            return;
        ConfirmationWindowUI.OpenConfirmationWindow(ConstContainer.DeleteWorldConfirmationMessage, () => { OnConfirmDeleteWorld();  });
    }

    private void OnConfirmDeleteWorld()
    {
        SaveSelectionManager.Instance.DeleteGameData(selectedWorldIndexUI);
    }

    private void OnWorldSelected()
    {
        if (selectedWorldIndexUI == -1)
            return;
        GameMenuUIManager.Instance.CloseSelectWorldMenu();
        SaveSelectionManager.Instance.SetSelectedWorldIndex(selectedWorldIndexUI);
        GameMenuUIManager.Instance.OpenSelectCharacterMenu();
    }

    private void InstantiateWorldListEntries()
    {
        //Debug.Log("Instatiating world list");
        foreach (Transform child in worldListContentUI)
        {
            onWorldListEntrySelectCallback -= child.GetComponent<SaveListEntryUI>().NewEntryGotSelected;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < SaveSelectionManager.Instance.dataList.Count; i++)
        {
            GameObject saveListEntry = Instantiate(worldListEntryUI, worldListContentUI);
            onWorldListEntrySelectCallback += saveListEntry.GetComponent<SaveListEntryUI>().NewEntryGotSelected;
            saveListEntry.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnWorldListEntrySelected(saveListEntry.GetComponent<SaveListEntryUI>().index);
            });
            saveListEntry.GetComponent<SaveListEntryUI>().SetIndex(i);
            saveListEntry.GetComponent<SaveListEntryUI>().SetName(SaveSelectionManager.Instance.dataList[i].GetWorldName());
        }
    }
}
