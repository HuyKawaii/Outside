using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Loot Table")]
[Serializable]
public class LootTable : ScriptableObject
{
    public float value;
    [SerializeReference, SubclassSelector]
    public List<Loot> LootList;

    public Loot RollLootTable()
    {
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);
        foreach (Loot loot in LootList)
        {
            rand -= loot.dropChance;
            if (rand <= 0.0f)
                return loot;
        }

        return null;
    }
}

[Serializable]
public abstract class Loot
{
    public float dropChance;
}


[Serializable]
public class ItemLoot : Loot
{
    public SerializableItem item;
}

[Serializable]
public class StatLoot : Loot
{
    public StatType type;
    public float statMutiplier;
}

[Serializable]
public class AbilityLoot: Loot
{
    public int abilityIndex;
}