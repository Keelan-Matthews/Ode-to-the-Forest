using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float strength = 50f, delay = 0.15f;
    
    public UnityEvent onBegin, onEnd;
    private SpriteRenderer _spriteRenderer;
    private Vector2 originalPosition;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void PlayFeedback(Transform sender)
    {
        if (sender == null) return;
        StopAllCoroutines();
        onBegin?.Invoke();

        var direction = (transform.position - sender.position).normalized;
        StartCoroutine(Knockback(direction));
        onEnd?.Invoke();
    }
    
    private IEnumerator Knockback(Vector2 direction)
    {
        var elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            var force = Mathf.Lerp(0f, strength, elapsedTime / delay);
            transform.position += (Vector3)(direction * (force * Time.fixedDeltaTime));
            // Set z to 0
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetKnockback(float strength)
    {
        this.strength = strength;
    }

    // public void PlayFeedback(GameObject sender)
    // {
    //     StopAllCoroutines();
    //     onBegin?.Invoke();
    //     
    //     // // Get the sprite renderer of this object and change the color to hexadecimal
    //     // spriteRenderer.color = new Color(0.990566f, 0.4345407f, 0.4345407f);
    //     // Get the direction of the sender and apply the force to the receiver
    //     var direction = (transform.position - sender.transform.position).normalized;
    //     rb.AddForce(new Vector2(direction.x, direction.y) * strength, ForceMode2D.Impulse);
    //
    //     // Reset the velocity after a delay
    //     StartCoroutine(Reset());
    // }
    // private IEnumerator Reset()
    // {
    //     yield return new WaitForSeconds(delay);
    //     rb.velocity = Vector2.zero;
    //     // spriteRenderer.color = Color.white;
    //     onEnd?.Invoke();
    // }
    //
    // public void SetKnockback(float strength)
    // {
    //     this.strength = strength;
    // }
}
