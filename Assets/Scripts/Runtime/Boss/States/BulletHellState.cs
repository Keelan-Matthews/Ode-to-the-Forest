using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellState : StateMachineBehaviour
{
    // There are currently 2 different attacks
    private int _attackNumber = 2;

    private static readonly int SpawnEnemies = Animator.StringToHash("SpawnEnemies");
    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the spawn enemies trigger
        animator.ResetTrigger(SpawnEnemies);
        animator.ResetTrigger(TakeDamage);
        // Alternate between setting attack number to 1 and 2
        _attackNumber = _attackNumber == 1 ? 2 : 1;
        animator.GetComponent<BulletHellController>().cycleEnded = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BulletHellController>().cycleEnded)
        {
            animator.SetTrigger(SpawnEnemies);
        }
        else
        {
            animator.GetComponent<BulletHellController>().Shoot(_attackNumber);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<BulletHellController>().cycleEnded = false;
        animator.GetComponent<BulletHellController>().isShooting = false;
    }
}
