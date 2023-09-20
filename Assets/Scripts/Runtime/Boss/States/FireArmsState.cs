using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArmsState : StateMachineBehaviour
{
    [SerializeField] private float stateTime = 10f;
    private float _timer;

    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int EnragedBulletHell = Animator.StringToHash("EnragedBulletHell");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(EnragedBulletHell);
        animator.ResetTrigger(TakeDamage);
        animator.GetComponent<FireArmsController>().StartFollowing();
        _timer = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            
            _timer += Time.deltaTime;
            if (_timer >= stateTime)
            {
                animator.SetTrigger(EnragedBulletHell);
            }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //
    // }
}
