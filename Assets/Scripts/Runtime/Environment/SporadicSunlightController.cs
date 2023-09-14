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
    
    [SerializeField] private GameObject sunlightPrefab;

    private Room room;
    
    // This class will randomly spawn x number of sunlightPrefabs with random radii.
    // They will spawn within the bounds of the room.
    // The centre of a prefab must be at least the maxRadius distance away from the wall.
    
    // The sunlight prefab has 2 light children. The SoftLight child and the SunlightCollider must
    // be the desired radius. The HardLight must be slightly smaller than the SoftLight.

    private void Awake()
    {
        room = GetComponentInParent<Room>();
    }

    private void OnEnable()
    {
        SpawnSunlight();
    }
    
    private bool IsTooCloseToAnotherSunlight(GameObject sunlight, float radius)
    {
        var colliders = Physics2D.OverlapCircleAll(sunlight.transform.position, radius);

        foreach (var c in colliders)
        {
            if (c.CompareTag("SunlightCollider") && c.gameObject != sunlight.GetComponentInChildren<CircleCollider2D>().gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnSunlight()
    {
        var numberOfRings = Random.Range(minNumberOfRings, maxNumberOfRings + 1);
        for (var i = 0; i < numberOfRings; i++)
        {
            var radius = Random.Range(minRadius, maxRadius);
            
            var sunlight = Instantiate(sunlightPrefab, transform.position, Quaternion.identity);
            sunlight.transform.SetParent(transform);
            
            // Get the children of the sunlight prefab.
            var softLight = sunlight.GetComponentsInChildren<Light2D>()[0];
            var hardLight = sunlight.GetComponentsInChildren<Light2D>()[1];
            var sunlightCollider = sunlight.GetComponentInChildren<CircleCollider2D>();
            
            softLight.pointLightOuterRadius = radius;
            hardLight.pointLightOuterRadius = radius - 0.1f;
            sunlightCollider.radius = radius;

            var randomPosition = room.GetRandomPositionInRoom(radius);
            
            // If the random position is too close to the wall, try again.
            var maxDistanceIterations = 100;
            while (Vector2.Distance(randomPosition, transform.position) < maxRadius && maxDistanceIterations > 0)
            {
                randomPosition = room.GetRandomPositionInRoom(radius);
                maxDistanceIterations--;
            }
            
            sunlight.transform.position = randomPosition;
            
            // If the sunlight is too close to another sunlight, try again.
            var iterations = 100;
            while (IsTooCloseToAnotherSunlight(sunlight, radius) && iterations > 0)
            {
                randomPosition = room.GetRandomPositionInRoom(radius);
                sunlight.transform.position = randomPosition;
                iterations--;
            }
        }
    }
}
