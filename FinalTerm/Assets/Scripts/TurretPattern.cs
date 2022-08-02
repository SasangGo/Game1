using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPattern : APattern
{
    [SerializeField] GameObject turret;
    [SerializeField] GameObject shotPoint;
    [SerializeField] GameObject player;

    private const float PHASETIME = 10f;
    private Vector3 bulletRot;

    protected override void OnEnable()
    {
        base.OnEnable();
        turret.SetActive(false); // 터렛 비활성화
        StartCoroutine(PhaseTimer(PHASETIME)); // 패턴 시작 타이머
        StartCoroutine(AlertHazard(hazardZones)); // 패턴 전 경고
    }

    protected override void StartPattern()
    {
        base.StartPattern();
        turret.SetActive(true);
        InvokeRepeating("ShootBullet", 1, 0.3f); //ShootBullet() 함수를 1초후 0.3초마다 실행
    }
    // 총알 발사
    private void ShootBullet()
    {
        GameObject bullet = ObjectPool.Instance.GetObject(0);
        bulletRot = player.transform.position;
        bullet.transform.position = shotPoint.transform.position;
        bulletRot.y = shotPoint.transform.position.y;
        bullet.transform.LookAt(bulletRot);
    }
}
