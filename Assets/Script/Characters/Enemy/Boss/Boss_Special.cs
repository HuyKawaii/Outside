using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Special : StateMachineBehaviour
{
    BossCombat bossCombat;
    BossController bossController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossCombat = animator.transform.parent.GetComponent<BossCombat>();
        bossController = animator.transform.parent.GetComponent<BossController>();
        bossCombat.ResetSpecialAttack();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}
}
