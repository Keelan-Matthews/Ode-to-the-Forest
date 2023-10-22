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

    public GameObject GetPooledObject(bool player = false)
    {
        foreach (var t in _pooledObjects)
        {
            if (t == null) continue;
            if (!t.activeInHierarchy)
            {
                if (player)
                {
                    if (t.GetComponent<BulletController>().isEnemyBullet) continue;
                }
                return t;
            }
        }

        if (!willGrow) return null;

        // Cannot find an object in list but list can grow, so make a new one
        var obj = (GameObject)Instantiate(pooledObject);
        _pooledObjects.Add(obj);
        return obj;
    }
}
