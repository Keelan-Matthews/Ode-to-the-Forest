using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class SporadicSunlightController : MonoBehaviour
{
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;

    [SerializeField] private int minNumberOfRings;
    [SerializeField] private int maxNumberOfRings;
    [SerializeField] private bool isBossRoom;
    private int numberOfRings;
    
    [SerializeField] private float minTimeBetweenSpawns;
    [SerializeField] private float maxTimeBetweenSpawns;
    [SerializeField] private float timeBetweenNumberOfRingsChange;
    [SerializeField] private float minLifetime;
    [SerializeField] private float maxLifetime;
    
    [SerializeField] private GameObject sunlightPrefab;
    [SerializeField] private SunlightController sunlightController;

    public bool spawn = true;

    private Room room;

    private void Awake()
    {
        room = GetComponentInParent<Room>();
        UpdateNumberOfRings();
    }

    private void Start()
    {
        if (isBossRoom) return;
        Spawn();
    }

    public void Spawn()
    {
        StartCoroutine(SpawnSunlight());
    }

    private IEnumerator SpawnSunlight()
    {
        while (spawn)
        {
            while (transform.childCount < numberOfRings)
            {
                var radius = Random.Range(minRadius, maxRadius);

                var randomPosition = new Vector2(0, 0);
                var validPosition = false;

                // Try to find a valid position that is not too close to existing circles.
                var maxAttempts = 4;
                while (!validPosition && maxAttempts > 0)
                {
                    if (isBossRoom)
                    {
                        randomPosition = room.GetRandomPositionInLeftHalfOfRoom(radius);
                    }
                    else
                    {
                        randomPosition = room.GetRandomPositionInRoom(radius);
                    }
                    
                    validPosition = IsPositionValid(randomPosition, radius);
                    maxAttempts--;
                }

                if (validPosition)
                {
                    var sunlight = Instantiate(sunlightPrefab, randomPosition, Quaternion.identity);
                    sunlight.transform.SetParent(transform);

                    var softLight = sunlight.GetComponentsInChildren<Light2D>()[0];
                    var hardLight = sunlight.GetComponentsInChildren<Light2D>()[1];
                    var sunlightCollider = sunlight.GetComponentInChildren<CircleCollider2D>();

                    softLight.pointLightOuterRadius = radius;
                    hardLight.pointLightOuterRadius = radius - 0.1f;
                    sunlightCollider.radius = radius * 0.13f;
                    sunlightCollider.GetComponent<SunlightController>().Spawn();

                    sunlight.GetComponent<SunlightDestroyer>().DestroySunlight(Random.Range(minLifetime, maxLifetime));
                }
            }

            // Wait for a random amount of time before checking again.
            var timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private bool IsPositionValid(Vector2 position, float radius)
    {
        // Check if the new position is too close to existing circles.
        foreach (Transform child in transform)
        {
            var distance = Vector2.Distance(position, child.position);
            if (distance < radius + child.GetComponentInChildren<CircleCollider2D>().radius)
            {
                return false; // Position is too close to an existing circle.
            }
        }
    
        return true; // Position is valid.
    }

    private void UpdateNumberOfRings()
    {
        numberOfRings = Random.Range(minNumberOfRings, maxNumberOfRings + 1);
    }
    
    public void Expand()
    {
        // Call expand on each of its children.
        foreach (Transform child in transform)
        {
            child.GetComponentInChildren<SunlightController>().Expand();
        }
        
        sunlightController.BrightenRoomLight();
    }
}
