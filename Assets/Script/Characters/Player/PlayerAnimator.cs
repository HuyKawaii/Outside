using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    EquipmentManager equipmentManager;
    //int mainHandLayer = 1;
    //int offHandLayer = 2;

    [SerializeField] Animator toolAnimator;
    PlayerCombat playerCombat;
    protected override void Start()
    {
        base.Start();
        equipmentManager = GetComponentInChildren<EquipmentManager>();
        playerCombat = GetComponent<PlayerCombat>();
        playerCombat.initiateAttackCallback += SetSwingTrigger;
        //equipmentManager.equipmentChangeCallBack += ToggleGripAnimation;
    }


    void SetSwingTrigger()
    {
        toolAnimator.SetTrigger("swingTrigger");
        //Debug.Log("Play animation");
    }
    //public void ToggleGripAnimation(Equipment newEquipment, Equipment oldEquipment)
    //{
    //    if (newEquipment != null && newEquipment.equipSlot == EquipmentSlot.MainHand)
    //        characterAnimator.SetLayerWeight(mainHandLayer, 1);
    //    else if (newEquipment == null && oldEquipment.equipSlot == EquipmentSlot.MainHand)
    //        characterAnimator.SetLayerWeight(mainHandLayer, 0);
    //    else if (newEquipment != null && newEquipment.equipSlot == EquipmentSlot.OffHand)
    //        characterAnimator.SetLayerWeight(offHandLayer, 1);
    //    else if (newEquipment == null && oldEquipment.equipSlot == EquipmentSlot.OffHand)
    //        characterAnimator.SetLayerWeight(offHandLayer, 0);
    //}
}
