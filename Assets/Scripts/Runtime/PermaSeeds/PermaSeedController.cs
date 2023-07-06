using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermaSeedController : MonoBehaviour
{
    // When this prefab is instantiated, it will become a random perma 
    // seed option
    public PermaSeed permaSeed;
    public List<Sprite> dropSprites;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        permaSeed = PermaSeedManager.Instance.GetRandomPermaSeed();
        
        // Get the drop sprite that has a name with the perma seed in it, making both lowercase
        var dropSprite = dropSprites.Find(sprite => sprite.name.ToLower().Contains(permaSeed.name.ToLower()));
        // Set the sprite of the child sprite renderer to the drop sprite
        spriteRenderer.sprite = dropSprite;
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
