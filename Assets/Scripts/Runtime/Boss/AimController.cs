using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private FireArmsController fireArmsController;
    [SerializeField] private RuntimeAnimatorController[] armStates;
    private static readonly int Expose = Animator.StringToHash("Expose");

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        fireArmsController.playerIsInsideAim = true;
    }

    public void SetArmState(int index, bool expose = false)
    {
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = armStates[index];

        if (expose)
        {
            animator.SetTrigger(Expose);
        }
    }
    
    public void EnableCollider(bool enabled)
    {
        GetComponentInChildren<BoxCollider2D>().enabled = enabled;
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
