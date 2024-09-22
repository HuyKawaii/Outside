using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SaveSelectionManager : MonoBehaviour
{
    #region || ----- Singleton ----- ||
    public static SaveSelectionManager Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public delegate void SaveSelectionCallback();
    public SaveSelectionCallback onWorldSelectedCallback;
    public SaveSelectionCallback onDataLoadedCallback;
    public SaveSelectionCallback onCharacterDeletedCallback;

    public int selectedWorldIndex {  get; private set; }
    public int selectedCharacterIndex {  get; private set; }
    public List<GameData> dataList;

    private void Start()
    {
        selectedWorldIndex = -1;
        LoadGameDataToSelection();
    }

    public void SetSelectedWorldIndex(int index)
    {
        this.selectedWorldIndex = index;
        if (onWorldSelectedCallback != null)
            onWorldSelectedCallback();
    }

    public void SetSelectedCharacterIndex(int index)
    {
        this.selectedCharacterIndex = index;
    }

    public GameData GetSelectedWorld()
    {
        if (selectedWorldIndex >= 0)
            return dataList[selectedWorldIndex];
        return null;
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }

    public PlayerData GetSelectedPlayerData()
    {
        if (selectedWorldIndex >= 0 && selectedCharacterIndex >= 0)
            return dataList[selectedWorldIndex].playerDataArray[selectedCharacterIndex];
        return PlayerData.Empty;
    }

    public void DeleteGameData(int gameDataIndex)
    {
        GameSavingManager.Instance.DeleteGameData(dataList[gameDataIndex]);
        LoadGameDataToSelection();
        if (onDataLoadedCallback != null)
            onDataLoadedCallback();
    }

    public void DeleteCharacterData(int chracterDataIndex)
    {
        GameSavingManager.Instance.DeleteChracterData(dataList[selectedWorldIndex], dataList[selectedWorldIndex].playerDataArray[chracterDataIndex]);
        PlayerData[] selectedWorldPlayerData = dataList[selectedWorldIndex].playerDataArray;
        PlayerData[] newPlayerData = new PlayerData[selectedWorldPlayerData.Length-1];
        int count = 0;
        for (int i = 0; i < selectedWorldPlayerData.Length; i++)
        {
            if (i != chracterDataIndex)
            {
                newPlayerData[count++] = selectedWorldPlayerData[i];
            }
        }
        dataList[selectedWorldIndex].playerDataArray = newPlayerData;

        if (onCharacterDeletedCallback != null)
            onCharacterDeletedCallback();
    }

    public void LoadGameDataToSelection()
    {
        dataList = GameSavingManager.Instance.LoadAllData();

        if (onDataLoadedCallback != null)
            onDataLoadedCallback();
    }

    public bool CheckWorldName(string newWorldName)
    {
        foreach (GameData data in dataList)
        {
            if (data.GetWorldName() == newWorldName)
                return false;
        }
        return true;
    }

    public bool CheckCharacterName(string newCharacterName)
    {
        PlayerData[] playerDatas;
        
        if (!NetworkManager.Singleton.IsClient)
        {
            playerDatas = GetSelectedWorld().playerDataArray;
        }
        else
            playerDatas = GameManager.Instance.playerDataArray;

        foreach (PlayerData playerData in playerDatas)
        {
            if (playerData.playerName == newCharacterName)
                return false;
        }
        return true;
    }
}
