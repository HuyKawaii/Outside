using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatData", menuName = "Combat/StatData")]
public class CharacterStatData : ScriptableObject
{
    public Stat maxHealth;
    public Stat armor;
    public Stat damage;
    public Stat attackSpeed;
    public Stat attackRange;
    public Stat visionRange;
    public Stat maxStamina;
    public Stat maxHunger;

    public Stat attackKnockbackDistance;
    public Stat knockbackResistance;

    private void OnEnable()
    {
        //Debug.Log("Reset is called");
        maxHealth.RemoveAllModifiers();
        armor.RemoveAllModifiers();
        damage.RemoveAllModifiers();
        attackSpeed.RemoveAllModifiers();
        attackRange.RemoveAllModifiers();
        visionRange.RemoveAllModifiers();
        maxStamina.RemoveAllModifiers();
        knockbackResistance.RemoveAllModifiers();
        attackKnockbackDistance.RemoveAllModifiers();
        maxHunger.RemoveAllModifiers();
    }
}

public enum StatType
{
    MaxHealth,
    Armor,
    Damage,
    AttackSpeed,
    AttackRange,
    VisionRange,
    MaxStamina,
    MaxHunger,
    AttackKnockbackDistance,
    KnockbackResistance,
}
