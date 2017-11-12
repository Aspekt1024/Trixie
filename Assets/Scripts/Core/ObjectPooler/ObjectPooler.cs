using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler Instance;

    public List<ObjectPoolItem> ObjectPools;
    public List<ProjectilePoolItem> ProjectilePools;

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
        foreach (ProjectilePoolItem pool in ProjectilePools)
        {
            pool.Initialise(transform);
        }
    }

    public GameObject GetPooledProjectile(Projectile.ProjectileType type)
    {
        foreach (ProjectilePoolItem pool in ProjectilePools)
        {
            if (pool.Type == type)
            {
                return pool.GetPooledObject();
            }
        }
        return null;
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
