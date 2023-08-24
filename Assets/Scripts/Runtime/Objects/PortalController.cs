using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private int cost = 0;
    private bool _interacted = false;
    Interactable interactable;
    
    private void Awake()
    {
        // Get the Interactable component
        interactable = GetComponentInChildren<Interactable>();
        
        if (ScenesManager.Instance.currentSceneName != "Tutorial")
        {
            // Set the prompt text
            interactable.SetCost(cost);
        }
        else
        {
            interactable.SetCost(0);
        }
    }
    
    private void OnEnable()
    {
        GameManager.OnDiscount += SetCost;
    }
    
    private void OnDisable()
    {
        GameManager.OnDiscount -= SetCost;
    }
    
    public void Interact()
    {
        if (_interacted) return;
        if (ScenesManager.Instance.currentSceneName != "Tutorial" && PlayerController.Instance.GetEssence() < cost)
        {
            interactable.TriggerCannotAfford();
            return;
        }
        
        _interacted = true;

        // Remove the essence from the player
        PlayerController.Instance.SpendEssence(cost);
        
        // Save the player's data
        
        
        // If it is the tutorial, just load the home screen, else load the death screen
        if (GameManager.Instance.isTutorial)
        {
            DataPersistenceManager.Instance.SaveGame();
            // Load the Home scene
            ScenesManager.LoadScene("Home");
        }
        else
        {
            DeathScreenController.Instance.TriggerScreen(true);
        }
    }
    
    public void SetCost(int discount)
    {
        cost -= discount;
        var interactable = GetComponentInChildren<Interactable>();
        interactable.SetCost(cost);
    }
}
