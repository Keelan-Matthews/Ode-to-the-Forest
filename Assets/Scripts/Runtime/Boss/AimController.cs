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
        GetComponentInChildren<CapsuleCollider2D>().enabled = enabled;
    }

    public void TakeDamage(int damage)
    {
        fireArmsController.TakeDamage(damage);
        // Flash red
        StartCoroutine(FlashRed());
    }
    
    private IEnumerator FlashRed()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        fireArmsController.playerIsInsideAim = false;
    }
}
