using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private int cost = 0;
    
    public void Interact()
    {
        if (PlayerController.Instance.GetEssence() < cost)
        {
            Debug.Log("Player has " + PlayerController.Instance.GetEssence() + " essence, but needs " + cost + " to use the portal.");
            return;
        }
        
        // Remove the essence from the player
        PlayerController.Instance.SpendEssence(cost);
        
        // Load the Home scene
        ScenesManager.LoadScene("Home");
    }
}
