using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour을 상속받은 스크립트만 사용 가능
// 사용법 : 클래스 이름 뒤에 : Singleton<해당 클래스 이름> 하면 끝
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
                // 해당 인스턴스가 씬에 없으면 새로 추가
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
            // 이미 있다면 초기화 과정에서 파괴
            Destroy(gameObject);
            return;
        }
    }
}
