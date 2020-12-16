//GameObject pooling script
//Author: Surya Narendran (https://github.com/SuryaNarendran)
//Date: 16 Dec 2020



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A pooling system for GameObjects to facilitate reuse
/// </summary>
public class GameObjectPool
{
    public GameObject Prefab { get; set; }
    public Transform ObjectParent { get; set; }

    private HashSet<GameObject> _activeObjects = new HashSet<GameObject>();
    private Queue<GameObject> _sleepingObjects = new Queue<GameObject>();

    public IReadOnlyCollection<GameObject> ActiveObjects { get { return _activeObjects; } }

    
    public GameObjectPool(GameObject prefab, int count, Transform parent=null)
    {
        Prefab = prefab;
        ObjectParent = parent;
        InstantiateObjects(count);
    }

    ///<summary>Gets an activated GameObject from the sleeping pool</summary>
    public GameObject GetObject()
    {
        if (_sleepingObjects.Count < 1)
            InstantiateObjects(1); //makes sure the pool expands to accomodate the GetObject request if necessary

        GameObject awakened = _sleepingObjects.Dequeue();
        awakened.SetActive(true);
        _activeObjects.Add(awakened);
        return awakened;
    }

    ///<summary>Returns a member of the active pool to the sleeping pool</summary>
    public void ReturnObject(GameObject returned)
    {
        //the caller is responsible for resetting the object to its default state, if necessary
        if (_activeObjects.Contains(returned))
        {
            _activeObjects.Remove(returned);
            returned.SetActive(false);
            _sleepingObjects.Enqueue(returned);
        }

        else Debug.LogError("the returned game object is not a member of the pool!");
    }

    ///<summary>Creates prefab instances and adds them to the sleeping pool</summary>
    public void InstantiateObjects(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newGameObject = MonoBehaviour.Instantiate(Prefab);
            newGameObject.transform.SetParent(ObjectParent);
            newGameObject.SetActive(false);
            _sleepingObjects.Enqueue(newGameObject);
        }
    }

    ///<summary> Sets all members to inactive and returns them to the sleeping pool<summary>
    public void ReturnAll()
    {
        foreach(GameObject active in _activeObjects)
        {
            active.SetActive(false);
            _sleepingObjects.Enqueue(active);
        }
        _activeObjects.Clear();
    }

    public bool IsActiveMember(GameObject member)
    {
        return _activeObjects.Contains(member);
    }
}
