using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnragedBulletHellState : StateMachineBehaviour
{
    // There are currently 2 different attacks
    private int _attackNumber = 4;
    
    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int FireArms = Animator.StringToHash("FireArms");
    private static readonly int Enrage = Animator.StringToHash("Enrage");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the spawn enemies trigger
        animator.ResetTrigger(FireArms);
        animator.ResetTrigger(Enrage);
        // Alternate between setting attack number to 1 and 2
        _attackNumber = _attackNumber == 3 ? 4 : 3;
        animator.GetComponent<BulletHellController>().cycleEnded = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BulletHellController>().cycleEnded)
        {
            animator.SetTrigger(FireArms);
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
