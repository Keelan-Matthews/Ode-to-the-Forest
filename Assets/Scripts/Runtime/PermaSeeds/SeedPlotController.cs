using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPlotController : MonoBehaviour
{
    private bool _isPlanted;
    private bool _isGrown;
    private PermaSeed _permaSeed;
    
    private Animator _animator;
    
    private void Start()
    {
        // Get the animator in the child
        _animator = GetComponentInChildren<Animator>();
    }
    public void Interact()
    {
        // If the seed hasn't been planted yet, plant it
        // else if it has been planted but hasn't grown yet, 
        // check if the player has enough essence to grow it
        // if it has been grown, then interacting with it should 
        // uproot it
        Debug.Log("Player has interacted with a seed plot.");
        if (!_isPlanted)
        {
            // Check if the player has a permaSeed
            if (!PermaSeedManager.Instance.HasSeed())
            {
                Debug.Log("Player has no seed to plant.");
                return;
            }

            // Plant the seed in the plot
            _permaSeed = PermaSeedManager.Instance.PlantSeed();
            _isPlanted = true;
            
            Debug.Log("Player has planted a seed.");
            
            var animationName = $"Plant{_permaSeed.name}";
            
            // Play the plant animation
            _animator.SetTrigger(animationName);
        }
        else if (!_isGrown)
        {
            // Try grow the plant
            if (_permaSeed.Grow(HomeRoomController.Instance.GetEssence()))
            {
                _isGrown = true;
                // Subtract essenceRequired from the home Essence
                HomeRoomController.Instance.SpendEssence(_permaSeed.essenceRequired);
                
                // Add the seed to activePermaSeeds
                PermaSeedManager.Instance.AddActiveSeed(_permaSeed);
                
                Debug.Log("Player has grown a seed.");
                
                var animationName = $"Grow{_permaSeed.name}";
                
                // Play the grow animation
                _animator.SetTrigger(animationName);
            }
            else
            {
                // If it hasn't grown, do nothing
                Debug.Log("Player has " + PlayerController.Instance.GetEssence() + " essence, but needs " + _permaSeed.essenceRequired + " to grow a seed.");
            }
        }
        else
        {
            // Some kind of "Are you sure?" prompt
            
            
            // Uproot the seed
            _isPlanted = false;
            _isGrown = false;
            
            // Remove the seed from activePermaSeeds
            PermaSeedManager.Instance.UprootSeed(_permaSeed);
            
            var animationName = $"Uproot{_permaSeed.name}";
            
            // Play the uproot animation
            _animator.SetTrigger(animationName);
            
            _permaSeed = null;
            
            Debug.Log("Player has uprooted a seed.");
        }
    }
}
