using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellState : StateMachineBehaviour
{
    // There are currently 2 different attacks
    private int _attackNumber = 2;

    private static readonly int SpawnEnemies = Animator.StringToHash("SpawnEnemies");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Alternate between setting attack number to 1 and 2
        _attackNumber = _attackNumber == 1 ? 2 : 1;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BulletHellController>().cycleEnded)
        {
            animator.SetTrigger(SpawnEnemies);
            return;
        }
        animator.GetComponent<BulletHellController>().Shoot(_attackNumber);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackNumber = 2;
    }
}
