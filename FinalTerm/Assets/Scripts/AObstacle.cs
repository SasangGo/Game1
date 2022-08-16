using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 구조적으로 사용하기 편하도록 abstract class로 선언
// 모든 장애물들은 AObstacle을 상속받아 사용하면 됨
// 충돌시에 관한 작용이 쓰여있음
public abstract class AObstacle : MonoBehaviour
{
    public RaycastHit[] hits; // 셀 감지
    public int Index { get; protected set; }
    protected virtual void OnEnable()
    {
        List<GameObject> poolList = ObjectPool.Instance.poolList;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (gameObject.name.Contains(poolList[i].name))
            {
                Index = i;
                return;
            }
        }
    }
    // Ontrigger 시작할 때
    protected virtual void OnTriggerEnter(Collider other)
    {
        //위와 같음
        PlayerControl player = other.GetComponent<PlayerControl>();
        if(player != null && player.gameObject.layer == 10)
        {
            if (gameObject.tag == "ExpBall")
            {
                player.exp += gameObject.GetComponent<ExpBall>().exp;
                ObjectPool.Instance.ReturnObject(gameObject, 3);
            }
        }
    }
    // 장애물 오브젝트 풀링(반환) (반환시간, 인덱스 번호)
    public IEnumerator ReturnObstacle(float time, int index)
    {
        float cnt = 0;
        while (cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        // 물리를 가지고 있는 장애물일 경우 속도를 초기화
        Rigidbody rigid = GetComponent<Rigidbody>();
        if(rigid != null) rigid.velocity = Vector3.zero;

        // 반환할 때 붉은색으로 만들었던 셀을 원래 색으로 되돌림
        if(hits != null && hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.ChangeColor(cell.originColor);
            }
        }
        ObjectPool.Instance.ReturnObject(gameObject, index);

    }
}
