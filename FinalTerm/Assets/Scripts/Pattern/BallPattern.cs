using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPattern : APattern
{
    [SerializeField] GameObject ball;
    [SerializeField] GameObject wall;

    private const float PHASETIME = 10f;
    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 1f;
        wall.SetActive(true);
        StartCoroutine(PhaseTimer(PHASETIME)); // 패턴 시작 타이머
        StartCoroutine(AlertHazard(hazardZones)); // 패턴 전 경고
    }

    protected override void StartPattern()
    {
        base.StartPattern();
        ball.SetActive(true);
    }
    private void OnDisable()
    {
        ball.transform.position = new Vector3(0, 20f, 0);
        wall.SetActive(false); // 벽 비활성화
        ball.SetActive(false); // 볼 비활성화
    }

}
