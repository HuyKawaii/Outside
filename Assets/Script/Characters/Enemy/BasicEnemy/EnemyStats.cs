using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private EnemyCombat enemyCombat;

    private void Awake()
    {
        enemyCombat = GetComponent<EnemyCombat>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void OutOfHealth()
    {
        base.OutOfHealth();
        enemyCombat.Die();
    }
}
