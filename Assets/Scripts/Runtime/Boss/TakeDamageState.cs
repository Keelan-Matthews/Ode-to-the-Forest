using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageState : StateMachineBehaviour
{
    private static readonly int Enrage = Animator.StringToHash("Enrage");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int BulletHell = Animator.StringToHash("BulletHell");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //
    // }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If not all the cores have been destroyed, transition to BulletHell,
        // else if the boss has been defeated, transition to Die
        // else transition to Enrage
        if (animator.GetComponent<BossController>().coresDestroyed < 2)
        {
            animator.SetTrigger(BulletHell);
        }
        else if (animator.GetComponent<BossController>().isDead)
        {
            animator.SetTrigger(Die);
        }
        else
        {
            animator.SetTrigger(Enrage);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //
    // }
}
