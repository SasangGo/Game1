﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePattern : APattern
{
    [SerializeField] float MAXDELAY = 2F;
    [SerializeField] float MINDELAY = 0.5F;
    [SerializeField] int amountPerSpawn; // 미사일 생성 개수
    private const float PHASETIME = 10f; // 진행 시간
    private const float HEIGHT = -5f; // 미사일 높이
    private const float OFFSET = 50f;


    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 2f;
        isAlertEnd = true; // 경고 없이 바로 실행
        StartCoroutine(PhaseTimer(PHASETIME));

    }
    protected override void StartPattern()
    {
        base.StartPattern();
        for (int i = 0; i < amountPerSpawn; i++)
            StartCoroutine(SpawnMissile());
    }
    // 미사일 생성
    private IEnumerator SpawnMissile()
    {
        float time;
        while (!GameManager.Instance.isPhaseEnd)
        {
            // 랜덤 시간만큼 대기
            time = Random.Range(MINDELAY, MAXDELAY);
            yield return new WaitForSeconds(time);
            // 랜덤된 위치에 랜덤하게 생성
            Vector3 start = transform.TransformDirection(sPos.position);
            Vector3 end = transform.TransformDirection(ePos.position);

            float posX = Random.Range(start.x, end.x);
            float posZ = Random.Range(start.z, end.z);

            GameObject missile = ObjectPool.Instance.GetObject(2);
            int index = Random.Range(0, 4);
            switch (index)
            {
                case 0:
                    missile.transform.Rotate(Vector3.up, 180);
                    missile.transform.position = new Vector3(posX, HEIGHT, start.z - OFFSET);
                    break;
                case 1:
                    //missile.transform.Rotate(Vector3.up, 180);
                    missile.transform.position = new Vector3(posX, HEIGHT, end.z + OFFSET);
                    break;
                case 2:
                    missile.transform.Rotate(Vector3.up, 270);
                    missile.transform.position = new Vector3(start.x - OFFSET, HEIGHT, posZ);
                    break;
                case 3:
                    missile.transform.Rotate(Vector3.up, 90);
                    missile.transform.position = new Vector3(end.x + OFFSET, HEIGHT, posZ);
                    break;
                    // 미사일 바라보는 방향 컨트롤
            }
        }

    }
}
