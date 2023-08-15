using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunlightController : MonoBehaviour
{
    public Light2D hardLight;
    public Light2D softLight;
    public Light2D roomLight;
    public Collider2D roomCollider;
    private Light2D _globalLight;
    
    private float _damageDelay = 2f;

    private void Awake()
    {
        // Get the global light
        _globalLight = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
    }

    // Check if the player has left the sunlight and update the inSunlight bool
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Don't start the coroutine if SunlightCollider is inactive
        if (!gameObject.activeSelf) return;
        StartCoroutine(DamagePlayerCoroutine(other.GetComponent<PlayerController>()));
    }

    private IEnumerator DamagePlayerCoroutine(PlayerController player)
    {
        // Don't start the coroutine if SunlightCollider is inactive
        if (!gameObject.activeSelf) yield break;
        // Get timeInDarkness from Room script
        var timeInDarkness = GameManager.Instance.activeRoom.timeInDarkness;
        // Don't start the coroutine if SunlightCollider is inactive
        if (!gameObject.activeSelf) yield break;
        // Wait 2 seconds
        yield return new WaitForSeconds(_damageDelay);
        // dim the global light and then damage the player
        var time = 0f;
        var currentIntensity = _globalLight.intensity;
        while (time < timeInDarkness)
        {
            if (CheckInSunlight(player)) yield break;
            time += Time.deltaTime / 2f;
            _globalLight.intensity = Mathf.Lerp(currentIntensity, 0.1f, time);
            yield return null;
        }

        DamagePlayer(player);
    }

    private void DamagePlayer(PlayerController player)
    { 
        player.TakeDamage(1);
        
        // Set the damage delay to 3 if the player is corrupted
        if (player.isCorrupted)
            _damageDelay = 4f;
        else
            _damageDelay = 2f;
        StartCoroutine(DamageDelay(player));
    }
    
    private IEnumerator DamageDelay(PlayerController player)
    {
        if (CheckInSunlight(player)) yield break;
        yield return new WaitForSeconds(_damageDelay);
        if (CheckInSunlight(player)) yield break;
        DamagePlayer(player);
    }

    private bool CheckInSunlight(PlayerController player)
    {
        if (!player.inSunlight) return false;
        _globalLight.intensity = 0.3f;
        return true;
    }

    public void Expand()
    {
        // Gradually increase the intensity of the room light while
        // gradually decreasing the intensity of the hard and soft lights
        StartCoroutine(BrightenRoomLightCoroutine(0.8f));
        StartCoroutine(DimHardLightCoroutine());
        StartCoroutine(DimSoftLightCoroutine());
        
        // Enable the room collider
        roomCollider.enabled = true;
        
        // Disable the sunlight collider
        GetComponent<Collider2D>().enabled = false;
    }

    public void LightRoomUpgradeObelisk()
    {
        // Increase the intensity of the room light
        StartCoroutine(BrightenRoomLightCoroutine(1.5f));
        // Change the colour of the room light to compensate for the green grass (gradually)
        var newColour = new Color(0.2295f, 0.4131f, 0.6320f);
        StartCoroutine(ChangeRoomLightColourCoroutine(newColour));
    }
    
    private IEnumerator ChangeRoomLightColourCoroutine(Color newColour)
    {
        if (roomLight == null) yield break;
        // Gradually change the colour of the room light to the desired colour
        while (roomLight.color != newColour)
        {
            roomLight.color = Color.Lerp(roomLight.color, newColour, Time.deltaTime * 2);
            yield return null;
        }

        // Set the colour of the room light to the desired colour
        roomLight.color = newColour;
    }
    
    public void LightRoomDowngradeObelisk()
    {
        // Increase the intensity of the room light,
        // while changing its colour to red
        StartCoroutine(BrightenRoomLightCoroutine(1.1f));
        roomLight.color = new Color(1f, 0.5f, 0.5f);
    }

    private IEnumerator BrightenRoomLightCoroutine(float intensity)
    {
        if (roomLight == null) yield break;
        // Gradually increase the light intensity to the desired value
        while (roomLight.intensity < intensity)
        {
            roomLight.intensity += Time.deltaTime * 2;
            yield return null;
        }

        // Set the light intensity to the desired value
        roomLight.intensity = intensity;

        // Hold the light at the desired intensity for a short period of time (e.g., 0.5 seconds)
        float flashDuration = 0.5f;
        float flashTimer = 0f;
        while (flashTimer < flashDuration)
        {
            flashTimer += Time.deltaTime;
            yield return null;
        }

        // Gradually decrease the light intensity back to the desired value
        while (roomLight.intensity > intensity)
        {
            roomLight.intensity -= Time.deltaTime * 3;
            yield return null;
        }

        // Set the light intensity to the desired value
        roomLight.intensity = intensity;
    }

    
    private IEnumerator DimHardLightCoroutine()
    {
        if (hardLight == null) yield break;
        
        // Gradually dim the light and then disable it afterwards
        while (hardLight.intensity > 0)
        {
            hardLight.intensity -= Time.deltaTime;
            yield return null;
        }
        hardLight.enabled = false;
    }
    
    private IEnumerator DimSoftLightCoroutine()
    {
        if (softLight == null) yield break;
        while (softLight.intensity > 0)
        {
            softLight.intensity -= Time.deltaTime;
            yield return null;
        }
        softLight.enabled = false;
    }
    
    public void DecreaseRadius()
    {
        const float modifier = 0.8f;
        // Decrease the radius of the sunlight collider immediately
        if (GetComponent<CircleCollider2D>() == null) return;
        GetComponent<CircleCollider2D>().radius *= modifier;
        
        // Update the soft and hard light radius to match the sunlight collider immediately
        hardLight.pointLightOuterRadius *= modifier;
        softLight.pointLightOuterRadius *= modifier;
    }
    
    public void IncreaseRadius()
    {
        const float modifier = 1.2f;
        // Increase the radius of the sunlight collider immediately
        if (GetComponent<CircleCollider2D>() == null) return;
        GetComponent<CircleCollider2D>().radius *= modifier;
        
        // Update the soft and hard light radius to match the sunlight collider immediately
        hardLight.pointLightOuterRadius *= modifier;
        softLight.pointLightOuterRadius *= modifier;
    }
}
