using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bishop : ABoss
{
    private const float TELEPORT_SPEED = 0.5f;
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
        action = 1;
        switch (action)
        {
            case 0:
                Trace();
                break;
            case 1:
                Teleport();
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
    private void Teleport()
    {
        Vector3Int point = ConvertCellPos(target.position);
        if (!CheckCanMove(point) && target != null) return;

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
 
        bossState = BossState.idle;
        Invoke("OnAction", actionDelay);
    }
}
