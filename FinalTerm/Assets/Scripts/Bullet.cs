using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AObstacle
{
    private Vector3 shotPos;
    private float shotSpeed;
    private void OnEnable()
    {
        if (GameManager.Instance.isGameOver) return;

        shotSpeed = 25f;
        StartCoroutine(ReturnObstacle(3f,0));
    }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * shotSpeed;

    }
}
