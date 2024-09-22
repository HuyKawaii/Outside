using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Item/New Equipment")]
public class Equipment : ItemSO
{
    public EquipSlot equipSlot;
    public EquipmentMeshRegion[] coveredMeshRegions;

    public int armorModifier;
    public int damageModifier;

    public virtual void Equip()
    {
        PlayerManager.Instance.player.GetComponentInChildren<EquipmentManager>().EquipRpc(MultiplayerManager.Instance.GetIndexFromItem(this));
    }

    public virtual bool ReturnEquipmentToPlayerInventory()
    {
        Item item = new Item(this);
        return item.ReturnToPlayerInventory();
    }

}

public enum EquipSlot { Head, Chest, Legs, Feet, MainHand, OffHand }
public enum EquipmentMeshRegion { Legs, Arms, Torso}