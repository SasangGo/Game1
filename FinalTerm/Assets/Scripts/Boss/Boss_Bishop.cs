﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bishop : ABoss
{
    [SerializeField] ParticleSystem teleportEffect;
    [SerializeField] GameObject[] magicBalls;
    [SerializeField] MagicCloud magicCloud;

    private const float TELEPORT_SPEED = 0.2f;
    private const int MAX_BALL_NUM = 6;
    private const float RADIUS = 10F;
    private const int OFFSET_OBJECTPOOL = 5;
    private const int REPEAT = 2;
    protected override void OnEnable()
    {
        base.OnEnable();
        InvokeRepeating("UpdateTarget", 0.1f, 0.25f);
        Invoke("OnAction", actionDelay);
        speed = 50;
        health = 4;
        cntHealth = 2;
    }
    protected override void Update()
    {
        base.Update();

    }
    protected override void OnAction()
    {
        base.OnAction();
        action = Random.Range(0, 3);
        switch (action)
        {
            case 0:
                Trace();
                break;
            case 1:
                Teleport();
                break;
            case 2:
                ShootMagicBall();
                break;
        }
    }
    protected override void Trace()
    {
        base.Trace();
        int go = 1;
        Vector3Int movePos = GetMovePosition(go, go);
        while (CheckCanMove(movePos))
        {
            moveList.Add(movePos);
            go++;
            movePos = GetMovePosition(go, go);
        }
        float distance = float.MaxValue;
        for (int i = 0; i < moveList.Count; i++)
        {
            float temp = Vector3Int.Distance(ConvertCellPos(target.position), moveList[i]);
            if (temp < distance)
            {
                distance = temp;
                movePos = moveList[i];
            }
        }
        StartCoroutine(MovingAction(movePos));
        moveList.Clear();
    }
    private void Teleport()
    {
        Vector3Int point = ConvertCellPos(target.position);
        if (!CheckCanMove(point) && target == null) return;

        StartCoroutine(TeleportAction(point));
    }
    private IEnumerator TeleportAction(Vector3Int point)
    {
        bossState = BossState.attack;
        MeshCollider colli = mesh.GetComponent<MeshCollider>();

        anim.SetBool("Teleport", true);
        yield return new WaitForSeconds(TELEPORT_SPEED);
        colli.enabled = true;
        mesh.enabled = true;
        anim.SetBool("Teleport", false);
        transform.position = point + Vector3.up * 5;
        teleportEffect.Play();

        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
    private void ShootMagicBall()
    {
        if (target == null) return;
        StartCoroutine(MagicBallAction());
    }
    private IEnumerator MagicBallAction()
    {
        bossState = BossState.attack;

        // magic Ball 개수를 세기 위해 list에 저장
        List<AObstacle> balls = new List<AObstacle>(); 
        int count = REPEAT;

        while (count-- > 0)
        {
            balls.Clear(); // 리스트 초기화
            int offset = 0;
            // MAX_BALL_NUM 개수만큼의 매직 볼 랜덤 소환
            while (balls.Count < MAX_BALL_NUM)
            {
                // 반구에 균등하게 배분해서 생성
                float angle = offset * Mathf.PI / (MAX_BALL_NUM - 1);
                int rand = Random.Range(0, magicBalls.Length) + OFFSET_OBJECTPOOL;
                AObstacle ball = ObjectPool.Instance.GetObject(rand).GetComponent<AObstacle>();
                ball.enabled = false;
                // 반 구 모양으로 소
                ball.transform.position = transform.position + (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)) * RADIUS;
                offset++;
                balls.Add(ball);
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(1f);
            // 각각의 매직볼 액션 활성
            foreach (AObstacle ball in balls)
            {

                ball.enabled = true;
                ball.transform.LookAt(target);
            }
            yield return new WaitForSeconds(5f);
        }
        yield return new WaitForSeconds(7f);
        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
    protected override void Enraged()
    {
        base.Enraged();
        magicCloud.gameObject.SetActive(true);
        magicCloud.transform.SetParent(null);
        anim.SetTrigger("Rage");
        StartCoroutine(ChangeState());
    }
    private IEnumerator ChangeState()
    {
        gameObject.layer = LayerMask.NameToLayer("Invincibility");
        yield return new WaitUntil(
            () => anim.GetCurrentAnimatorStateInfo(0).IsName("Bishop_Rage")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
        Debug.Log("패턴 종료 ");
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
    protected override void Die()
    {
        base.Die();
        magicCloud.RemoveMagicCloud();
    }
}
