using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bishop : ABoss
{
    [SerializeField] ParticleSystem teleportEffect;
    [SerializeField] GameObject[] MagicBalls;

    private const float TELEPORT_SPEED = 0.2f;
    private const int MAX_BALL_NUM = 6;
    private const float RADIUS = 10F;
    private const int OFFSET_OBJECTPOOL = 5;
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
        action = 2;
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
        anim.enabled = false;
        StartCoroutine(MagicBallAction());
    }
    private IEnumerator MagicBallAction()
    {
        List<AObstacle> balls = new List<AObstacle>();
        int offset = 0;
        // MAX_BALL_NUM 개수만큼의 매직 볼 랜덤 소환
        while (balls.Count < MAX_BALL_NUM)
        {
            float angle = offset * Mathf.PI / (MAX_BALL_NUM - 1);
            int rand = Random.Range(0, MagicBalls.Length) + OFFSET_OBJECTPOOL;
            AObstacle ball = ObjectPool.Instance.GetObject(rand).GetComponent<AObstacle>();
            ball.enabled = false;
            // 반 구 모양으로 소
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
    }
}
