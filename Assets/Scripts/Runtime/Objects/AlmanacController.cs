using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmanacController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject canvasBook;
    private bool _open = false;
    
    // Cooldown for the book opening animation
    private float _cooldown = 0.5f;
    
    private void Awake()
    {
        // Get the Interactable component
        var interactable = GetComponentInChildren<Interactable>();
        
        // Set the prompt text
        interactable.SetCost(0);
    }
    
    public void Interact()
    {
        if (_open) return;
        canvasBook.SetActive(true);
        StartCoroutine(OpenCooldown());
    }

    private IEnumerator OpenCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _open = true;
    }
    
    private IEnumerator CloseCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _open = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_open) return;
            canvasBook.SetActive(false);
            StartCoroutine(CloseCooldown());
        }
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
