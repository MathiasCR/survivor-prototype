using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    public List<GameObject> pooledObjects;

    private int _lastIndexActivated;

    public GameObjectPool(GameObject poolPrefab, int poolAmount)
    {
        pooledObjects = new List<GameObject>();
        GameObject go;
        for (int i = 0; i < poolAmount; i++)
        {
            go = Object.Instantiate(poolPrefab);
            go.SetActive(false);
            pooledObjects.Add(go);
        }
    }

    public void OnDestroy()
    {
        pooledObjects.Clear();
    }

    public GameObject GetGameObject(Vector3 position, Quaternion rotation)
    {
        GameObject go = pooledObjects.Find((go) => !go.activeInHierarchy);

        go.transform.position = position;
        go.transform.rotation = rotation;

        return go;
    }

    /// <summary>
    /// Return an object to the pool (reset objects before returning).
    /// </summary>
    public void ReturnGameObject(GameObject gameObject)
    {
        pooledObjects.Find((go) => go.Equals(gameObject)).SetActive(false);
    }
}
