using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler Instance;

    public List<ObjectPoolItem> ObjectPools;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple ObjectPoolers in this scene. Only one can be present");
            Destroy(gameObject);
            return;
        }

        foreach (ObjectPoolItem pool in ObjectPools)
        {
            pool.Initialise(transform);
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        foreach (ObjectPoolItem pool in ObjectPools)
        {
            if (pool.ObjectPrefab.tag == tag)
            {
                return pool.GetPooledObject();
            }
        }
        return null;
    }
}
