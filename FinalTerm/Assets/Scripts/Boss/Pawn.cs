using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ABoss
{
    const float ERORR_FIX_DELAY = 0.5f;
    protected override void OnEnable()
    {
        base.OnEnable();
        health = 1;
        cntHealth = health;
        InvokeRepeating("OnAction", 2,2);
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnAction()
    {
        Vector3Int movePos = ConvertCellPos(transform.position + transform.forward * GameManager.Instance.CELL_SIZE);
        StartCoroutine(PawnAction(movePos));
    }
    protected IEnumerator PawnAction(Vector3 movePos)
    {

        // 중력을 꺼둬 Slerp에 방해되지 않게 함
        rigid.useGravity = false;
        float cnt = 0;
        while (Vector3.Distance(transform.position, movePos) > 0.5f && cnt < ERORR_FIX_DELAY)
        {
            cnt += Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, movePos, 0.1f);
            yield return null;
        }
        transform.position = movePos;
        rigid.useGravity = true;
        bossState = BossState.idle;
    }
    protected override void Die()
    {
        Destroy(gameObject);
    }
}
