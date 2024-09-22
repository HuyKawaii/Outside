using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureLootGraphic : MonoBehaviour
{
    [SerializeField]
    private List<StatUpGraphic> statUpGraphicList;
    [SerializeField]
    private List<AbilityGraphic> abilityGraphicList;
    private Vector3 endScale;
    private float LerpTime = 1.5f;
    private float elapsedTime = 0f;
    private Loot loot;
    private delegate void GetLootDelegate(Collider playerCollider);
    private GetLootDelegate getLootDelegate;
    private void Start()
    {
        endScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (elapsedTime < LerpTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, endScale, Mathf.Clamp01(elapsedTime / LerpTime));
        }
       
    }

    public void SetLoot(Loot loot)
    {
        this.loot = loot;

        ItemLoot itemLoot = loot as ItemLoot;
        if (itemLoot != null)
        {
            Instantiate(itemLoot.item.item.itemGraphic, transform.position, transform.rotation, transform);
            getLootDelegate = GivePlayerItemLoot;
            return;
        }

        StatLoot statLoot = loot as StatLoot;
        if (statLoot != null)
        {
            GameObject statupGraphicPrefab = null;
            foreach (StatUpGraphic graphic in statUpGraphicList)
            {
                if (graphic.type == statLoot.type)
                    statupGraphicPrefab = graphic.graphic;
            }
            if (statupGraphicPrefab != null)
                Instantiate(statupGraphicPrefab, transform.position, transform.rotation, transform);
            getLootDelegate = GivePlayerStatLoot;
            return;
        }

        AbilityLoot abilityLoot = loot as AbilityLoot;
        if (abilityLoot != null)
        {
            GameObject abilityGraphicPrefab = null;
            foreach (AbilityGraphic abilityGraphic in abilityGraphicList)
            {
                if ((int)abilityGraphic.type == abilityLoot.abilityIndex)
                    abilityGraphicPrefab = abilityGraphic.graphic;
            }

            if (abilityGraphicPrefab != null)
                Instantiate(abilityGraphicPrefab, transform.position, transform.rotation, transform);
            getLootDelegate = GivePlayerAbility;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetLoot(other))
            Destroy(gameObject);
    }

    protected bool GetLoot(Collider other)
    {
        if (other.GetComponent<PlayerInventoryManager>() != null)
        {
            if (getLootDelegate != null)
                getLootDelegate(other);
            return true;
        }
        return false;
    }

    private void GivePlayerItemLoot(Collider collider)
    {
        ItemLoot itemLoot = loot as ItemLoot;
        collider.GetComponent<PlayerInventoryManager>().Add(new Item(itemLoot.item.item, itemLoot.item.count));
    }

    private void GivePlayerStatLoot(Collider collider)
    {
        StatLoot statLoot = loot as StatLoot;
        CharacterStats playerStats = GetComponent<CharacterStats>();

        if (statLoot.type == StatType.Damage)
            playerStats.statData.damage.AddMultiplierModifier(statLoot.statMutiplier);
    }

    private void GivePlayerAbility(Collider collider)
    {
        AbilityLoot abilityLoot = loot as AbilityLoot;
        AbilityManager.Instance.abilityUnlock[abilityLoot.abilityIndex] = true;
    }
}

[Serializable]
public class StatUpGraphic
{
    public StatType type;
    public GameObject graphic;
}

[Serializable]
public class AbilityGraphic
{
    public Ability type;
    public GameObject graphic;
}
