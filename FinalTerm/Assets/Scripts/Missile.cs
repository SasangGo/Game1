using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : AObstacle
{
    [SerializeField] float speed;
    private RaycastHit[] hits;
    private bool timerEnd;
    private void OnEnable()
    {
        speed = 200f;
        timerEnd = false;
        StartCoroutine(Timer(2f));
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Cell cell = other.GetComponent<Cell>();
        if(cell != null)
        {
            cell.ChangeColor(cell.originColor);
        }
    }

    private IEnumerator Timer(float time)
    {
        float cnt = 0;
        while(cnt <= time)
        {
            cnt += Time.deltaTime;
            yield return null;
        }
        timerEnd = true;
        StartCoroutine(ReturnObstacle(3f, 2));

    }
    void Update()
    {
        if (!timerEnd) return;
        transform.position += -transform.forward * Time.deltaTime * speed;
    }
    private void FixedUpdate()
    {
        if (timerEnd) return;

        Ray ray = new Ray(transform.position, -transform.forward);
        hits = Physics.SphereCastAll(ray, 5f,Mathf.Infinity,LayerMask.GetMask("Ground"));
        if (hits != null)
        {
            foreach (RaycastHit hit in hits)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.ChangeColor(Color.red);
            }
        }
    }
}
