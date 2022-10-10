using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bishop : ABoss
{
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
        action = 0;
        switch (action)
        {
            case 0:
                if(target != null) Trace(target.position);
                break;
        }
    }
    protected override void Trace(Vector3 pos)
    {
        base.Trace(pos);
        int go = 1;
        Vector3Int movePos = GetMovePosition(go, go);
        while (CheckCanMove(movePos))
        {
            moveList.Add(movePos);
            go++;
            movePos = GetMovePosition(go, go);
        }
        float distance = float.MaxValue;
        for(int i = 0; i < moveList.Count; i++)
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
}
