using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AObstacle
{
    private Vector3 shotPos; // 발사 위치
    private float shotSpeed; // 날아가는 속도
    protected override void OnEnable()
    {
        base.OnEnable();
        if (GameManager.Instance.isGameOver) return;

        shotSpeed = 25f;
        StartCoroutine(ReturnObstacle(3f,0)); //3초 뒤 반환
    }

    private void Start()
    {

        SoundManager.Instance.PlaySound(SoundManager.Instance.objectAudioSource, SoundManager.Instance.turretBulletSound);
    }

    void Update()
    {
        // 앞으로 날아감
        transform.position += transform.forward * Time.deltaTime * shotSpeed;

    }
}
