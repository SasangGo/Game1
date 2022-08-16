using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBall : AObstacle
{
    public float speed = 1f;
    public int exp = 5;

    private void OnEnable()
    {
        StartCoroutine(ReturnObstacle(3f, 3)); //3초 뒤 반환
    }
    void Update()
    {
        if (transform.position.y >= -6f)
            speed = -1f;
        else if (transform.position.y <= -7f)
            speed = 1f;

        transform.position = transform.position + new Vector3(0, speed * Time.deltaTime, 0);
    }
}
