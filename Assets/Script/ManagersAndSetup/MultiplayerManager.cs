using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MultiplayerManager : NetworkBehaviour
{
    #region Singleton
    public static MultiplayerManager Instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    
    [SerializeField] private ItemListSO itemList;
    [SerializeField] private EntityListSO enemyList;
    [SerializeField] private ResourcesListSO resourceList;
    [SerializeField] private StructureListSO structureList;
    [SerializeField] private LootTableListSO lootTableList;
    #region Spawn Pickup
    public ItemSO GetItemFromIndex(int index)
    {
        if (index < 0)
            return null;
        return itemList.itemSOList[index];
    }

    public int GetIndexFromItem(Item item)
    {
        if (item == null) return -1;
        return itemList.itemSOList.IndexOf(item.itemSO);
    }

    public int GetIndexFromItem(ItemSO item)
    {
        return itemList.itemSOList.IndexOf(item);
    }

    public void DropItemInWorld(Vector3 position, Item item)
    {
        DropItemInWorldServerRpc(position, GetIndexFromItem(item), item.count);
    }

    public void DropItemInWorld(Vector3 position, int itemIndex, int itemCount)
    {
        DropItemInWorldServerRpc(position, itemIndex, itemCount);
    }

    public void DropItemInWorld(Vector3 position, SerializableItem item)
    {
        DropItemInWorldServerRpc(position, GetIndexFromItem(item.item), item.count);
    }

    [Rpc(SendTo.Server)]
    private void DropItemInWorldServerRpc(Vector3 position, int itemIndex, int amount = 1)
    {
        NetworkObjectReference pickUp = InstantiatePickup(itemIndex, position);
        SetPickUpItemClientRpc(itemIndex, pickUp, amount);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPickUpItemClientRpc(int itemIndex, NetworkObjectReference pickUpNetworkObjectReference, int amount = 1)
    {
        pickUpNetworkObjectReference.TryGet(out NetworkObject pickUp);
        Item thisItem = new Item(GetItemFromIndex(itemIndex), amount);
        pickUp.GetComponent<PickupItemBehaviour>().item = new SerializableItem(thisItem.itemSO, thisItem.count);
    }

    private NetworkObjectReference InstantiatePickup(int itemIndex, Vector3 position)
    {
        NetworkObject pickup = Instantiate(GetItemFromIndex(itemIndex).itemPickupPrefab, position, Quaternion.identity).GetComponent<NetworkObject>();
        //Debug.Log(pickup.name);
        //Debug.Log(sourceObject.transform.position);
        pickup.Spawn();
        pickup.transform.parent = SetupSingleton.Instance.pickupHolder;
        return pickup;
    }

    public void PlaceObjectInWorldMultiplayer(Placeable placeable, Vector3 placePosition)
    {
        PlaceItemInWorldServerRpc(GetIndexFromItem(placeable), placePosition);
    }

    public void PlaceObjectInWorldMultiplayerByIndex(int index, Vector3 placePosition)
    {
        PlaceItemInWorldServerRpc(index, placePosition);
    }

    public void PlaceChestInWorldMultiplayer(Vector3 placePosition, int[] chestContent)
    {
        GameObject placedPrefab = ((Placeable)GetItemFromIndex(0)).placedFerfab;
        NetworkObject placedObject = Instantiate(placedPrefab, placePosition, placedPrefab.transform.rotation).GetComponent<NetworkObject>();
        placedObject.Spawn();
        placedObject.transform.parent = SetupSingleton.Instance.placeableHolder;

        InventoryManager chestInventory = placedObject.GetComponent<InventoryManager>();

        for (int i = 0; i < chestContent.Length; i += 2)
        {
            if (chestContent[i] < 0)
                continue;
            Item item = new Item(GetItemFromIndex(chestContent[i]), chestContent[i + 1]);
            chestInventory.Add(item, (int)i / 2);
        }
    }

    [Rpc(SendTo.Server)]
    private void PlaceItemInWorldServerRpc(int itemIndex, Vector3 placePosition)
    {
        GameObject placedPrefab = ((Placeable)GetItemFromIndex(itemIndex)).placedFerfab;
        NetworkObject placedObject = Instantiate(placedPrefab, placePosition, placedPrefab.transform.rotation).GetComponent<NetworkObject>();
        placedObject.Spawn();
        placedObject.transform.parent = SetupSingleton.Instance.placeableHolder;
    }
    #endregion

    #region SpawnResource
    public void SpawnResourceMultiplayer(ResourcesSettings resource, Vector3 spawnPosition)
    {
        SpawnResourceServerRpc(GetIndexFromResource(resource), spawnPosition);
    }

    public void SpawnResourceFromIndexMultiplayer(int resourceIndex, Vector3 spawnPosition, float health = 0)
    {
        SpawnResourceServerRpc(resourceIndex, spawnPosition, health);
    }

    [Rpc(SendTo.Server)]
    private void SpawnResourceServerRpc(int resourceIndex, Vector3 spawnPosition, float health = 0)
    {
        ResourcesSpawner.Instance.SpawnResourcesByIndex(resourceIndex, spawnPosition, health);
    }

    public int GetIndexFromResource(ResourcesSettings resource)
    {
        return resourceList.resourceList.IndexOf(resource);
    }

    public ResourcesSettings GetResourceFromIndex(int resourceIndex)
    {
        return resourceList.resourceList[resourceIndex];
    }
    #endregion

    #region Spawn Structure
    public void SpawnStructureMutiplayer(StructureSetting structure, Vector3 position)
    {
        SpawnStructureServerRpc(GetIndexFromStrucuter(structure), position);
    }

    public void SpawnStructureFromIndexMutiplayer(int structureIndex, Vector3 position)
    {
        SpawnStructureServerRpc(structureIndex, position);
    }

    [Rpc(SendTo.Server)]
    private void SpawnStructureServerRpc(int structureIndex, Vector3 position)
    {
        StructureSetting structureSetting = GetStructureFromIndex(structureIndex);
        GameObject structure = Instantiate(structureSetting.structurePrefab, position, Quaternion.identity);
        structure.GetComponent<NetworkObject>().Spawn();
        structure.transform.parent = SetupSingleton.Instance.structureHolder.transform;
    }

    public int GetIndexFromStrucuter(StructureSetting structure)
    {
        return structureList.structureList.IndexOf(structure);
    }

    public StructureSetting GetStructureFromIndex(int structureIndex)
    {
        return structureList.structureList[structureIndex];
    }
    #endregion

    #region Spawn Enemy
    public void SpawnEnemyMultiplayer(GameObject enitty, Vector3 spawnPosition, Transform parent = null)
    {
        if (parent == null) 
            SpawnEnemyServerRpc(GetIndexFromEnemy(enitty), spawnPosition);
        else
            SpawnEnemyWithSeperateParentServerRpc(GetIndexFromEnemy(enitty), spawnPosition, parent.GetComponent<NetworkObject>());
    }

    public void SpawnEnemyFromIndexMultiplayer(int entityIndex, Vector3 spawnPosition, float health = 0)
    {
        SpawnEnemyServerRpc(entityIndex, spawnPosition, health);
    }

    [Rpc(SendTo.Server)]
    private void SpawnEnemyServerRpc(int entityIndex, Vector3 spawnPosition, float health = 0)
    {
        EnemySpawnManager.Instance.SpawnEnemyByIndex(entityIndex, spawnPosition, null, health);
    }

    [Rpc(SendTo.Server)]
    private void SpawnEnemyWithSeperateParentServerRpc(int entityIndex, Vector3 spawnPosition, NetworkObjectReference parent)
    {
        NetworkObject networkObject;
        parent.TryGet(out networkObject, NetworkManager);
        Debug.Log(networkObject.gameObject.name);
        EnemySpawnManager.Instance.SpawnEnemyByIndex(entityIndex, spawnPosition, networkObject.transform);
    }

    private int GetIndexFromEnemy(GameObject enemy)
    {
        return enemyList.entityList.IndexOf(enemy);
    }

    public int GetIndexFromInstantiatedEnemy(GameObject enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats == null)
            return -1;

        for (int i = 1; i < enemyList.entityList.Count; i++)
        {
            Debug.Log("Enemy: " + enemy.name);
            Debug.Log("Enemy: " + enemyList.entityList[i].GetComponent<EnemyStats>().statData);
            if (enemyStats.statData == enemyList.entityList[i].GetComponent<EnemyStats>().statData)
            {
                return i;
            }
        }
        return -1;
    }

    public GameObject GetEnemyFromIndex(int index)
    {
        return enemyList.entityList[index];
    }
    #endregion

    #region LootTable
    public int GetIndexFromLootTable(LootTable lootTable)
    {
        return lootTableList.lootTableList.IndexOf(lootTable);
    }

    public LootTable GetLootTableFromIndex(int index)
    {
        return lootTableList.lootTableList[index];
    }
    #endregion

    #region SpawnProjectile
    public void SpawnProjectileMultiplayer(GameObject entity, Vector3 spawnPosition, Vector3 target)
    {
        SpawnProjectileServerRpc(GetIndexFromEnemy(entity), spawnPosition, target);
    }

    [Rpc(SendTo.Server)]
    private void SpawnProjectileServerRpc(int entityIndex, Vector3 spawnPosition, Vector3 target)
    {
        GameObject projectile = Instantiate(GetEnemyFromIndex(entityIndex), spawnPosition, Quaternion.identity);
        projectile.GetComponent<ArrowController>().SetTarget(target);
        projectile.GetComponent<NetworkObject>().Spawn();
    }
    #endregion

    #region Setup
    public void SetWorldSeedMultiplayer()
    {
        if (IsServer)
        {
            SetWorldSeedRpc(GameManager.Instance.worldSeed);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetWorldSeedRpc(int seed)
    {
        Debug.Log(TerrainLoader.Instance);
        GameManager.Instance.SetWorldSeed(seed);
    }

    public Transform SpawnHolder(Transform parent)
    {
        if (!IsServer)
            return null;
        GameObject holder = Instantiate(SetupSingleton.Instance.holderPrefab);
        holder.GetComponent<NetworkObject>().Spawn();
        holder.transform.SetParent(parent);
        return holder.transform;
    }

    public void SpawnSetupHolder()
    {
        if (IsServer)
        {
            GameObject entityHolder = Instantiate(SetupSingleton.Instance.holderPrefab);
            entityHolder.GetComponent<NetworkObject>().Spawn();
            SetupSingleton.Instance.enemyHolder = entityHolder.transform;

            GameObject pickupHolder = Instantiate(SetupSingleton.Instance.holderPrefab);
            pickupHolder.GetComponent<NetworkObject>().Spawn();
            SetupSingleton.Instance.pickupHolder = pickupHolder.transform;

            GameObject resourceHolder = Instantiate(SetupSingleton.Instance.holderPrefab);
            resourceHolder.GetComponent<NetworkObject>().Spawn();
            SetupSingleton.Instance.resourceHolder = resourceHolder.transform;

            GameObject structureHolder = Instantiate(SetupSingleton.Instance.holderPrefab);
            structureHolder.GetComponent<NetworkObject>().Spawn();
            SetupSingleton.Instance.structureHolder = structureHolder.transform;

            GameObject placeableHolder = Instantiate(SetupSingleton.Instance.holderPrefab);
            placeableHolder.GetComponent<NetworkObject>().Spawn();
            SetupSingleton.Instance.placeableHolder = placeableHolder.transform;
        }
    }
    #endregion
    
}
