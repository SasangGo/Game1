using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBall : AObstacle
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Cell cell = other.GetComponent<Cell>();
        if(cell != null)
        {
            // 땅과 부딪힐 때는 땅을 원래 색으로 바꿈
            cell.ChangeColor(cell.originColor);
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        // 3초 후 다시 없어짐
        StartCoroutine(ReturnObstacle(3f,1));
    }
    private void FixedUpdate()
    {
        // 아래 쪽으로 레이캐스트를 쏴서 땅을 빨간색으로 만듬
        Ray ray = new Ray(transform.position, transform.forward);
        hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if(hits != null)
        {
            foreach(var hit in hits)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.ChangeColor(Color.red);
            }
        }
    }
}
