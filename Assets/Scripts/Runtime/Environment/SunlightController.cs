using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunlightController : MonoBehaviour
{
    // Check if the player has collided with the sunlight and update the inSunlight bool
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().inSunlight = true;
        }
    }
    
    // Check if the player has left the sunlight and update the inSunlight bool
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().inSunlight = false;
        }
    }
}
