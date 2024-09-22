using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStats : EnemyStats
{
    public new ResourceStatData statData
    {
        get { return (ResourceStatData)base.statData; }
        set { base.statData = value; }
    }

    protected override void Start()
    {
        base.Start();
        try
        {
            ResourceStatData thisStat = statData;
        }
        catch (InvalidCastException)
        {
            Debug.LogError("Change stat data of " + gameObject.name + " to correct type");
        }
    }

    public override void TakeDamage(GameObject damageSource, float damageTaken)
    {
        if (!IsOwner) return;
        Tool usedTool = (Tool)damageSource.GetComponentInChildren<EquipmentManager>().equipmentList[(int)EquipSlot.MainHand];
        if (usedTool == null)
            return;
        Debug.Log(usedTool.toolType);

        if ((statData.toolType == Tool.ToolType.None || statData.toolType == usedTool.toolType) && statData.toolLevel <= usedTool.toolLevel)
        {
            base.TakeDamage(damageSource, damageTaken);
        }
        else
        {
            Debug.Log("Wrong type of tool or tool not strong enough");
        }
    }
}
