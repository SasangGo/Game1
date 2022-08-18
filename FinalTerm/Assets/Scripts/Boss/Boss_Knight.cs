using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Knight : ABoss
{

    // 기본적인 정보
    protected override void Start()
    {
        base.Start();
        speed = 5;
        health = 4;
    }
    protected override void Trace(Vector3 pos)
    {
        base.Trace(pos);
        // 체스에서 말의 움직임 구현
        moveList.Add(GetMovePosition(2, 1));
        moveList.Add(GetMovePosition(1, 2));

        // 두 방향 중 랜덤 방향으로 움직임
        int idx = Random.Range(0, moveList.Count);
        Vector3 move = moveList[idx];
        Rotate();
        Debug.Log(move);
        StartCoroutine(MovingAction(move));

        //새로운 리스트를 받기위해 리스트 초기화
        moveList.Clear();
    }
}
