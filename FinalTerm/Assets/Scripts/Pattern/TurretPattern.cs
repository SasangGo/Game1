using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPattern : APattern
{
    [SerializeField] GameObject turret;
    [SerializeField] GameObject shotPoint;
    [SerializeField] float attackRange;
    [SerializeField] int amountRandomBullet;
    [SerializeField] GameObject player;
    [SerializeField] GameObject clone;
    private const float PHASETIME = 10f;
    private Vector3 bulletRot;
    private Transform target;

    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 1f;
        turret.SetActive(false); // 터렛 비활성화
        StartCoroutine(PhaseTimer(PHASETIME)); // 패턴 시작 타이머
        StartCoroutine(AlertHazard(hazardZones)); // 패턴 전 경고
    }

    protected override void StartPattern()
    {
        base.StartPattern();
        turret.SetActive(true);
        InvokeRepeating("UpdateTarget", 0, 0.25f); // 타겟을 0.25초마다 찾음
        InvokeRepeating("ShootBullet", 1, 0.3f); //ShootBullet() 함수를 1초후 0.3초마다 실행
    }
    // 총알 발사
    private void ShootBullet()
    {
        if (target == null) return;
        GameObject bullet = ObjectPool.Instance.GetObject(0);
        bulletRot = target.position;
        if(clone.activeSelf)
            bulletRot = clone.transform.position;
        else
            bulletRot = player.transform.position;
        bullet.transform.position = shotPoint.transform.position;
        bulletRot.y = shotPoint.transform.position.y;
        bullet.transform.LookAt(bulletRot);

        GameObject[] bullets = new GameObject[amountRandomBullet];
        for(int i = 0; i < bullets.Length; i++)
        {
            bullets[i] = ObjectPool.Instance.GetObject(0);
            bullets[i].transform.position = shotPoint.transform.position;
            bullets[i].transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            bulletRot.y = shotPoint.transform.position.y;
        }
    }
    private void UpdateTarget()
    {
        Collider[] cols = Physics.OverlapSphere(turret.transform.position, attackRange, LayerMask.GetMask("Player"));
        for(int i = 0; i < cols.Length; i++)
        {
            PlayerControl newTarget= cols[i].GetComponent<PlayerControl>();
            if (newTarget != null)
            {
                target = newTarget.transform;
                return;
            }
        }
        target = null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(turret.transform.position, attackRange);
    }
}
