using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GameSavingManager : MonoBehaviour
{
    #region || ----- Singleton ----- ||
    public static GameSavingManager Instance { get; private set; }

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

    private string settingKey = "Setting";
    private string saveDirectory;

    private void Start()
    {
        saveDirectory = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Saves";

        if (!System.IO.Directory.Exists(saveDirectory))
        {
            System.IO.Directory.CreateDirectory(saveDirectory);
        }
    }

    #region || ----- Save Game Data ----- ||

    #region || ----- Save All Game Data ----- ||

    public GameData GetGameData()
    {
        WorldData worldData = GetWorldData();

        List<PlayerData> playerData = new List<PlayerData>();

        foreach (GameObject playerObject in LobbyManager.Instance.playerList)
        {
            playerData.Add(GetPlayerData(playerObject));
        }
       

        EnviromentalData enviromentalData = GetEnviromentalData();

        GameData gameData = new GameData
        {
            worldData = worldData,
            playerDataArray = playerData.ToArray(),
            enviromentalData = enviromentalData,
        };

        return gameData;
    }

    public void SaveGameData()
    {
        GameData gameData = GetGameData();
        WorldData worldData = gameData.worldData;
        string gameDirectory = saveDirectory + Path.AltDirectorySeparatorChar + worldData.worldName;

        if (!System.IO.Directory.Exists(gameDirectory))
        {
            System.IO.Directory.CreateDirectory(gameDirectory);
        }

        SaveWorldData(worldData, gameDirectory);
        SaveEnviromentaldData(gameData.enviromentalData, gameDirectory);

        string playerDataDirectory = gameDirectory + Path.AltDirectorySeparatorChar + "PlayerData";
        if (!System.IO.Directory.Exists(playerDataDirectory))
        {
            System.IO.Directory.CreateDirectory(playerDataDirectory);
        }

        PlayerData[] playerDataArray = gameData.playerDataArray;
        foreach (PlayerData playerData in playerDataArray)
        {
            SavePlayerData(playerData, gameDirectory);
        }
    }

    public List<GameData> LoadAllData()
    {
        List<GameData> dataList = new List<GameData>();
        foreach (string directory in Directory.EnumerateDirectories(saveDirectory))
        {
            WorldData worldData;
            using (StreamReader sr = new StreamReader(directory + Path.AltDirectorySeparatorChar + "worldData.json"))
            {
                string json = sr.ReadToEnd();
                worldData = JsonUtility.FromJson<WorldData>(json);
            }

            EnviromentalData enviromentalData;
            using (StreamReader sr = new StreamReader(directory + Path.AltDirectorySeparatorChar + "enviromentalData.json"))
            {
                string json = sr.ReadToEnd();
                enviromentalData = JsonUtility.FromJson<EnviromentalData>(json);
            }

            string playerDataDirectory = directory + Path.AltDirectorySeparatorChar + "PlayerData";
            List<PlayerData> playerDataList = new List<PlayerData>();

            if (System.IO.Directory.Exists (playerDataDirectory))
            {
                foreach (string file in Directory.EnumerateFiles(playerDataDirectory))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string json = sr.ReadToEnd();
                        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                        //Debug.Log(playerData.playerName);
                        playerDataList.Add(playerData);
                    }
                }
            }
            
            dataList.Add(new GameData
            {
                worldData = worldData,
                playerDataArray = playerDataList.ToArray(),
                enviromentalData = enviromentalData,
            });
        }
        //Debug.Log("All data loaded");
        return dataList;
    }

    public void DeleteGameData(GameData gameData)
    {
        string worldName = gameData.worldData.worldName;
        string gameDirectory = saveDirectory + Path.AltDirectorySeparatorChar + worldName;

        if (System.IO.Directory.Exists(gameDirectory))
        {
            System.IO.Directory.Delete(gameDirectory, true);
            Debug.Log("Game data deleted at: " + gameDirectory);
        }
    }

    public void DeleteChracterData(GameData gameData, PlayerData playerData)
    {
        string worldName = gameData.worldData.worldName;
        string gameDirectory = saveDirectory + Path.AltDirectorySeparatorChar + worldName;

        if (System.IO.Directory.Exists(gameDirectory))
        {
            System.IO.Directory.Delete(gameDirectory, true);
            Debug.Log("Game data deleted at: " + gameDirectory);
        }
    }

    #endregion

    #region Get Data
    private WorldData GetWorldData()
    {
        return new WorldData
        {
            worldName = GameManager.Instance.worldName,
            worldSeed = GameManager.Instance.worldSeed,
        };
    }

    private EntityData GetEnemyData(GameObject enemy)
    {
        int index = MultiplayerManager.Instance.GetIndexFromInstantiatedEnemy(enemy);
        float health = enemy.GetComponent<EnemyStats>().health;
        if (index < 0)
        {
            Debug.Log("Null entity " + enemy.name);
            return EntityData.Empty;
        }

        return new EntityData
        {
            health = health,
            entityIndex = index,
            position = enemy.transform.position,
            rotation = enemy.transform.rotation.eulerAngles,
        };
    }

    private EntityData GetResourceData(GameObject resource)
    {
        int index = MultiplayerManager.Instance.GetIndexFromResource(resource.GetComponent<ResourceController>().GetReferencfe());
        ResourceStats resourceStats = resource.GetComponent<ResourceStats>();
        float health = 0;
        if (resourceStats != null)
            health = resourceStats.health;

        if (index < 0)
        {
            Debug.Log("Null entity " + resource.name);
            return EntityData.Empty;
        }

        return new EntityData
        {
            health = health,
            entityIndex = index,
            position = resource.transform.position,
            rotation = resource.transform.rotation.eulerAngles,
        };
    }

    private PickupData GetPickupData(GameObject pickup)
    {
        int index = MultiplayerManager.Instance.GetIndexFromItem(pickup.GetComponent<PickupItemBehaviour>().item.item);
        int count = pickup.GetComponent<PickupItemBehaviour>().item.count;
        if (index < 0)
        {
            //Debug.Log("Null entity " + pickup.name);
            return PickupData.Empty;
        }

        return new PickupData
        {
            itemIndex = index,
            itemCount = count,
            position = pickup.transform.position,
            rotation = pickup.transform.rotation.eulerAngles,
        };
    }

    private EntityData GetPlacealbeData(GameObject placealbe)
    {
        int index = MultiplayerManager.Instance.GetIndexFromItem(placealbe.GetComponent<PlaceableReference>().reference);
        float health = placealbe.GetComponent<ResourceStats>().health;

        return new EntityData
        {
            entityIndex = index,
            health = health,
            position = placealbe.transform.position,
            rotation = placealbe.transform.rotation.eulerAngles,
        };
    }

    private ChestData GetChestData(GameObject chest)
    {
        InventoryManager chestInventory = chest.GetComponent<InventoryManager>();
        int[] chestInventoryData = new int[72];
        for (int i = 0; i < chestInventory.inventorySize; i++)
        {
            Item item = chestInventory.itemList[i];
            chestInventoryData[2 * i] = MultiplayerManager.Instance.GetIndexFromItem(item);
            if (item != null)
                chestInventoryData[2 * i + 1] = chestInventory.itemList[i].count;
        }

        return new ChestData
        {
            health = chest.GetComponent<ResourceStats>().health,
            position = chest.transform.position,
            rotation = chest.transform.rotation.eulerAngles,
            chestContent = chestInventoryData,
        };
    }

    private StructureData GetStructureData(GameObject structure)
    {
        int index = MultiplayerManager.Instance.GetIndexFromStrucuter(structure.GetComponent<Structure>().GetStructureReference());

        return new StructureData
        {
            structureIndex = index,
            position = structure.transform.position,
            rotation = structure.transform.rotation.eulerAngles,
        };
    }

    private EnviromentalData GetEnviromentalData()
    {
        #region Time of day
        float timeOfDay = DayNightManager.instance.GetTimeOfDay();
        #endregion
        #region Enemy List
        List<EntityData> enemyDataList = new List<EntityData>();

        foreach (Transform entity in SetupSingleton.Instance.enemyHolder.transform)
        {
            EntityData entityData = GetEnemyData(entity.gameObject);

            if (!entityData.IsEmpty())
                enemyDataList.Add(entityData);
        }
        #endregion

        #region Resource List
        List<EntityData> resourceDataList = new List<EntityData>();

        foreach (Transform resource in SetupSingleton.Instance.resourceHolder.transform)
        {
            EntityData entityData = GetResourceData(resource.gameObject);

            if (!entityData.IsEmpty())
                resourceDataList.Add(entityData);
        }
        #endregion

        #region Pickup List
        List<PickupData> pickupDataList = new List<PickupData>();

        foreach (Transform pickup in SetupSingleton.Instance.pickupHolder.transform)
        {
            PickupData pickupData = GetPickupData(pickup.gameObject);
            //Debug.Log(pickup.name);
            if (!pickupData.IsEmpty())
            {
                pickupDataList.Add(pickupData);
                //Debug.Log("Pickup addded");
            }
        }
        #endregion

        #region Structure List
        List<StructureData> structureDataList = new List<StructureData>();

        foreach (Transform structure in SetupSingleton.Instance.structureHolder.transform)
        {
            StructureData structureData = GetStructureData(structure.gameObject);
            if (!structureData.IsEmpty())
            {
                structureDataList.Add(structureData);
            }
        }
        #endregion

        #region Placeable List
        List<EntityData> placeableDataList = new List<EntityData>();
        List<ChestData> chestDataList = new List<ChestData>();

        foreach (Transform entity in SetupSingleton.Instance.placeableHolder.transform)
        {
            if (MultiplayerManager.Instance.GetIndexFromItem(entity.GetComponent<PlaceableReference>().reference) == 0)
            {
                ChestData chestData = GetChestData(entity.gameObject);
                chestDataList.Add(chestData);
            }
            else
            {
                EntityData entityData = GetPlacealbeData(entity.gameObject);

                if (!entityData.IsEmpty())
                    placeableDataList.Add(entityData);
            }
        }
        #endregion

        return new EnviromentalData
        {
            timeOfDay = timeOfDay,
            enemyDataList = enemyDataList,
            resourceDataList = resourceDataList,
            pickupDataList = pickupDataList,
            structureDataList = structureDataList,
            placeableDataList = placeableDataList,
            chestDataList = chestDataList,
        };
    }

    private PlayerData GetPlayerData(GameObject playerGameObject)
    {
        InventoryManager playerInventory = playerGameObject.GetComponent<InventoryManager>();
        int[] playerInventoryData = new int[72];
        for (int i = 0; i < playerInventory.inventorySize; i++)
        {
            Item item = playerInventory.itemList[i];
            playerInventoryData[2 * i] = MultiplayerManager.Instance.GetIndexFromItem(item);
            if (item != null)
                playerInventoryData[2 * i + 1] = playerInventory.itemList[i].count;
        }

        PlayerData playerData = new PlayerData()
        {
            playerHealth = playerGameObject.GetComponent<PlayerStats>().health,
            playerName = playerGameObject.GetComponentInChildren<PlayerName>().GetPlayerName(),
            playerPosition = UtilFunc.ConvertVector3ToFloat(playerGameObject.transform.position),
            playerRotation = UtilFunc.ConvertVector3ToFloat(playerGameObject.transform.rotation.eulerAngles),
            playerInventory = playerInventoryData,
        };

        return playerData;
    }

    #endregion

    #region || ----- Save Parital Data ----- ||

    private void SaveWorldData(WorldData worldData, string gameDirectory)
    {
        string json = JsonUtility.ToJson(worldData);
        using (StreamWriter sw = new StreamWriter(gameDirectory + Path.AltDirectorySeparatorChar + "worldData.json"))
        {
            sw.Write(json);
            Debug.Log("Saved world data to: " + gameDirectory);
        }
    }

    private void SaveEnviromentaldData(EnviromentalData enviromentalData, string gameDirectory)
    {
        string json = JsonUtility.ToJson(enviromentalData);
        using (StreamWriter sw = new StreamWriter(gameDirectory + Path.AltDirectorySeparatorChar + "enviromentalData.json"))
        {
            sw.Write(json);
            Debug.Log("Saved enviromental data to: " + gameDirectory);
        }
    }

    public void SavePlayerData(PlayerData playerData, string gameDirectory)
    {
        string json = JsonUtility.ToJson(playerData);
        string playerDataPath = gameDirectory + Path.AltDirectorySeparatorChar + "PlayerData" + Path.AltDirectorySeparatorChar + playerData.playerName + ".json";
        using (StreamWriter sw = new StreamWriter(playerDataPath))
        {
            sw.Write(json);
            Debug.Log("Saved player data to: " + playerDataPath);
        }
    }

    public PlayerData LoadPlayerData(string playerName)
    {
        using (StreamReader sr = new StreamReader(saveDirectory + Path.AltDirectorySeparatorChar + playerName + ".json"))
        {
            string json = sr.ReadToEnd();
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }

    #endregion

    #endregion

    #region || ----- Save Setting ----- ||
    public void SaveSetting(float _masterVolumn)
    {
        Settings newSettings = new Settings()
        {
            masterVolumn = _masterVolumn,
        };

        PlayerPrefs.SetString(settingKey, JsonUtility.ToJson(newSettings));
        PlayerPrefs.Save();
    }

    public Settings LoadSetting()
    {
        return JsonUtility.FromJson<Settings>(PlayerPrefs.GetString(settingKey));
    }
    #endregion
}

[System.Serializable]
public class Settings
{
    public float masterVolumn;
}