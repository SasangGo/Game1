using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{

    private Dictionary<int, Queue<GameObject>> poolingObjects = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] GameObject[] poolPrefabs;

    private void Awake()
    {
        Initialize(200);
    }
    private void Initialize(int count)
    {
        for(int i = 0; i < poolPrefabs.Length; i++)
        {
            for (int j = 0; j < count; j++)
            {
                if (!poolingObjects.ContainsKey(i))
                {
                    Queue<GameObject> newQueue = new Queue<GameObject>();
                    poolingObjects.Add(i, newQueue);
                }
                CreateNewObject(i);
            }
        }
    }
    private GameObject CreateNewObject(int index)
    {
        var obj = Instantiate(poolPrefabs[index]);
        poolingObjects[index].Enqueue(obj);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        return obj;
    }
    public GameObject GetObject(int index)
    {
        GameObject obj = null;

        if (Instance.poolingObjects.ContainsKey(index))
        {
            if (Instance.poolingObjects[index].Count > 0)
            {
                obj = Instance.poolingObjects[index].Dequeue();
            }
            else
            {
                obj = CreateNewObject(index);
            }

        obj.SetActive(true);
        obj.transform.SetParent(null);
        }
        return obj;
    }
    public void ReturnObject(GameObject obj, int index)
    {
        obj.transform.SetParent(Instance.transform);
        obj.SetActive(false);
        Instance.poolingObjects[index].Enqueue(obj);
    }
}
