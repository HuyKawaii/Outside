using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TreasureChest : Interactalble
{
    public int goldToOpen;
    private PlayerEconManager playerEconManager;
    private Animator animator;
    private TreasureLootGraphic lootGraphic;
    [SerializeField] private LootTable lootTable;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        lootGraphic = GetComponentInChildren<TreasureLootGraphic>();
    }

    protected override void Start()
    {
        base.Start();
        lootGraphic.gameObject.SetActive(false);
    }
    protected override void SetHoverText()
    {
        hoverText = goldToOpen + " gold to open";
    }

    public override void Interact(Transform player)
    {
        Loot lootDrop = lootTable.RollLootTable();
        lootGraphic.SetLoot(lootDrop);
        playerEconManager = player.GetComponent<PlayerEconManager>();
        if (playerEconManager.SpendGold(goldToOpen))
        {
            Debug.Log("Opened Treasure Chest");
            animator.SetTrigger("open");
            DisableHoverText();
        }
        else
            Debug.Log("Not enough gold");
    }

    public void RevealLoot()
    {
        lootGraphic.gameObject.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    private void SetLootTableRpc(int lootTableIndex)
    {
        this.lootTable = MultiplayerManager.Instance.GetLootTableFromIndex(lootTableIndex);
    }

    public void SetLootTable(LootTable lootTable)
    {
        SetLootTableRpc(MultiplayerManager.Instance.GetIndexFromLootTable(lootTable));
    }
}
