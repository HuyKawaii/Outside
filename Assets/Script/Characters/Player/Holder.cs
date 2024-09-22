using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    [SerializeField]
    private EquipmentManager equipmentManager;
    private LayerMask firstPersonRender = 12;

    private void Start()
    {
        equipmentManager.mainHandChangeCallBack += ChangeHoldingItem;
    }

    private void ChangeHoldingItem(ItemSO item)
    {
        int count = transform.childCount;
        if (count != 0)
            Destroy(transform.GetChild(0).gameObject);
        
        if (item != null)
        {
            GameObject gameObject = Instantiate(item.itemGraphic, transform);
            gameObject.layer = firstPersonRender;
        }
    }

}
