using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntitySpanwer : Interactalble
{
    public GameObject entity;

    public override void Interact(Transform player)
    {
        MultiplayerManager.Instance.SpawnEnemyMultiplayer(entity, transform.position + Vector3.down);
    }

    protected override void SetHoverText()
    {
        if (entity != null)
        {
            hoverText = "Spawn " + entity.name;
        }
    }

    protected override void SnapToGround()
    {
    }
}
