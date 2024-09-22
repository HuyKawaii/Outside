using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyActionEvent : EnemyActionEvent
{
    RangeEnemyCombat rangeEnemyCombat;

    private void Awake()
    {
        rangeEnemyCombat = GetComponentInParent<RangeEnemyCombat>();
    }

    public void ArrowOutEvent()
    {
        rangeEnemyCombat.SpawnArrow();
    }
}
