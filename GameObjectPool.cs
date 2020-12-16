//GameObject pooling script
//Author: Surya Narendran (https://github.com/SuryaNarendran)
//Date: 16 Dec 2020



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //<summary> Gets a GameObject from the sleeping pool and activates it </summary>
    public GameObject GetObject()
    {
        if (_sleepingObjects.Count < 1)
            InstantiateObjects(1); //makes sure the pool expands to accomodate the GetObject request if necessary

        GameObject awakened = _sleepingObjects.Dequeue();
        awakened.SetActive(true);
        _activeObjects.Add(awakened);
        return awakened;
    }

    //<summary>Call this to return a member of the active pool to the sleeping pool. 
    //The caller is responsible for first resetting the state of the returned GameObject to default if neccesary</summary>
    public void ReturnObject(GameObject returned)
    {
        if (_activeObjects.Contains(returned))
        {
            _activeObjects.Remove(returned);
            returned.SetActive(false);
            _sleepingObjects.Enqueue(returned);
        }

        else Debug.LogError("the returned game object is not a member of the pool!");
    }

    //<summary>Creates prefab instances and adds them to the sleeping pool</summary>
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

    //<summary> Sets all members to inactive and returns them to the sleeping pool<summary>
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
