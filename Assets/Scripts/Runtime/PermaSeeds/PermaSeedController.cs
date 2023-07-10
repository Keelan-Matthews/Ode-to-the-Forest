using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermaSeedController : MonoBehaviour
{
    // When this prefab is instantiated, it will become a random perma 
    // seed option
    public PermaSeed permaSeed;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        permaSeed = PermaSeedManager.Instance.GetRandomPermaSeed();
        
        // Set the sprite of the child sprite renderer to the drop sprite
        spriteRenderer.sprite = permaSeed.icon;
    }
    
    public void SetPermaSeed(string seedName)
    {
        permaSeed = PermaSeedManager.Instance.GetSpecificPermaSeed(seedName);
        
        // Set the sprite of the child sprite renderer to the drop sprite
        spriteRenderer.sprite = permaSeed.icon;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            
            // Add the perma seed to the player's inventory
            PlayerController.Instance.AddPermaSeed(permaSeed);
        }
    }
}
