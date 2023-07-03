using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssencePooler : MonoBehaviour
{
    public static EssencePooler Instance;
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
        foreach (var t in _pooledObjects)
        {
            if (t == null) continue; //FIX THIS LATER!!!!!!!!!!!!!!!!!!!!!!!!!
            if (t.activeInHierarchy) continue;
            // Make sure the sprite renderer is enabled
            t.GetComponent<SpriteRenderer>().enabled = true;
            return t;
        }

        if (!willGrow) return null;

        // Cannot find an object in list but list can grow, so make a new one
        var obj = (GameObject)Instantiate(pooledObject);
        _pooledObjects.Add(obj);
        return obj;
    }
}
