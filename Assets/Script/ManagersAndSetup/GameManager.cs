using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region || ----- Singleton ----- ||
    public static GameManager Instance;

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

    public ConstContainer.SceneList sceneToLoad; //debug only
    public delegate void GameManagerCallback();
    public GameManagerCallback onSetGameDataForClient;
    public bool isGameStarted = false;
    public bool isWorldSeedSet { get; private set; }
    public bool isPlayerExit;

    GameData gameData;
    PlayerData localPlayerData = PlayerData.Empty;
    public PlayerData[] playerDataArray;
    public bool isPlayingOnline;
    List<int> takenPlayerData;
    public int worldSeed { get; private set; }
    public string worldName { get; private set; }
    public string playerName { get; private set; }

    public event Action gameLoadComplete;

    private void Start()
    {
        if (SaveSelectionManager.Instance.selectedWorldIndex == -1)
        {
            worldSeed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        else
        {
            worldSeed = SaveSelectionManager.Instance.dataList[SaveSelectionManager.Instance.selectedWorldIndex].worldData.worldSeed;
        }
    }

    public void CreateGameFromSave()
    {
        gameData = SaveSelectionManager.Instance.GetSelectedWorld();
        playerDataArray = gameData.playerDataArray;
        localPlayerData = SaveSelectionManager.Instance.GetSelectedPlayerData();

        takenPlayerData = new List<int>();
        ExistingCharacterDataTaken(SaveSelectionManager.Instance.GetSelectedCharacterIndex());
        this.worldName = gameData.worldData.worldName;
        this.worldSeed = gameData.worldData.worldSeed;
        this.playerName = localPlayerData.playerName.ToString();

        LobbyManager.Instance.CreateLobby();
    }

    public void CreateGameFromSave(string playerName)
    {
        gameData = SaveSelectionManager.Instance.GetSelectedWorld();
        playerDataArray = gameData.playerDataArray;
        localPlayerData = PlayerData.Empty;

        this.worldName = gameData.worldData.worldName;
        this.worldSeed = gameData.worldData.worldSeed;
        this.playerName = playerName;

        LobbyManager.Instance.CreateLobby();
    }

    public void CreateNewGame(string worldName, int worldSeed, string playerName)
    {
        gameData = null;
        playerDataArray = null;
        localPlayerData = PlayerData.Empty;

        this.worldName = worldName;
        if (worldSeed >= 0)
            this.worldSeed = worldSeed;
        else
            this.worldSeed = UnityEngine.Random.Range(0, int.MaxValue);
        this.playerName = playerName;

        LobbyManager.Instance.CreateLobby();
    }

    public PlayerData GetLocalPlayerData()
    {
        return localPlayerData;
    }

    public PlayerData[] GetAllPlayerData()
    {
        return gameData.playerDataArray;
    }

    public void SetPlayerDataForClient(PlayerData[] playerData)
    {
        this.playerDataArray = playerData;
        if (onSetGameDataForClient != null)
        {
            Debug.Log("Running set game data callback");
            onSetGameDataForClient();
        }
    }

    public void SetCharacterForClient(int characterDataIndex)
    {
        localPlayerData = playerDataArray[characterDataIndex];
        playerName = localPlayerData.playerName.ToString();
        LobbyManager.Instance.ClientSelectCharacterData(characterDataIndex);
    }

    public void CreateCharacterForClient(string characterName)
    {
        playerName = characterName;
        LobbyManager.Instance.ClientSelectCharacterData(-1);
    }

    public void StartGame()
    {
        isGameStarted = true;
        if (isPlayingOnline)
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad.ToString(), LoadSceneMode.Single);
        else
            SceneManager.LoadScene(sceneToLoad.ToString());
    }

    public void StartGameAsClient()
    {
        isGameStarted = true;
    }

    public void ExitGame()
    {
        Cursor.lockState = CursorLockMode.None;
        isWorldSeedSet = false;
        isPlayerExit = true;
        gameData = null;
        if (NetworkManager.Singleton.IsServer && GameManager.Instance.isGameStarted)
        {
            GameSavingManager.Instance.SaveGameData();
            //NetworkManager.Singleton.SceneManager.LoadScene(ConstContainer.MenuScene, LoadSceneMode.Single);
        }
        //else
        //{
        NetworkManager.Singleton.Shutdown();

        if (SceneManager.GetActiveScene().name != ConstContainer.MenuScene)
            SceneManager.LoadScene(ConstContainer.MenuScene);
        //}

        isGameStarted = false;
        SaveSelectionManager.Instance.LoadGameDataToSelection();
    }

    public bool IsCharacterDataTaken(int index)
    {
        return takenPlayerData.Contains(index);
    }

    public void ExistingCharacterDataTaken(int index)
    {
        if (index >= 0)
            takenPlayerData.Add(index);
    }

    public void ClientCreateNewCharacter(string characterName)
    {
        localPlayerData = PlayerData.Empty;
        playerName = characterName;
        LobbyManager.Instance.ClientSelectCharacterData(-1);

    }

    public bool IsGameDataSet()
    {
        return gameData != null;
    }

    public EnviromentalData GetEnviromentalData()
    {
        return gameData.enviromentalData;
    }

    public void GameLoadComplete()
    {
        //Debug.Log("Game load complete");
        if (gameLoadComplete != null)
        {
            gameLoadComplete();
        }
    }

    public void SetWorldSeed(int worldSeed)
    {
        this.worldSeed = worldSeed;
        isWorldSeedSet = true;
    }
}