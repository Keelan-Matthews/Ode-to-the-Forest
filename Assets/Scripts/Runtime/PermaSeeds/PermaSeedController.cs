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
    private const float SeedTravelSpeed = 10f;

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
        if (col.gameObject.CompareTag("EssenceCollector"))
        {
            // If this is inactive, return
            if (!gameObject.activeSelf) return;
            StartCoroutine(MoveTowardsPlayer(col.gameObject));
        }
        
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            
            // Add the perma seed to the player's inventory
            PermaSeedManager.Instance.AddPermaSeed(permaSeed);
            
            // If it is collected in the WaveRoom, then unlock the doors
            if (GameManager.Instance.activeRoom.gameObject.name == "WaveRoom")
            {
                GameManager.Instance.activeRoom.UnlockRoom();
            }
        }
    }
    
    private IEnumerator MoveTowardsPlayer(GameObject player)
    {
        while (true)
        {
            // Move the essence toward the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, SeedTravelSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
