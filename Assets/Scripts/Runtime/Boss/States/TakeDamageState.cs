using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageState : StateMachineBehaviour
{
    private static readonly int Enrage = Animator.StringToHash("Enrage");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int BulletHell = Animator.StringToHash("BulletHell");
    
    private bool _canTransition;
    private float _transitionTime = 3f;
    private float _timer;
    private static readonly int EnragedBulletHell = Animator.StringToHash("EnragedBulletHell");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0f;
        _canTransition = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If not all the cores have been destroyed, transition to BulletHell,
        // else if the boss has been defeated, transition to Die
        // else transition to Enrage
        
        var bossController = animator.GetComponent<BossController>();

        if (!_canTransition)
        {
            _timer += Time.deltaTime;
            if (_timer >= _transitionTime)
            {
                _canTransition = true;
            }
        }
        else if (bossController.coresDestroyed < 2)
        {
            animator.SetTrigger(BulletHell);
        } else if (bossController.armsDestroyed < 2 && bossController.isEnraged)
        {
            animator.SetTrigger(EnragedBulletHell);
        }
        else if (bossController.isDead)
        {
            animator.SetTrigger(Die);
        }
        else if (!bossController.isEnraged)
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
