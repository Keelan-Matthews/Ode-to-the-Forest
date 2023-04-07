using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunlightController : MonoBehaviour
{
    public Light2D hardLight;
    public Light2D softLight;
    public Light2D roomLight;
    public Collider2D roomCollider;

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

    public void Expand()
    {
        // Gradually increase the intensity of the room light while
        // gradually decreasing the intensity of the hard and soft lights
        StartCoroutine(BrightenRoomLightCoroutine());
        StartCoroutine(DimHardLightCoroutine());
        StartCoroutine(DimSoftLightCoroutine());
        
        // Enable the room collider
        roomCollider.enabled = true;
        
        // Disable the sunlight collider
        GetComponent<Collider2D>().enabled = false;
    }
    
    private IEnumerator BrightenRoomLightCoroutine()
    {
        while (roomLight.intensity < 1)
        {
            roomLight.intensity += Time.deltaTime;
            yield return null;
        }
        
        while (roomLight.intensity < 1.4)
        {
            roomLight.intensity += Time.deltaTime * 2;
            yield return null;
        }

        // Bring the light back down to 0.8
        while (roomLight.intensity > 0.8)
        {
            roomLight.intensity -= Time.deltaTime * 2;
            yield return null;
        }
    }
    
    private IEnumerator DimHardLightCoroutine()
    {
        while (hardLight.intensity > 0)
        {
            hardLight.intensity -= Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator DimSoftLightCoroutine()
    {
        while (softLight.intensity > 0)
        {
            softLight.intensity -= Time.deltaTime;
            yield return null;
        }
    }
}
