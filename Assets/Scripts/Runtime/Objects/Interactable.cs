using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject interactText;
    public GameObject interactCost;
    public GameObject interactCostText;
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
    
    public void SetCost(int cost)
    {
        interactCostText.GetComponent<TextMeshPro>().text = cost.ToString();
        
        if (isInRange)
        {
            interactCost.GetComponent<SpriteRenderer>().enabled = true;
            interactCostText.GetComponent<TextMeshPro>().enabled = true;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !_interactable) return;
        isInRange = true;
        // Set the interact prompt sprite renderer to active
        interactPrompt.GetComponent<SpriteRenderer>().enabled = true;
        
        // If the cost is 0, disable the cost text
        if (interactCostText.GetComponent<TextMeshPro>().text == "0")
        {
            interactCost.GetComponent<SpriteRenderer>().enabled = false;
            interactCostText.GetComponent<TextMeshPro>().enabled = false;
        }
        else
        {
            interactCost.GetComponent<SpriteRenderer>().enabled = true;
            interactCostText.GetComponent<TextMeshPro>().enabled = true;
        }
        // Enable interact text
        interactText.GetComponent<TextMeshPro>().enabled = true;

        // Enable the interact outline material
        parent.GetComponent<SpriteRenderer>().material.SetFloat(OutlineThickness, 1f);
        
        enterAction?.Invoke();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        DisableInteraction();
        
        exitAction?.Invoke();
    }
    
    // This function disables interaction immediately
    public void DisableInteraction()
    {
        isInRange = false;
        interactPrompt.GetComponent<SpriteRenderer>().enabled = false;
        interactCost.GetComponent<SpriteRenderer>().enabled = false;
        // Disable interact text
        interactText.GetComponent<TextMeshPro>().enabled = false;
        interactCostText.GetComponent<TextMeshPro>().enabled = false;
        
        // Disable the interact outline material
        parent.GetComponent<SpriteRenderer>().material.SetFloat(OutlineThickness, 0f);
    }
    
    public void SetPromptText(string text)
    {
        interactText.GetComponent<TextMeshPro>().text = text;
    }
}
