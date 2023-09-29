using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnrageState : StateMachineBehaviour
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Activate the  sporadic sunlight
        var bossRoom = GameManager.Instance.activeRoom.GetComponent<BossRoomController>();
        bossRoom.ActivateSporadicSunlight();
        animator.GetComponent<BulletHellController>().isEnraged = true;
        AudioManager.PlaySound(AudioManager.Sound.BossEnrage, animator.transform.position);
        animator.GetComponent<BossController>().isEnraged = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //
    // }
}
