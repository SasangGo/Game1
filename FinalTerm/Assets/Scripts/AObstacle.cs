using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 구조적으로 사용하기 편하도록 abstract class로 선언
// 모든 장애물들은 AObstacle을 상속받아 사용하면 됨
// 충돌시에 관한 작용이 쓰여있음
public abstract class AObstacle : MonoBehaviour
{
    // 물리충돌 시작할 때(Ontrigger 체크 X)
    protected virtual void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌하면 플레이어의 DieOperate 코루틴을 시작시킴
        PlayerControl player = collision.collider.GetComponent<PlayerControl>();
        if(player != null)
        {
            //충돌 지점을 체크하여 OnDamaged 함수에 넘김
            Vector3 colliPos = collision.contacts[0].point;
            Debug.Log(colliPos);
            player.OnDamaged(colliPos);
        }
    }
    // Ontrigger 시작할 때
    protected virtual void OnTriggerEnter(Collider other)
    {
        //위와 같음
        PlayerControl player = other.GetComponent<PlayerControl>();
        if(player != null && player.gameObject.layer == 0)
        {
            Vector3 colliPos = other.bounds.center;
            player.OnDamaged(colliPos);
        }
    }
    // 장애물 오브젝트 풀링(반환) (반환시간, 인덱스 번호)
    protected IEnumerator ReturnObstacle(float time, int index)
    {
        float cnt = 0;
        while (cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        ObjectPool.Instance.ReturnObject(gameObject, index);
    }
}
