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

    private int numRings = 4;
    private float ringRadius = 0.7f;
    private float ringWidth = 0.7f;
    private float ringIntensity = 0.5f;
    private float ringFalloff = 1;

    private void Awake()
    {
        // Get the global light
        _globalLight = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
        // InitializeSunlightRings();
    }

    /**
     *  This function gets the position of the hard light. It then creates concentric rings around the hard light,
     *  with a radius of ringRadius and a width of ringWidth. The number of rings is determined by numRings.
     *  The hardLight intensity is set to ringIntensity, and the falloff is set to ringFalloff.
     * The falloff determines the light intensity of the rings, with a falloff of 0 meaning the intensity is constant
     * across the ring, and a falloff of 1 meaning the intensity is 1 at the center of the ring and 0.2 at the edge of the ring.
     */
    private void InitializeSunlightRings()
    {
        // Get the position of the hard light
        var hardLightPosition = transform.position;

        // Create concentric rings around the hard light
        for (var i = 1; i <= numRings; i++)
        {
            var ringRadiusI = ringRadius + i * ringWidth;

            // Set the intensity of the ring
            var ringIntensityI = ringIntensity * Mathf.Pow(ringFalloff, i - 1);

            // Create the ring
            var ring = new GameObject("Ring" + i)
            {
                transform =
                {
                    position = transform.position,
                }
            };
            ring.AddComponent<Light2D>();
            var ringLight = ring.GetComponent<Light2D>();

            // Set the properties of the ring
            ringLight.color = hardLight.color;
            ringLight.intensity = ringIntensityI;
            ringLight.falloffIntensity = hardLight.falloffIntensity;
            ringLight.pointLightInnerRadius = hardLight.pointLightInnerRadius + ringWidth / 2f;
            ringLight.pointLightOuterRadius = ringRadiusI;
            ringLight.shadowIntensity = hardLight.shadowIntensity;
            
            // Update the culling mask of the ring
            
            
            // Set the ring as a child of the hard light
            ring.transform.parent = transform;
        }
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
        yield return new WaitForSeconds(2);
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
        StartCoroutine(DamageDelay(player));
    }
    
    private IEnumerator DamageDelay(PlayerController player)
    {
        if (CheckInSunlight(player)) yield break;
        yield return new WaitForSeconds(3);
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
}
