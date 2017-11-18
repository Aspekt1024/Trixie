using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem {

    public List<GameObject> PooledObjects;
    public GameObject ObjectPrefab;
    public int PoolSize;
    public bool PoolCanGrow;

    private Transform poolItemParent;

    public void Initialise(Transform parentPool)
    {
        poolItemParent = new GameObject(ObjectPrefab.name + " Pool").transform;
        poolItemParent.SetParent(parentPool);

        PooledObjects = new List<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            AddNewObjectToList();
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            if (!PooledObjects[i].activeInHierarchy)
            {
                return PooledObjects[i];
            }
        }

        if (PoolCanGrow)
        {
            GameObject obj = AddNewObjectToList();
            return obj;
        }

        return null;
    }

    private GameObject AddNewObjectToList()
    {
        GameObject obj = Object.Instantiate(ObjectPrefab, poolItemParent);
        obj.SetActive(false);
        PooledObjects.Add(obj);
        SetPoolSpecificAttributes(obj);
        return obj;
    }

    protected virtual void SetPoolSpecificAttributes(GameObject obj) { }
}
