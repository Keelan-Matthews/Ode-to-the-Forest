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

    private float _interactCooldown = 1f;
    private bool _canInteract = true;
    
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
        
        if (isInRange && cost != 0)
        {
            interactCost.GetComponent<SpriteRenderer>().enabled = true;
            interactCostText.GetComponent<TextMeshPro>().enabled = true;
        }
        else
        {
            interactCost.GetComponent<SpriteRenderer>().enabled = false;
            interactCostText.GetComponent<TextMeshPro>().enabled = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enterAction?.Invoke();
        }
        
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
    
    // This function makes all the text flash red for a second
    public IEnumerator FlashRed()
    {
        // Gradually set the interact text to red over 0.5 seconds
        for (float i = 0; i < 0.1f; i += Time.deltaTime)
        {
            interactText.GetComponent<TextMeshPro>().color = Color.Lerp(Color.white, Color.red, i / 0.5f);
            interactCostText.GetComponent<TextMeshPro>().color = Color.Lerp(Color.white, Color.red, i / 0.5f);
            yield return null;
        }

        // Gradually set the interact text to white over 0.5 seconds
        for (float i = 0; i < 0.1f; i += Time.deltaTime)
        {
            interactText.GetComponent<TextMeshPro>().color = Color.Lerp(Color.red, Color.white, i / 0.5f);
            interactCostText.GetComponent<TextMeshPro>().color = Color.Lerp(Color.red, Color.white, i / 0.5f);
            yield return null;
        }
        
        // Set the interact text to white
        interactText.GetComponent<TextMeshPro>().color = Color.white;
        interactCostText.GetComponent<TextMeshPro>().color = Color.white;
    }
    
    // This coroutine moves the interact text right slightly and then back to its original position in 0.25 seconds
    public IEnumerator MoveTextRight()
    {
        var offset = 0.03f;
        // Move the interact text right slightly over 0.25 seconds
        for (float i = 0; i < 0.1f; i += Time.deltaTime)
        {
            var position = interactText.transform.position;
            position = Vector3.Lerp(position, new Vector3(position.x + offset, position.y, position.z), i / 0.25f);
            interactText.transform.position = position;
            var position1 = interactCostText.transform.position;
            position1 = Vector3.Lerp(position1, new Vector3(position1.x + offset, position1.y, position1.z), i / 0.25f);
            interactCostText.transform.position = position1;
            
            // Do the same for interact cost and interact prompt
            var position2 = interactCost.transform.position;
            position2 = Vector3.Lerp(position2, new Vector3(position2.x + offset, position2.y, position2.z), i / 0.25f);
            interactCost.transform.position = position2;
            var position3 = interactPrompt.transform.position;
            position3 = Vector3.Lerp(position3, new Vector3(position3.x + offset, position3.y, position3.z), i / 0.25f);
            interactPrompt.transform.position = position3;
            
            yield return null;
        }

        // Move the interact text back to its original position over 0.25 seconds
        for (float i = 0; i < 0.1f; i += Time.deltaTime)
        {
            var position = interactText.transform.position;
            position = Vector3.Lerp(position, new Vector3(position.x - offset, position.y, position.z), i / 0.25f);
            interactText.transform.position = position;
            var position1 = interactCostText.transform.position;
            position1 = Vector3.Lerp(position1, new Vector3(position1.x - offset, position1.y, position1.z), i / 0.25f);
            interactCostText.transform.position = position1;
            
            // Do the same for interact cost and interact prompt
            var position2 = interactCost.transform.position;
            position2 = Vector3.Lerp(position2, new Vector3(position2.x - offset, position2.y, position2.z), i / 0.25f);
            interactCost.transform.position = position2;
            var position3 = interactPrompt.transform.position;
            position3 = Vector3.Lerp(position3, new Vector3(position3.x - offset, position3.y, position3.z), i / 0.25f);
            interactPrompt.transform.position = position3;
            
            yield return null;
        }
    }

    public void TriggerCannotAfford()
    {
        if (!_canInteract) return;
        StartCoroutine(FlashRed());
        StartCoroutine(MoveTextRight());

        StartCoroutine(Cooldown());
    }

    public bool IsInteractable()
    {
        return _canInteract;
    }
    
    // This coroutine disables interaction for 0.5 seconds
    private IEnumerator Cooldown()
    {
        _canInteract = false;
        yield return new WaitForSeconds(_interactCooldown);
        _canInteract = true;
    }
}
