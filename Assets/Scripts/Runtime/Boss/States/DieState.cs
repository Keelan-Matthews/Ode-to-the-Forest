using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : StateMachineBehaviour
{
    private static readonly int FireArms = Animator.StringToHash("FireArms");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset fire arms trigger
        animator.ResetTrigger(FireArms);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the boss controller
        var bossController = animator.GetComponent<BossController>();
        // Drop the seed of life
        bossController.DropSeedOfLife();
        // Destroy the boss
        Destroy(animator.gameObject);
    }
}
