using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : AObstacle
{
    [SerializeField] Transform target;
    private Rigidbody rigid;
    public float force;
    public float delayTime;


    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        force = 2700f;
    }

    private void FixedUpdate()
    {
        delayTime += Time.fixedDeltaTime;
        if(delayTime > 0.05f)
        {
            Vector3 direction = Vector3.Normalize(target.position - transform.position);
            rigid.AddForce(direction * force, ForceMode.Force);
            delayTime = 0;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {

        }
    }
}
