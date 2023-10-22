using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArmsState : StateMachineBehaviour
{
    [SerializeField] private float stateTime = 10f;
    private float _timer;
    private int _armsDestroyed;

    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int EnragedBulletHell = Animator.StringToHash("EnragedBulletHell");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Enrage = Animator.StringToHash("Enrage");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0f;
        animator.ResetTrigger(EnragedBulletHell);
        animator.ResetTrigger(TakeDamage);
        animator.ResetTrigger(Enrage);
        animator.GetComponent<FireArmsController>().StartFollowing();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // This will either wait for 6 seconds, or if one of the cores get broken, it will transition to the next state
            if (_armsDestroyed != animator.GetComponent<BossController>().armsDestroyed)
            {
                _armsDestroyed = animator.GetComponent<BossController>().armsDestroyed;
                
                if (_armsDestroyed == 2)
                {
                    animator.GetComponent<BossController>().isDead = true;
                    animator.SetTrigger(Die);
                }
                else
                {
                    animator.SetTrigger(TakeDamage);
                }
            }
            else if (_timer >= stateTime && !BossController.Instance.isDead && !PlayerController.Instance.IsDead())
            {
                animator.SetTrigger(EnragedBulletHell);
            }
            else
            {
                _timer += Time.deltaTime;
            }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0f;
    }
}
