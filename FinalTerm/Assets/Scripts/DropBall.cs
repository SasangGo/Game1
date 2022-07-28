using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBall : AObstacle
{
    private RaycastHit[] hits;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Cell cell = other.GetComponent<Cell>();
        if(cell != null)
        {
            cell.ChangeColor(cell.originColor);
        }
    }
    private void OnEnable()
    {
        StartCoroutine(ReturnObstacle(3f,1));
    }
    private void FixedUpdate()
    {
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
