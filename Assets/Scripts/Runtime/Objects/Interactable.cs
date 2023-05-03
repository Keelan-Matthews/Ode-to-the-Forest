using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public GameObject interactPrompt;
    
    // Update is called once per frame
    private void Update()
    {
        if (!isInRange) return;
        if (Input.GetKeyDown(interactKey))
        {
            interactAction.Invoke();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isInRange = true;
        // Set the interact prompt sprite renderer to active
        interactPrompt.GetComponent<SpriteRenderer>().enabled = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isInRange = false;
        // Set the interact prompt sprite renderer to inactive
        interactPrompt.GetComponent<SpriteRenderer>().enabled = false;
    }
}
