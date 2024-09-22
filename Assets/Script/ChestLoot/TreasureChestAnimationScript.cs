using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestAnimationScript : MonoBehaviour
{
    TreasureChest chest;

    private void Awake()
    {
        chest = GetComponentInParent<TreasureChest>();
    }

    public void OpenChestAnimationFinished()
    {
        Debug.Log("Animation Finished");
        chest.RevealLoot();
    }
}
