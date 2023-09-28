using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private FireArmsController fireArmsController;
    [SerializeField] private RuntimeAnimatorController[] armStates;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        fireArmsController.playerIsInsideAim = true;
    }

    public void SetArmState(int index)
    {
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = armStates[index];
    }
    
    public void EnableCollider()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void TakeDamage(int damage)
    {
        fireArmsController.TakeDamage(damage);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        fireArmsController.playerIsInsideAim = false;
    }
}
