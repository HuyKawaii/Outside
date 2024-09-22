using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionEvent : MonoBehaviour
{
    AttackPos enemyAttackPos;
    [SerializeField] EnemyController enemyController;

    private void Awake()
    {
        enemyAttackPos = transform.parent.GetComponentInChildren<AttackPos>();
        enemyController = transform.parent.GetComponent<EnemyController>();
    }

    public void ActiveAttackHitbox()
    {
        enemyAttackPos.GetComponent<Collider>().enabled = true;
    }

    public void DeactiveAttackHitbox()
    {
        enemyAttackPos.GetComponent<Collider>().enabled = false;
    }

    public void StopAttackOnController()
    {
        enemyController.StopAttack();
    }
}
