﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkTurretPattern : APattern
{
    [SerializeField] int amountSpawn = 3;
    [SerializeField] GameObject[] turrets;

    private const float PHASETIME = 10f; // 진행시간
    private const float HEIGHT = -20f;

    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 1f;
        isAlertEnd = true;
        StartCoroutine(PhaseTimer(PHASETIME)); //바로 시작
    }

    protected override void StartPattern()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (!GameManager.Instance.isGameOver)
        {
            for (int i = 0; i < amountSpawn; i++) SpawnTurret(turrets[i]);
            yield return new WaitForSeconds(5f);
        }
    }

    private void SpawnTurret(GameObject turret)
    {
        Vector3 start = transform.TransformDirection(sPos.position);
        Vector3 end = transform.TransformDirection(ePos.position);

        // 셀 범위 내에서 랜덤한 위치에 창이 생성됨
        float posX = Random.Range(start.x - 5, end.x + 5);
        float posZ = Random.Range(start.z - 5, end.z + 5);
        turret.transform.position = GetPosition(posX, HEIGHT, posZ);
        turret.SetActive(true);
    }

    // 위치를 셀 위치에 떨어지게 하기 위해 픽셀단위 벡터를 반환
    private Vector3 GetPosition(float x, float y, float z)
    {
        int posX = (int)x / 5 * 5;
        int posZ = (int)z / 5 * 5;

        return new Vector3(posX, y, posZ);
    }
}