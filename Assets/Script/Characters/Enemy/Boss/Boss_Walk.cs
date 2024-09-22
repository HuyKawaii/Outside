using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_Walk : StateMachineBehaviour
{
    BossController bossController;
    BossCombat bossCombat;
    float attackCooldown;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossController = animator.transform.parent.GetComponent<BossController>();
        bossCombat = animator.transform.parent.GetComponent<BossCombat>();
        attackCooldown = 1/bossCombat.myStats.statData.attackSpeed.GetValue();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("speed", UtilFunc.GetHorizontalSpeed(bossController.velocity) / bossController.walkingSpeed);
        bossController.agent.destination = bossController.target.position;
        bossController.agent.speed = bossController.walkingSpeed;
        bossController.FacePlayer();
        attackCooldown -= Time.fixedDeltaTime;

        Debug.Log("Special attack state: " + bossCombat.IsSpecialAttackReady());
        if (bossCombat.IsSpecialAttackReady())
        {
            animator.SetTrigger("special");
            return;
        }

        if (bossController.distance < bossCombat.attackRange && attackCooldown < 0)
            animator.SetTrigger("attack");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossController.agent.speed = 0;
        animator.ResetTrigger("attack");
        animator.ResetTrigger("special");
    }
}
