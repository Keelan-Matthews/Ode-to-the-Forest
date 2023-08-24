using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmanacController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private void Awake()
    {
        // Get the Interactable component
        var interactable = GetComponentInChildren<Interactable>();
        
        // Set the prompt text
        interactable.SetCost(0);
    }
    
    public void Interact()
    {
        
    }
    
    public void OpenBook()
    {
        animator.SetTrigger("OpenBook");
    }
    
    public void CloseBook()
    {
        animator.SetTrigger("CloseBook");
    }
}
