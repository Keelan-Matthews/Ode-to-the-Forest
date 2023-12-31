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
    
    // Particle effects
    public ParticleSystem rareParticle;
    public ParticleSystem legendaryParticle;
    private int _difficulty;

    private void Awake()
    {
        // Get the difficulty of the current room
        var difficulty = GameManager.Instance.activeRoom.GetDifficulty();
        _difficulty = difficulty;
        permaSeed = PermaSeedManager.Instance.GetRandomPermaSeed(difficulty);

        if (permaSeed == null) return;
        permaSeed.SetSeedNameAndIcon();

        // Set the sprite of the child sprite renderer to the drop sprite
        spriteRenderer.sprite = permaSeed.icon;
    }

    private void Start()
    {
        if (permaSeed.essenceRequired is > 10 and < 20)
        {
            rareParticle.Play();
        }
        else if (permaSeed.essenceRequired >= 20)
        {
            legendaryParticle.Play();
        }
    }

    public void SetPermaSeed(string seedName)
    {
        permaSeed = PermaSeedManager.Instance.GetSpecificPermaSeed(seedName);

        // Set the sprite of the child sprite renderer to the drop sprite
        if (permaSeed == null) return;
        spriteRenderer.sprite = permaSeed.icon;
    }

    public void Interact()
    {
        // Stop the particle effects
        rareParticle.Stop();
        legendaryParticle.Stop();
        Destroy(gameObject);
        
        // If the player already has a perma seed in their inventory,
        // Create a new instance of the perma seed and drop it,
        // before collecting the new perma seed
        if (PermaSeedManager.Instance.HasSeed())
        {
            // Drop the old seed
            var oldSeedName = PermaSeedManager.Instance.GetStoredPermaSeed().seedName;
            GameManager.Instance.DropSpecificPermaSeed(PlayerController.Instance.transform.position, oldSeedName);
            PermaSeedManager.Instance.RemoveStoredPermaSeed();
        }

        // Add the perma seed to the player's inventory
        PermaSeedManager.Instance.AddPermaSeed(permaSeed);

        // If it is collected in the WaveRoom, then unlock the doors
        if (GameManager.Instance.activeRoom.gameObject.name == "WaveRoom")
        {
            GameManager.Instance.activeRoom.UnlockRoom();
        }
        
        AudioManager.PlaySound(AudioManager.Sound.SeedPickup, transform.position);
        
        Debug.Log("Collected " + permaSeed.seedName);
        // If the perma seed name is Seed Of Life, load the home scene after 2 seconds
        if (permaSeed.seedName == "Seed Of Life")
        {
            BossRoomController.Instance.SpawnPortal();
        }
    }
}