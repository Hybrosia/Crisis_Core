using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class ObjectPoolController
{
    private static Dictionary<GameObject, List<GameObject>> inactiveObjects = new Dictionary<GameObject, List<GameObject>>();
    private static Dictionary<GameObject, List<GameObject>> activeObjects = new Dictionary<GameObject, List<GameObject>>();

    //Activates an inactive object from the object pool or instantiates a new one.
    public static GameObject SpawnFromPrefab(GameObject prefab)
    {
        if (!inactiveObjects.ContainsKey(prefab)) inactiveObjects.Add(prefab, new List<GameObject>());
        if (!activeObjects.ContainsKey(prefab)) activeObjects.Add(prefab, new List<GameObject>());

        while (inactiveObjects[prefab].Count > 0 && !inactiveObjects[prefab][0])
        {
            inactiveObjects[prefab].RemoveAt(0);
        }
        
        if (inactiveObjects[prefab].Count > 0)
        {
            var objectToActivate = inactiveObjects[prefab][0];
            activeObjects[prefab].Add(objectToActivate);
            inactiveObjects[prefab].RemoveAt(0);

            objectToActivate.SetActive(true);
            return objectToActivate;
        }
        else
        {
            var newObject = Object.Instantiate(prefab);
            activeObjects[prefab].Add(newObject);
            return newObject;
        }
    }

    //Adds the instance back to the pool of inactive objects. If the instance for whatever reason is not registered as active, falls back to destroying it.
    public static void DeactivateInstance(GameObject instance)
    {
        GameObject key = null;
        foreach (var (prefab, objects) in activeObjects)
        {
            if (!objects.Contains(instance)) continue;
            
            key = prefab;
            break;
        }

        if (key)
        {
            instance.SetActive(false);
            activeObjects[key].Remove(instance);
            inactiveObjects[key].Add(instance);
            //TODO: Make child of a transform in DontDestroyOnLoad
        }
        else Object.Destroy(instance);
    }

    //Returns a list of all active objects that have been spawned from the given prefab.
    public static List<GameObject> GetActiveObjects(GameObject prefab)
    {
        activeObjects.TryGetValue(prefab, out var objects);
        return objects;
    }
}
