using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Sprites")]
    [SerializeField] private GameObject[] clouds;
    
    [Header("Cloud Spawn Settings")]
    [SerializeField] private float cloudSpeed = 0.5f;
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 6f;
    [SerializeField] private float cloudOffset = 1f;
    [SerializeField] private int probability = 5;
    
    private bool _spawnedCloud = false;
    private bool _setSpawnTime = false;
    private Room _room;
    private float nextSpawnTime;
    
    private void Awake()
    {
        // Get the room that this spawner is in
        _room = GetComponentInParent<Room>();
        
        // There is a 60% chance that _spawnedCloud will be true
        _spawnedCloud = UnityEngine.Random.Range(0, 10) > probability;
    }

    private void Update()
    {
        // If this is not the active room, return
        if (_room != GameManager.Instance.activeRoom) return;
        
        if (!_setSpawnTime)
        {
            // Set the next spawn time
            nextSpawnTime = Time.time + UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
            _setSpawnTime = true;
        }

        if (GameManager.Instance.activeRoom.IsCleared() || GameManager.Instance.isTutorial || _spawnedCloud) return;
        
        // Spawn a cloud at a random time
        if (Time.time > nextSpawnTime)
        {
            SpawnCloud();
            _spawnedCloud = true;
        }
    }

    public void SpawnCloud()
    {
        // This will spawn a cloud at the offset position from the spawner.
        // The cloud will need to travel to the opposite side of the screen.
        // The cloud needs to be placed, and travel in such a way that it looks like it is moving in a straight line.
        // the placement is dependent on the rotation of the cloud
        
        // Choose a random cloud 
        var cloud = clouds[UnityEngine.Random.Range(0, clouds.Length)];
        // Spawn the cloud in a random position along the offset radius
        var randomSpawnPosition = transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle.normalized) * cloudOffset;
        var spawnedCloud = Instantiate(cloud, randomSpawnPosition, Quaternion.identity);
        spawnedCloud.transform.SetParent(transform);
        
        CalculateTrajectory(spawnedCloud);
    }

    private void CalculateTrajectory(GameObject spawnedCloud)
    {
        // Move the cloud straight through the centre of the spawner
        var cloudTrajectory = transform.position - spawnedCloud.transform.position;
        // Get the angle of the trajectory
        var cloudAngle = Mathf.Atan2(cloudTrajectory.y, cloudTrajectory.x) * Mathf.Rad2Deg;
        // Rotate the cloud to face the correct direction
        spawnedCloud.transform.rotation = Quaternion.AngleAxis(cloudAngle, Vector3.forward);
        // Calculate the end position of the cloud
        var endPosition = transform.position + cloudTrajectory.normalized * (cloudOffset * 2f);

        // Coroutine to move the cloud
        StartCoroutine(MoveCloud(spawnedCloud, endPosition));
    }

    private IEnumerator MoveCloud(GameObject spawnedCloud, Vector3 endPosition)
    {
        // Calculate the distance between the cloud and the end position
        var distance = Vector3.Distance(spawnedCloud.transform.position, endPosition);
        // Calculate the time it will take to reach the end position
        var time = distance / cloudSpeed;
        // Calculate the speed of the cloud
        var speed = distance / time;
        
        // While the cloud is not at the end position
        while (spawnedCloud.transform.position != endPosition)
        {
            // Move the cloud towards the end position
            spawnedCloud.transform.position = Vector3.MoveTowards(spawnedCloud.transform.position, endPosition, speed * Time.deltaTime);
            yield return null;
        }
        
        // Destroy the cloud
        Destroy(spawnedCloud);
        _spawnedCloud = false;
    }
}
