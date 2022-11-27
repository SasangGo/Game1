using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straight_Ball : AObstacle
{
    private const float BALL_SPEED = 80F;
    private const int LIFE_TIME = 4;
    private Vector3 dir;
    private Rigidbody rigid;
<<<<<<< HEAD
    // Update is called once per frame
=======
>>>>>>> 434d4a809ae3601691ad8e48afa5ff76decce608

    void Start()
    {
        StartCoroutine(ReturnObstacle(LIFE_TIME, 5));
        InvokeRepeating("UpdateTarget", 0, 0.25f);
        rigid = GetComponent<Rigidbody>();
        Invoke("SetBall",0.3f);
    }

    private void SetBall()
    {
        if (point == null) return;
        dir = (point.position - transform.position).normalized;
        dir = new Vector3(dir.x, 0, dir.z);
        rigid.AddForce(dir * BALL_SPEED,ForceMode.Impulse);
    }
}