using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : AObstacle
{
    private Cell cell;
    private Vector3 beforePos;
    private Vector3 affterPos;
    public float speed = 8f;
    public bool isStart;

    private void Start()
    {
        ChangeAlertColor();
        Invoke("EnalbleSpear", 1f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void OnDisable()
    {
        isStart = false;
    }

    private void Update()
    {
        if(isStart)
            transform.position = Vector3.MoveTowards(transform.position, affterPos, speed);
    }

    private void ChangePos()
    {
        affterPos = beforePos;
    }

    private void ChangeAlertColor()
    {
        // 아래 쪽으로 레이캐스트를 쏴서 땅을 빨간색으로 만듬
        Ray ray = new Ray(transform.position, transform.up * 20f);
        hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Ground"));
        
        if (hits != null)
        {
            foreach (var hit in hits)
            {
                cell = hit.collider.GetComponent<Cell>();
                cell.ChangeColor(Color.red);
                Invoke("ChangeOriginColor", 1f);
            }
        }
    }
    private void ChangeOriginColor()
    {
        // 땅과 부딪힐 때는 땅을 원래 색으로 바꿈
        cell.ChangeColor(cell.originColor);
    }

    private void EnalbleSpear()
    {
        beforePos = transform.position;
        affterPos = new Vector3(beforePos.x, -5f, beforePos.z);
        isStart = true;
        Invoke("ChangePos", 1.5f);
        StartCoroutine(ReturnObstacle(3f, 4));
    }
}
