﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ABoss : MonoBehaviour
{
    [SerializeField] Material originColor;
    [SerializeField] Material damagedColor;
    [SerializeField] protected Transform sPos;
    [SerializeField] protected Transform ePos;

    protected int health;
    protected int speed;
    protected int cntHealth;
    protected Transform target;// 타겟
    protected int direction; // 타겟 위치 정보
    protected Rigidbody rigid;
    protected int action;
    protected MeshRenderer mesh;
    protected Animator anim;
    protected float actionDelay;
    protected int[] dx = { 1, 1, -1, -1 };
    protected int[] dz = { -1, 1, 1, -1 };

    // 인식 타겟 방향
    private const int FRONT = -180;
    private const int RIGHT = 90;
    private const int LEFT = -90;
    private const int BACK = 0;
    private const float ERORR_FIX_DELAY = 0.5F;

    //이동 방향 리스트를 담는 배열
    protected List<Vector3Int> moveList = new List<Vector3Int>();

    //보스 상태 : 기본/추적/공격/죽음
    public enum BossState
    {
        idle,trace,attack,dead
    }
    protected BossState bossState = BossState.idle;
    protected virtual void OnEnable()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        direction = 0;
        actionDelay = 3f;
        
    }

    protected virtual void Update()
    {
        if (bossState == BossState.dead) return;
        if (transform.position.y < GameManager.Instance.DEADLINE)
        {
            Die();
        }
        if (target == null) return;
        Rotate(target.position);
    }
    protected virtual void UpdateTarget()
    {
        //타겟 인식 구 레이캐스트
        Collider[] cols = Physics.OverlapSphere(transform.position, 500f, LayerMask.GetMask("Player", "Invincibility"));
        for (int i = 0; i < cols.Length; i++)
        {
            PlayerControl newTarget = cols[i].GetComponent<PlayerControl>();
            if (newTarget != null)
            {
                //타겟을 발견하면 Trace()
                target = newTarget.transform;
                UpdateDirection();
                return;
            }
        }
        Debug.Log("타겟놓침");
        target = null;
    }
    protected virtual void UpdateDirection()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (RIGHT >= angle && angle > BACK)
        {
            direction = 1;
        }
        else if (BACK >= angle && angle > LEFT)
        {
            direction = 2;
        }
        else if (angle <= LEFT && angle > FRONT)
        {
            direction = 3;
        }
        else
        {
            direction = 0;
        }
    }
    protected virtual void Trace(Vector3 pos)
    {
        if (target.gameObject == null) return;
        // 보스를 추적 상태로 바꿈
        anim.enabled = false;
        bossState = BossState.trace;
    }
    protected virtual void Enraged()
    {
        bossState = BossState.attack;
        CancelInvoke("OnAction");// 광폭화중에는 패턴을 멈춤.
    }
    // 보스 움직임 액션
    protected IEnumerator MovingAction(Vector3 movePos)
    {

        // 중력을 꺼둬 lerp에 방해되지 않게 함
        rigid.useGravity = false;
        float cnt = 0;
        while(Vector3.Distance(transform.position,movePos) > 0.5f && cnt < ERORR_FIX_DELAY)
        {
            cnt += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position+ Vector3.up, movePos, cnt);

            yield return null;
        }
        transform.position = movePos;
        rigid.useGravity = true;
        bossState = BossState.idle;
        anim.enabled = true;
        Invoke("OnAction", actionDelay);
    }

    // 이동 방향을 바라보도록 자연스럽게 (부드럽게) 회전
    protected virtual void Rotate(Vector3 location)
    {
        if (target == null || bossState != BossState.trace) return;
        if (location == Vector3.zero) return;
        location = new Vector3(location.x,GameManager.Instance.CELL_OFFSET_Y, location.z);
        Vector3 cntPos = new Vector3(transform.position.x, GameManager.Instance.CELL_OFFSET_Y, transform.position.z);
        Vector3 dir = (location - cntPos).normalized;
        Quaternion newRot = Quaternion.LookRotation(dir);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRot, Time.deltaTime * speed);
    }
    //이동 방향을 곧바로 바라
    protected virtual void Rotate(Vector3 location, bool immediate)
    {
        if (target == null || bossState == BossState.dead) return;
        if (location == Vector3.zero) return;
        location = new Vector3(location.x, GameManager.Instance.CELL_OFFSET_Y, location.z);
        Vector3 cntPos = new Vector3(transform.position.x, GameManager.Instance.CELL_OFFSET_Y, transform.position.z);
        Vector3 dir = (location - cntPos).normalized;
        Quaternion newRot = Quaternion.LookRotation(dir);
        if (immediate) rigid.rotation = Quaternion.Slerp(rigid.rotation, newRot, 1f);
        Debug.Log(rigid.rotation);
    }

    // 현재위치에서 가로 x칸 세로 z칸 만큼 이동
    protected virtual Vector3Int GetMovePosition(int x, int z)
    {
        int offset = GameManager.Instance.CELL_OFFSET_Y;
        Vector3 fixedPos = transform.position;
        int cell = GameManager.Instance.CELL_SIZE;
       fixedPos = new Vector3(fixedPos.x + dx[direction] * x * cell, offset, fixedPos.z + dz[direction] * z * cell);
        return ConvertCellPos(fixedPos);
    }
    protected virtual Vector3Int ConvertCellPos(Vector3 origin)
    {
        int offset = GameManager.Instance.CELL_OFFSET_Y;
        int cell = GameManager.Instance.CELL_SIZE;
        return new Vector3Int(Mathf.RoundToInt(origin.x / cell) * cell, offset, Mathf.RoundToInt(origin.z / cell) * cell);
    }

    protected virtual void OnAction()
    {
        bossState = BossState.attack;
        if (bossState != BossState.idle) return;
    }

    //해당 위치로 이동할 수 있는지 확인
    protected bool CheckCanMove(Vector3 pos)
    {
        float length = (pos - transform.position).magnitude;
        Vector3 frontDir = (pos - transform.position).normalized;

        // 해당 좌표에 땅이 없으면 이동 불가
        if (!Physics.Raycast(pos + Vector3.up, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"))) return false;
        // 해당 좌표로 가는 길목에 장애물이 있을 경우 이동 불가
        if (Physics.Raycast(transform.position, frontDir, length, LayerMask.GetMask("Ground"))) return false;

        return true;
    }
    public virtual void OnDamaged(Vector3 pos)
    {
        cntHealth--;
        if (cntHealth <= 0) Die();
        else if (cntHealth <= 1) Enraged();
        StartCoroutine(DamagedEffect(2f));
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
        gameObject.layer = 12;
        float percent = (health - cntHealth) / (float)health;
        mesh.material.color = Color.Lerp(originColor.color, damagedColor.color, percent);
    }
    protected void Die()
    {
        gameObject.layer = 9;
        bossState = BossState.dead;
        if(anim != null) anim.SetBool("Dead", true);
        CancelInvoke();
        GameManager.Instance.IntervalNextPhase();
        Destroy(gameObject, 2);
    }
}