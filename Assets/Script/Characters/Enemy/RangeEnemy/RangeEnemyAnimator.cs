using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyAnimator : CharacterAnimator
{
    private EnemyCombat enemyCombat;

    protected override void Start()
    {
        base.Start();
        enemyCombat = GetComponent<EnemyCombat>();
        enemyCombat.initiateAttackCallback += SetAttackTrigger;
    }

    public void SetAttackTrigger()
    {
        characterAnimator.SetTrigger("bow_attack");
        //Debug.Log("Play attack animation");
    }
}
