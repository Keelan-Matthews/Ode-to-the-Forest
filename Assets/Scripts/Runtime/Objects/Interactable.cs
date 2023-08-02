using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public UnityEvent enterAction;
    public UnityEvent exitAction;
    public GameObject interactPrompt;
    public GameObject interactCost;
    public GameObject parent;
    private bool _interactable = true;
    private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");

    // Update is called once per frame
    private void Update()
    {
        if (!isInRange) return;
        if (Input.GetKeyDown(interactKey))
        {
            interactAction.Invoke();
        }
    }

    public void SetInteractable(bool interact)
    {
        _interactable = interact;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !_interactable) return;
        isInRange = true;
        // Set the interact prompt sprite renderer to active
        interactPrompt.GetComponent<SpriteRenderer>().enabled = true;
        interactCost.GetComponent<SpriteRenderer>().enabled = true;
        
        // Enable the interact outline material
        parent.GetComponent<SpriteRenderer>().material.SetFloat(OutlineThickness, 1f);
        
        enterAction?.Invoke();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isInRange = false;
        // Set the interact prompt sprite renderer to inactive
        interactPrompt.GetComponent<SpriteRenderer>().enabled = false;
        interactCost.GetComponent<SpriteRenderer>().enabled = false;
        
        // Disable the interact outline material
        parent.GetComponent<SpriteRenderer>().material.SetFloat(OutlineThickness, 0f);
        
        exitAction?.Invoke();
    }
}
