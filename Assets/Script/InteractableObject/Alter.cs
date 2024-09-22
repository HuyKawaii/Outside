using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Alter : Structure
{
    private float minDistance = 5.0f;
    private float maxDistance = 10.0f;
    public List<EnemyGroup> spawnList = new List<EnemyGroup>();
    public List<SerializableItem> requiredItemList = new List<SerializableItem>();
    public LootTable alterLootTable;
    private Transform enemyHolder;
    private bool isActivaed = false;
    public ResourcesSettings treasureChestResourceSetting;
    public float maxTimer;
    private float timer;
    protected string materialText;
    protected override void Awake()
    {
        base.Awake();

        materialText = "Not enough required material:\n";
        foreach (SerializableItem requiredItem in requiredItemList)
        {
            materialText += requiredItem.item.name + " x " + requiredItem.count;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enemyHolder = MultiplayerManager.Instance.SpawnHolder(transform);
    }

    protected override void Update()
    {
        base.Update();

        if (!IsServer)
            return;

        if (isActivaed)
        {
            //Debug.Log("Enemy count: " + enemyHolder.childCount);
            timer -= Time.deltaTime;
            if (enemyHolder.childCount == 0)
            {
                SpawnChest();
            }
            if (timer < 0)
            {
                foreach(Transform child in enemyHolder)
                    Destroy(child.gameObject);
                isActivaed = false;
            }
        }
    }

    public override void Interact(Transform player)
    {
        if (isActivaed)
            return;

        ActivateAlterRpc(player.GetComponent<NetworkObject>());
    }

    protected override void SetHoverText()
    {
        hoverText = "Active alter";
    }

    private void SpawnChest()
    {
        TreasureChest treasureChest = ResourcesSpawner.Instance.SpawnResourcesByIndex(MultiplayerManager.Instance.GetIndexFromResource(treasureChestResourceSetting), transform.position).GetComponent<TreasureChest>();
        treasureChest.SetLootTable(alterLootTable);
        Destroy(gameObject);
    }

    private bool CheckRequiredMaterial(Transform player)
    {
        PlayerInventoryManager playerInventoryManager = player.GetComponent<PlayerInventoryManager>();
        if (playerInventoryManager != null)
        {
            foreach (SerializableItem requiredItem in requiredItemList)
            {
                bool found = false;
                foreach (Item item in playerInventoryManager.itemList)
                {
                    if (item == null)
                        continue;
                    if (requiredItem.item == item.itemSO && requiredItem.count <= item.count)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Debug.Log("Requried items for alter not enough");
                    text.GetComponent<TextMesh>().text = materialText;
                    return false;
                }
            }

        }
        else
        {
            Debug.Log("Alter cant find player inventory");
            return false;
        }

        return true;
    }

    protected override void ShowText()
    {
        if (textMesh.enabled == false)
        {
            text.GetComponent<TextMesh>().text = hoverText;
        }
        base.ShowText();
    }

    private void SpawnEnemies()
    {
        foreach (EnemyGroup enemyGroup in spawnList)
        {
            int count = enemyGroup.enemyCount;

            while (count > 0)
            {
                Vector3 spawnPosition = Random.insideUnitCircle * Random.Range(minDistance, maxDistance);
                NavMeshHit hit;
                int attemp = 0;
                while (!NavMesh.SamplePosition(transform.position + spawnPosition, out hit, enemyGroup.enemyType.GetComponent<NavMeshAgent>().height * 5, 1) && attemp < 10)
                {
                    attemp++;
                }

                if (attemp == 10)
                {
                    Debug.Log("Cant place enemy on NavMesh Surface");
                    count--;
                    continue;
                }

                MultiplayerManager.Instance.SpawnEnemyMultiplayer(enemyGroup.enemyType, transform.position + spawnPosition, enemyHolder);
                count--;
            }
            
        }
        Debug.Log("Spawn list: " + spawnList.Count);
    }

    [Rpc(SendTo.Server)]
    private void ActivateAlterRpc(NetworkObjectReference playerNetworkObjectReference)
    { 
        if (playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject, NetworkManager))
        {
            Transform player = playerNetworkObject.transform;
            if (!CheckRequiredMaterial(player))
                return;

            SpawnEnemies();

            isActivaed = true;
            timer = maxTimer;
        }
    }
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyType;
    public int enemyCount;
}