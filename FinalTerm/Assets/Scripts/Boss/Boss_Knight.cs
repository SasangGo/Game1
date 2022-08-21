using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Knight : ABoss
{
    [SerializeField] ParticleSystem rushEffect;
    // 기본적인 정보
    protected override void Start()
    {
        base.Start();
        speed = 50;
        health = 4;
        cntHealth = 2;
    }
    protected override void Update()
    {
        if (target == null) return;
        Rotate(target.position);

        if (bossState != BossState.idle) return;
        Vector3Int targetPos = ConvertCellPos(target.position);
        if (ConvertCellPos(transform.position).x == targetPos.x
            || ConvertCellPos(transform.position).z == targetPos.z)
        {
            Vector3 dir = (targetPos - ConvertCellPos(transform.position));
            StartCoroutine(Rush((dir.normalized)));
        }
    }
    protected override void Trace(Vector3 pos)
    {
        base.Trace(pos);
        // 체스에서 말의 움직임 구현
        moveList.Add(GetMovePosition(2, 1));
        moveList.Add(GetMovePosition(1, 2));

        // 두 방향 중 랜덤 방향으로 움직임
        while(moveList.Count > 0)
        {
            int idx = Random.Range(0, moveList.Count);
            // 배열 중 이동 가능한 장소가 있으면 MovingAction을 실행하여 이동
            if (CheckCanMove(moveList[idx]))
            {
                Debug.Log("움직임");

                StartCoroutine(MovingAction(moveList[idx]));
                break;
            }
            //그 장소가 이동불가능하다면 삭제
            moveList.Remove(moveList[idx]);
        }
        //리스트가 다 삭제될때까지 이동가능한 곳이 없다면 이동안하고 종료
        //새로운 리스트를 받기위해 리스트 초기화
        moveList.Clear();

    }
    protected override void OnAction()
    {
        base.OnAction();
        action = Random.Range(0, 2);
        switch (action)
        {
            case 0:
                {
                    if (target != null) Trace(target.position);
                }
                break;
            case 1:
                break;
        }
        Invoke("OnAction", 3);
    }
    private IEnumerator Rush(Vector3 dir)
    {
        bossState = BossState.attack;
        CancelInvoke();
        rushEffect.gameObject.SetActive(true);
        while (CheckCanMove(ConvertCellPos(transform.position + dir)))
        {
            transform.Translate(dir * Time.deltaTime * speed);
            yield return null;
        }
        transform.position = ConvertCellPos(transform.position);
        bossState = BossState.idle;
        rushEffect.gameObject.SetActive(false);
        Invoke("OnAction", 3);
        InvokeRepeating("UpdateTarget", 0.5f, 1);
    }
}
