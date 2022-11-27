using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasing_Ball : AObstacle
{
    private const float MAX_SPEED = 40F;
    private const float ATTACK_OFFSET = 5F;
    private const float DECREASE_PERCENT = 0.3F;
    private const float ROTATION_SPEED = 80F;
    private const int LIFE_TIME = 10;

    private Rigidbody rigid;
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0, 0.5f);
        StartCoroutine(ReturnObstacle(LIFE_TIME, 6));
        rigid = GetComponent<Rigidbody>();
    }

<<<<<<< HEAD
    // Update is called once per frame
=======
>>>>>>> 434d4a809ae3601691ad8e48afa5ff76decce608
    void Update()
    {
        if (rigid.velocity.magnitude > MAX_SPEED) rigid.velocity = rigid.velocity * DECREASE_PERCENT;
        rigid.AddForce(transform.forward, ForceMode.VelocityChange);
        if (point == null) return;
        Vector3 relativeDir = (point.position - transform.position) + Vector3.up * ATTACK_OFFSET;
        Quaternion dir = Quaternion.LookRotation(relativeDir);
<<<<<<< HEAD
        Debug.Log(dir);
=======
>>>>>>> 434d4a809ae3601691ad8e48afa5ff76decce608
        transform.rotation = Quaternion.Slerp(transform.rotation, dir, Time.deltaTime * ROTATION_SPEED);
    }
}
