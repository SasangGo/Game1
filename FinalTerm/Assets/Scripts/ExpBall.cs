using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBall : AObstacle
{
    public float speed = 1f;
    public float time = 0;
    public int exp = 5;

    private void OnEnable()
    {
        base.OnEnable();
        transform.position = SkillManager.Instance.GetRandomPosition();
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
