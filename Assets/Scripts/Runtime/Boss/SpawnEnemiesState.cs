using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesState : StateMachineBehaviour
{
    [SerializeField] private float downTime = 6f;
    private static readonly int BulletHell = Animator.StringToHash("BulletHell");
    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<BossEnemySpawner>().SpawnEnemies();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // This will either wait for 6 seconds, or if one of the cores get broken, it will transition to the next state
        // if (animator.GetComponent<BossEnemySpawner>()._justDied)
        // {
        //     animator.SetTrigger(TakeDamage);
        // }
        if (stateInfo.normalizedTime >= downTime)
        {
            animator.SetTrigger(BulletHell);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
