using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBlockPattern : APattern
{
    [SerializeField] GameObject mark;

    private const float PHASETIME = 10f; // 진행시간

    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 4f;
        isAlertEnd = true; // 이 패턴은 경고없이 바로 실행
        StartCoroutine(PhaseTimer(PHASETIME)); //바로 시작
        InvokeRepeating("DirectMark", 0f, 2f);
        mark.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        mark.SetActive(false);
    }

    private void DirectMark()
    {
        Vector3 start = transform.TransformDirection(sPos.position);
        Vector3 end = transform.TransformDirection(ePos.position);

        // 셀 범위 내에서 랜덤한 위치에 창이 생성됨
        float posX = Random.Range(start.x - 5, end.x + 5);
        float posZ = Random.Range(start.z - 5, end.z + 5);

        mark.GetComponent<Mark>().direction = GetPosition(posX, mark.transform.position.y, posZ);
    }

    // 위치를 셀 위치에 떨어지게 하기 위해 픽셀단위 벡터를 반환
    private Vector3 GetPosition(float x, float y, float z)
    {
        int posX = (int)x / 5 * 5;
        int posZ = (int)z / 5 * 5;

        return new Vector3(posX, y, posZ);
    }
}
