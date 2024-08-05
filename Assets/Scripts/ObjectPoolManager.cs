using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ObjectPoolManager : MonoBehaviour
{

    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    private GameObject ObjectPoolEmptyHolder;

    private static GameObject reifleBulletEmpty;
    private static GameObject pistolBulletEmpty;
    private static GameObject zombiesEmpty;

    public enum PoolType
    {
        ReifleBullet,
        PistolBullet,
        None,
        Zombie
    }

    public static PoolType poolType;

    private void Awake()
    {
        SetUpEmpties();
    }

    private void SetUpEmpties()
    {
        ObjectPoolEmptyHolder = new GameObject("Pooled Object");
        reifleBulletEmpty = new GameObject("Reifle Magazine");
        pistolBulletEmpty = new GameObject("Pistol Magazine");
        zombiesEmpty = new GameObject("Zombies");

        reifleBulletEmpty.transform.SetParent(ObjectPoolEmptyHolder.transform);
        pistolBulletEmpty.transform.SetParent(ObjectPoolEmptyHolder.transform);
        zombiesEmpty.transform.SetParent(ObjectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        
        PooledObjectInfo pool = ObjectPools.Find(p => p.lookUpString == objectToSpawn.name);

        //in case we didn't found the object, let's create it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookUpString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        //checks if ther are any inactive objects in the pool

        //GameObject spawnableObject = pool.inactiveGameObjects.FirstOrDefault();

        GameObject spawnableObject = null;
        foreach (GameObject gameObject in pool.inactiveGameObjects) {
            if (gameObject != null)
            {
                spawnableObject = gameObject;
            }
        }

        if (spawnableObject == null)
        {
            GameObject parentObject = SetParentObject(poolType);
            spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            if (parentObject != null)
            {
                spawnableObject.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            spawnableObject.transform.position = spawnPosition;
            spawnableObject.transform.rotation = spawnRotation;
            pool.inactiveGameObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }

        return spawnableObject; 
    }

    public static void returnObjectToPool(GameObject gameObject)
    {
        string objectName = gameObject.name.Substring(0, gameObject.name.Length-7); //removes the "(clone)"
        PooledObjectInfo pool = ObjectPools.Find(p => p.lookUpString == objectName);

        if (pool == null)
        {
            Debug.Log("Trying to releae an object that is not pooled: " + objectName);
        }
        else
        {
            gameObject.SetActive(false);
            pool.inactiveGameObjects.Add(gameObject);
        }

    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.ReifleBullet:
                return reifleBulletEmpty;
            case PoolType.PistolBullet:
                return pistolBulletEmpty;
            case PoolType.Zombie:
                return zombiesEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }

}

public class PooledObjectInfo
{
    public string lookUpString;
    public List<GameObject> inactiveGameObjects = new List<GameObject> ();
}
