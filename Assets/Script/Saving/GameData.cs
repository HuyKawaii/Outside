using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public WorldData worldData;
    public PlayerData[] playerDataArray;
    public EnviromentalData enviromentalData;
    //public EnemyData enemyData;

    public PlayerData FindPlayerData(string playerName)
    {
        foreach (PlayerData playerData in playerDataArray)
        {
            if (playerData.playerName == playerName) return playerData;
        }
        return PlayerData.Empty;
    }

    public string GetWorldName()
    {
        return worldData.worldName;
    }
}
