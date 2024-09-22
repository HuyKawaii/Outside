using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentalData
{
    public float timeOfDay;
    public List<EntityData> enemyDataList;
    public List<EntityData> resourceDataList;
    public List<PickupData> pickupDataList;
    public List<StructureData> structureDataList;
    public List<EntityData> placeableDataList;
    public List<ChestData> chestDataList;
}

[System.Serializable]
public class EntityData
{
    public float health;
    public int entityIndex;
    public  Vector3 position;
    public Vector3 rotation;

    public static readonly EntityData Empty = new EntityData
    {
        entityIndex = -1,
        position = Vector3.zero,
        rotation = Vector3.zero,
    };

    public bool IsEmpty()
    {
        return entityIndex == -1;
    }
}

[System.Serializable]
public class PickupData
{
    public int itemIndex;
    public int itemCount;
    public Vector3 position;
    public Vector3 rotation;

    public static readonly PickupData Empty = new PickupData
    {
        itemIndex = -1,
        position = Vector3.zero,
        rotation = Vector3.zero,
    };

    public bool IsEmpty()
    {
        return itemIndex == -1;
    }
}

[System.Serializable]
public class StructureData
{
    public int structureIndex;
    public Vector3 position;
    public Vector3 rotation;

    public static readonly StructureData Empty = new StructureData
    {
        structureIndex = -1,
        position = Vector3.zero,
        rotation = Vector3.zero,
    };

    public bool IsEmpty()
    {
        return structureIndex == -1;
    }
}

[System.Serializable]
public class ChestData
{
    public float health;
    public Vector3 position;
    public Vector3 rotation;
    public int[] chestContent;
}