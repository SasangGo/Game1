using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ABoss : MonoBehaviour
{
    protected int health;
    protected int speed;
    protected Transform target;// 타겟
    protected Rigidbody rigid;

    // 인식 타겟 방향
    private const int FRONT = -180;
    private const int RIGHT = 90;
    private const int LEFT = -90;
    private const int BACK = 0;
    private const int OFFSETY = -5;

    //방향 도움 배열 변수
    protected int[] dirX = { 1, 1, -1, -1 };
    protected int[] dirZ = { -1, 1, 1, -1 };

    //이동 방향 리스트를 담는 배열
    protected List<Vector3Int> moveList = new List<Vector3Int>();

    //보스 상태 : 기본/추적/공격/죽음
    public enum BossState
    {
        idle,trace,attack,dead
    }
    BossState bossState = BossState.idle;
    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();

        //3초마다 타겟 쫓아감
        Invoke("UpdateTarget", 3);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (target == null) return;
    }
    protected virtual void UpdateTarget()
    {
        //타겟 인식 구 레이캐스트
        Collider[] cols = Physics.OverlapSphere(transform.position, 50f, LayerMask.GetMask("Player"));
        for (int i = 0; i < cols.Length; i++)
        {
            PlayerControl newTarget = cols[i].GetComponent<PlayerControl>();
            if (newTarget != null)
            {
                //타겟을 발견하면 Trace()
                target = newTarget.transform;
                Trace(target.position);
                return;
            }
        }
        Debug.Log("타겟놓침");
        target = null;
    }
    protected virtual void Trace(Vector3 pos)
    {
        // 보스를 추적 상태로 바꿈
        bossState = BossState.trace;
    }
    // 보스 움직임 액션
    protected IEnumerator MovingAction(Vector3 movePos)
    {

        // 중력을 꺼둬 Slerp에 방해되지 않게 함
        rigid.useGravity = false;
        while(Vector3.Distance(transform.position,movePos) > 0.5f)
        {
            transform.position = Vector3.Slerp(transform.position, movePos, 0.1f);
            yield return null;
            Debug.Log("실행중");
        }
        transform.position = movePos;
        rigid.useGravity = true;
        Debug.Log("이동종료");
        Invoke("UpdateTarget", 3);
    }

    // 이동 방향을 바라보도록 회전
    protected virtual void Rotate()
    {
        if (target == null) return;
        Vector3 pos = new Vector3(target.position.x, 0, target.position.z);
        Quaternion newRot = Quaternion.LookRotation(pos.normalized);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRot, Time.deltaTime * speed);
    }

    // 현재위치에서 가로 x칸 세로 z칸 만큼 이동
    protected virtual Vector3Int GetMovePosition(int x, int z)
    {
        int idx = 0;
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg;

        if (RIGHT >= angle && angle > BACK)
        {
            idx = 1;
            Debug.Log("back_Right");
        }
        else if (BACK >= angle && angle > LEFT)
        {
            idx = 2;
            Debug.Log("Back_left");
        }
        else if (angle <= LEFT && angle > FRONT)
        {
            idx = 3;
            Debug.Log("Front_Left");
        }
        else
        {
            idx = 0;
            Debug.Log("front_Right");
        }
        Vector3 fixedPos = transform.position;
        fixedPos = new Vector3(fixedPos.x + x * dirX[idx] * 5, OFFSETY, fixedPos.z + z*dirZ[idx] * 5);
        return new Vector3Int(Mathf.RoundToInt(fixedPos.x/5)*5, OFFSETY, Mathf.RoundToInt(fixedPos.z/5)*5);
    }

    //이동할 위치에 땅이 있는지 체크
    protected bool CheckIsGround(Vector3 pos)
    {
        Collider[] hit = Physics.OverlapBox(pos, new Vector3(1, 1, 1), Quaternion.identity, LayerMask.GetMask("Ground"));
        Debug.Log(hit.Length);
        if (hit.Length > 0) return true;
        else return false;
    }
}
