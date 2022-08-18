﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ABoss : MonoBehaviour
{
    [SerializeField] Material originColor;
    [SerializeField] Material damagedColor;
    protected int health;
    protected int speed;
    protected int cntHealth;
    protected Transform target;// 타겟
    protected Rigidbody rigid;
    protected int action;
    protected MeshRenderer mesh;
    protected int[] dirX = { 1, 1, -1, -1 };
    protected int[] dirZ = { -1, 1, 1, -1 };

    // 인식 타겟 방향
    private const int FRONT = -180;
    private const int RIGHT = 90;
    private const int LEFT = -90;
    private const int BACK = 0;
    private const int OFFSETY = -5;
    private const float ERORR_FIX_DELAY = 0.5F;

    private Animator anim;
    //방향 도움 배열 변수

    //이동 방향 리스트를 담는 배열
    protected List<Vector3Int> moveList = new List<Vector3Int>();

    //보스 상태 : 기본/추적/공격/죽음
    public enum BossState
    {
        idle,trace,attack,dead
    }
    protected BossState bossState = BossState.idle;
    protected virtual void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        //3초마다 타겟 쫓아감
        InvokeRepeating("UpdateTarget", 0.5f, 1);
        InvokeRepeating("OnAction", 3, 3);
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (target == null) return;
        Rotate(target.position);
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
        anim.SetBool("Trace", true);
    }
    // 보스 움직임 액션
    protected IEnumerator MovingAction(Vector3 movePos)
    {

        // 중력을 꺼둬 Slerp에 방해되지 않게 함
        rigid.useGravity = false;
        float cnt = 0;
        while(Vector3.Distance(transform.position,movePos) > 0.5f && cnt < ERORR_FIX_DELAY)
        {
            cnt += Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, movePos, 0.1f);
            yield return null;
        }
        transform.position = movePos;
        rigid.useGravity = true;
        bossState = BossState.idle;
        anim.SetBool("Trace", false);
        Invoke("UpdateTarget", 3);
    }

    // 이동 방향을 바라보도록 회전
    protected virtual void Rotate(Vector3 location)
    {
        if (target == null || bossState != BossState.trace) return;
        if (location == Vector3.zero) return;
        location = new Vector3(location.x, 0, location.z);
        Quaternion dir = Quaternion.LookRotation(location).normalized;
        Vector3 v = dir.eulerAngles;
        dir = Quaternion.Euler(new Vector3(v.x, v.y, v.z));
        rigid.rotation = Quaternion.Lerp(rigid.rotation, dir, 1f) ;
        Debug.Log("회전");

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

    protected virtual void OnAction()
    { }

    //이동할 위치에 땅이 있는지 체크
    protected bool CheckIsGround(Vector3 pos)
    {
        if (Physics.Raycast(pos+Vector3.up,Vector3.down,Mathf.Infinity,LayerMask.GetMask("Ground"))) return true;
        else return false;
    }
    protected virtual void OnDamaged(Vector3 pos)
    {
        cntHealth--;
        if (cntHealth <= 0) Die();
        else
        {
            StartCoroutine(DamagedEffect(2f));
        }
    }
    protected virtual IEnumerator DamagedEffect(float time)
    {
        gameObject.layer = 9;
        while (time-- > 0)
        {
            mesh.material.color = damagedColor.color;
            yield return new WaitForSeconds(0.5f);
            mesh.material.color = originColor.color;
            yield return new WaitForSeconds(0.5f);
        }
        gameObject.layer = 0;
        float percent = (health - cntHealth) / (float)health;
        mesh.material.color = Color.Lerp(originColor.color, damagedColor.color, percent);
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        PlayerControl player = collision.collider.GetComponent<PlayerControl>();
        if (player == null) return;
        if(collision.GetContact(0).normal.y < -0.5f) 
        {
            OnDamaged(collision.GetContact(0).point);
        }
    }
    protected virtual void Die()
    {
        gameObject.layer = 9;
        bossState = BossState.dead;
        anim.SetBool("Dead", true);
        CancelInvoke();
        Destroy(gameObject, 2f);
    }
}
