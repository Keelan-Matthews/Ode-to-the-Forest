using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    [SerializeField] private AIBehaviour aiBehaviour;
    private float _spawnTime = 1f;
    private bool _spawned;
    
    private void Start()
    {
        // Wait for the spawn time
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(_spawnTime);
        _spawned = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (aiBehaviour == null || !_spawned) return;
        aiBehaviour.Think(this);
    }
    
    public bool IsSpawned()
    {
        return _spawned;
    }
}
