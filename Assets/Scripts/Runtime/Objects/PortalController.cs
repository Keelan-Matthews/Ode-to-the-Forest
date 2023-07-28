using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private int cost = 0;
    private bool _interacted = false;
    
    public void Interact()
    {
        if (_interacted) return;
        if (PlayerController.Instance.GetEssence() < cost)
        {
            Debug.Log("Player has " + PlayerController.Instance.GetEssence() + " essence, but needs " + cost + " to use the portal.");
            return;
        }
        
        _interacted = true;

        // Remove the essence from the player
        PlayerController.Instance.SpendEssence(cost);
        
        // Save the player's data
        DataPersistenceManager.Instance.SaveGame();
        
        // Load the Home scene
        ScenesManager.LoadScene("Home");
    }
}
