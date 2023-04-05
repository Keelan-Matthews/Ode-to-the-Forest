using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float strength = 10f, delay = 0.15f;
    
    public UnityEvent onBegin, onEnd;
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayFeedback(GameObject sender)
    {
        StopAllCoroutines();
        onBegin?.Invoke();
        
        // Get the sprite renderer of this object and change the color to hexadecimal
        spriteRenderer.color = new Color(1f, 0.8537f, 0.8537f);

        // Get the direction of the sender and apply the force to the receiver
        var direction = (transform.position - sender.transform.position).normalized;
        rb.AddForce(direction * strength, ForceMode2D.Impulse);
        
        // Reset the velocity after a delay
        StartCoroutine(Reset());
    }
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
        spriteRenderer.color = Color.white;
        onEnd?.Invoke();
    }
}
