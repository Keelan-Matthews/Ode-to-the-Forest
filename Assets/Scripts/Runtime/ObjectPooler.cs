using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public GameObject pooledObject;
    public int pooledAmount;
    public bool willGrow;
    
    private List<GameObject> _pooledObjects;
    
    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        _pooledObjects = new List<GameObject>();
        for (var i = 0; i < pooledAmount; i++)
        {
            var obj = (GameObject) Instantiate(pooledObject);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (var i = 0; i < _pooledObjects.Count; i++)
        {
            if (_pooledObjects[i] == null) continue; //FIX THIS LATER!!!!!!!!!!!!!!!!!!!!!!!!!
            if (!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }

        if (!willGrow) return null;

        // Cannot find an object in list but list can grow, so make a new one
        var obj = (GameObject)Instantiate(pooledObject);
        _pooledObjects.Add(obj);
        return obj;
    }
}
