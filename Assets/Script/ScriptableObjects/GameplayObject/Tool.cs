using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Item/New Tool")]
public class Tool : Equipment
{
    public int toolLevel;
    public ToolType toolType;
    protected virtual void Reset()
    {
        equipSlot = EquipSlot.MainHand;
        coveredMeshRegions = new EquipmentMeshRegion[] { EquipmentMeshRegion.Arms };
    }

    public void CreateFromItem(ItemSO item)
    {
        name = item.name;
        sprite = item.sprite;
        mesh = item.mesh;
        itemPickupPrefab = item.itemPickupPrefab;
        isStackable = false;
        equipSlot = EquipSlot.MainHand;
        coveredMeshRegions = new EquipmentMeshRegion[] { EquipmentMeshRegion.Arms };
        armorModifier = 0;
        damageModifier = 0;
        toolLevel = 0;
    }

    public override bool Use(InventoryManager inventoryManager)
    {
        return true;
    }

    public override void Equip()
    {
        PlayerManager.Instance.player.GetComponentInChildren<EquipmentManager>().EquipRpc(MultiplayerManager.Instance.GetIndexFromItem(this));
    }

    public override bool ReturnEquipmentToPlayerInventory()
    {
        return true;
    }

    public enum ToolType { Axe, Pickage, None}
}
