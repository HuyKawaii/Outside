using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentManager : NetworkBehaviour
{
    public Equipment[] equipmentList;
    public SkinnedMeshRenderer[] equipmentMeshes;
    public SkinnedMeshRenderer targetMesh;
    public delegate void OnEquipmentChange(Equipment newEquipment, Equipment oldEquipment);
    public OnEquipmentChange equipmentChangeCallBack;
    public delegate void OnMainHandChange(ItemSO item);
    public OnMainHandChange mainHandChangeCallBack;
    public Transform handObject;

    private void Start()
    {
        int noSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
        equipmentList = new Equipment[noSlots];
        equipmentMeshes = new SkinnedMeshRenderer[noSlots];
    }

    public void Equip(Equipment newEquipment)
    {
        //Unequip old item
        int slotIndex = (int)newEquipment.equipSlot;
        Equipment oldEquipment = Unequip(slotIndex);

        //Add to equipment list
        equipmentList[slotIndex] = newEquipment;

        //Add mesh to mesh list
        if (newEquipment.mesh != null)
        {
            SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newEquipment.mesh);
            equipmentMeshes[slotIndex] = newMesh;
            newMesh.transform.parent = targetMesh.transform;
            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;
            SetEquipmentBlendShape(newEquipment, 100);
        }
        

        //Call onEquipmentChange
        if(equipmentChangeCallBack != null)
            equipmentChangeCallBack.Invoke(newEquipment, oldEquipment);
    }

    public Equipment Unequip(int slotIndex)
    {
        Equipment oldEquipment = equipmentList[slotIndex];
        if (oldEquipment != null)
        {
            if (oldEquipment.ReturnEquipmentToPlayerInventory())
            {
                if (equipmentMeshes[slotIndex] != null)
                {
                    Destroy(equipmentMeshes[slotIndex].gameObject);
                }

                SetEquipmentBlendShape(oldEquipment, 0);

                equipmentList[slotIndex] = null;

                if (equipmentChangeCallBack != null)
                    equipmentChangeCallBack.Invoke(null, oldEquipment);

                return oldEquipment;
            }
        }

        return null;
    }

    [Rpc(SendTo.Everyone)]
    public void EquipRpc(int itemIndex)
    {
        if (!IsOwner && !IsServer)
            return;
        Equip((Equipment)MultiplayerManager.Instance.GetItemFromIndex(itemIndex));
    }

    [Rpc(SendTo.Everyone)]
    public void UnequipRpc(int itemSlot)
    {
        if (!IsOwner && !IsServer)
            return;
        Unequip(itemSlot);
    }

    [Rpc(SendTo.Everyone)]
    private void EquipMainHandRpc(int itemIndex)
    {
        if (!IsOwner && !IsServer)
            return;
        EquipMainHand(MultiplayerManager.Instance.GetItemFromIndex(itemIndex));
    }

    public void CallEquipMainHand(Item item)
    {
        EquipMainHandRpc(MultiplayerManager.Instance.GetIndexFromItem(item));
    }

    private void EquipMainHand(ItemSO item)
    {
        //if (equipmentList[(int)EquipSlot.MainHand] != null)
        if (mainHandChangeCallBack != null)
        {
            mainHandChangeCallBack(item);
        }

        if (handObject.childCount != 0)
        {
            Destroy(handObject.GetChild(0).gameObject);
            Debug.Log("Destroying hand object");
        }

        if (item == null)
        {
            Unequip((int)EquipSlot.MainHand);
        }
        else
        {
            SetHandholdGraphic(MultiplayerManager.Instance.GetIndexFromItem(item));   
            Tool tool = item as Tool;
            if (tool != null)
            {
                //Debug.Log("Holding an Tool");
                Equip(tool);
            }
            else
            {
                //Debug.Log("Holding an Item");
                //Tool newTool = new Tool(item);
                Tool newTool = ScriptableObject.CreateInstance<Tool>();
                newTool.CreateFromItem(item);
                Equip(newTool);
            }
        }
        
    }

    private void SetEquipmentBlendShape(Equipment equipment, int weight)
    {
        foreach (EquipmentMeshRegion meshRegion in equipment.coveredMeshRegions)
        {
            targetMesh.SetBlendShapeWeight((int)meshRegion, weight);
            //Debug.Log("Mesh updated");
        }
    }

    //[Rpc(SendTo.Everyone)]
    private void SetHandholdGraphic(int itemIndex)
    {
        ItemSO item = MultiplayerManager.Instance.GetItemFromIndex(itemIndex);
        GameObject newItemGraphic = Instantiate(item.itemGraphic, handObject.transform);
        newItemGraphic.transform.localScale = item.itemGraphic.transform.localScale / 100;
        newItemGraphic.transform.localEulerAngles = item.holdingRotation;
        newItemGraphic.transform.localPosition = item.holdingPosition;
        Debug.Log("Holding: " + newItemGraphic.transform.rotation.eulerAngles);
        if (IsOwner)
            newItemGraphic.layer = handObject.gameObject.layer;
    }
}
