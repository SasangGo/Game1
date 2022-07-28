using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>: MonoBehaviour where T: MonoBehaviour
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            instance = FindObjectOfType(typeof(T)) as T;
            if(instance == null)
            {
                instance = new GameObject(typeof(T).ToString(), typeof(T)).AddComponent<T>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        var objs = FindObjectsOfType<T>();
        if(objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }
}
