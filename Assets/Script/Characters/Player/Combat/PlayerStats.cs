using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private EquipmentManager equipmentManager;

    protected override void Start()
    {
        base.Start();
        equipmentManager = GetComponentInChildren<EquipmentManager>();
        equipmentManager.equipmentChangeCallBack += UpdateModifiers;
    }

    void UpdateModifiers(Equipment newEquipment, Equipment oldEquipment)
    {
        if (oldEquipment != null)
        {
            statData.damage.RemoveModifier(oldEquipment.damageModifier);
            statData.armor.RemoveModifier(oldEquipment.armorModifier);
        }

        if (newEquipment != null)
        {
            statData.damage.AddModifier(newEquipment.damageModifier);
            statData.armor.AddModifier(newEquipment.armorModifier);
        }
        
    }

    public override void OutOfHealth()
    {
        base.OutOfHealth();
        Debug.Log("Player Died");
    }
}
