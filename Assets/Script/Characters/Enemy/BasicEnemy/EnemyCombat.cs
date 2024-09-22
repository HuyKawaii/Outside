using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyCombat : CharacterCombat
{
    protected EnemyController enemyController { get; private set; }

    public List<LootTable> loots;
    public float attackRange {  get; private set; }
    private float attackTimer;

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyController>();
    }

    protected override void Start()
    {
        base.Start();
        attackRange = myStats.statData.attackRange.GetValue();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!IsServer) return;
        base.Update();
        AttackPattern();
    }

    public virtual void AttackPattern()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else if (enemyController.distance < attackRange)
        {
            Attack();
            attackTimer = 1 / myStats.statData.attackSpeed.GetValue();
        }
    }

    public virtual void Die()
    {
        Debug.Log("Enemy died");
        DropLoot();
        Destroy(gameObject);
        Debug.Log("Object destroyed");
    }

    public void DropLoot()
    {
        Debug.Log("Drop Loot");

        foreach (LootTable lootTable in loots)
        {
            ItemLoot lootDrop = (ItemLoot)lootTable.RollLootTable();
            Item item = new Item(lootDrop.item.item, lootDrop.item.count);
            item.DropInWorld(transform.position);
        }
        
    }

    protected override void SetTargetTag()
    {
        base.SetTargetTag();
        targetTag = "Player";
    }
}
