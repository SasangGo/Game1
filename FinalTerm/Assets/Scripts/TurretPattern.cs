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
        turret.SetActive(false);
        StartCoroutine(PhaseTimer(PHASETIME));
        StartCoroutine(AlertHazard(hazardZones));
    }

    protected override void StartPattern()
    {
        base.StartPattern();
        turret.SetActive(true);
        InvokeRepeating("ShootBullet", 1, 0.3f);
    }
    private void ShootBullet()
    {
        GameObject bullet = ObjectPool.Instance.GetObject(0);
        bulletRot = player.transform.position;
        bullet.transform.position = shotPoint.transform.position;
        bulletRot.y = shotPoint.transform.position.y;
        bullet.transform.LookAt(bulletRot);
    }
}
