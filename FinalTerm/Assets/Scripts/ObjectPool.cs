using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀링 관리하는 스크립트 < 싱글톤>
public class ObjectPool : Singleton<ObjectPool>
{
    // 매핑을 위해 Dictionary를 사용(숫자 키와 큐 값을 사용)
    private Dictionary<int, Queue<GameObject>> poolingObjects = new Dictionary<int, Queue<GameObject>>();

    // 생성할 게임 오브젝트 프리팹을 유니티 프로젝트 상에서 넣으면 됨
    public List<GameObject> poolList; 

    private void Awake()
    {
        Initialize(100);
    }
    // 초기화(개수)
    private void Initialize(int count)
    {
        for(int i = 0; i < poolList.Count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                // 풀링 Dictionary에 해당 오브젝트 큐 키가 없으면
                if (!poolingObjects.ContainsKey(i))
                {
                    // 해당 큐를 만들고 Dictinary에 추가
                    Queue<GameObject> newQueue = new Queue<GameObject>();
                    poolingObjects.Add(i, newQueue);
                }
                CreateNewObject(i);
            }
        }
    }
    // 오브젝트 생성(프리팹 번호)
    private GameObject CreateNewObject(int index)
    {
        var obj = Instantiate(poolList[index]);
        poolingObjects[index].Enqueue(obj); // 해당 키의 큐에 추가
        obj.SetActive(false); // 당장 사용 안하므로 비활성화
        obj.transform.SetParent(transform); // 해당 스크립트의 자식으로 지정
        return obj; // GameObject 형으로 반환
    }
    // 오브젝트 불러오기(프리팹 번호)
    public GameObject GetObject(int index)
    {
        GameObject obj = null;

        // 해당 키가 있으면
        if (Instance.poolingObjects.ContainsKey(index))
        {
            if (Instance.poolingObjects[index].Count > 0)
            {
                // 큐에 있는 경우에만 큐에서 빼서 가져옴
                obj = Instance.poolingObjects[index].Dequeue();
            }
            else
            {
                // 없으면 오브젝트 생성
                obj = CreateNewObject(index);
            }

        obj.SetActive(true); // 오브젝트 활성화
        obj.transform.SetParent(null); // 자식화 해제
        }
        return obj;
    }
    // 오브젝트 반환(되돌리기) (오브젝트, 번호)
    public void ReturnObject(GameObject obj, int index)
    {
        obj.transform.SetParent(Instance.transform); // 해당 인스턴스의 자식으로 다시 지정
        obj.SetActive(false); // 비활성화
        Instance.poolingObjects[index].Enqueue(obj); // 큐에 다시 추가
    }
}
