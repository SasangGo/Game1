using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearPattern : APattern
{
    [SerializeField] int amountSpawn = 40; // 초당 떨어지는 공 개수

    private const float PHASETIME = 10f; // 진행시간
    private const float HEIGHT = -20f; // 떨어지는 높이


    protected override void OnEnable()
    {
        base.OnEnable();
        expAmount = 1f;
        isAlertEnd = true;
        StartCoroutine(PhaseTimer(PHASETIME)); //바로 시작

    }
    protected override void StartPattern()
    {
        StartCoroutine(SpearObstacle());

    }
    // 공 소환 딜레이 코루틴
    private IEnumerator SpearObstacle()
    {
        while (!GameManager.Instance.isGameOver)
        {
            for (int i = 0; i < amountSpawn; i++) SpawnSpear();
            yield return new WaitForSeconds(5f);
        }
    }
    // 공 소환
    private void SpawnSpear()
    {
        Vector3 start = transform.TransformDirection(sPos.position);
        Vector3 end = transform.TransformDirection(ePos.position);

        // 셀 범위 내에서 랜덤한 위치에 창이 생성됨
        float posX = Random.Range(start.x - 5, end.x + 5);
        float posZ = Random.Range(start.z - 5, end.z + 5);
        GameObject spear = ObjectPool.Instance.GetObject(4);
        spear.transform.position = GetPosition(posX, HEIGHT, posZ);
    }

    // 위치를 셀 위치에 떨어지게 하기 위해 픽셀단위 벡터를 반환
    private Vector3 GetPosition(float x, float y, float z)
    {
        int posX = (int)x / 5 * 5;
        int posZ = (int)z / 5 * 5;

        return new Vector3(posX, y, posZ);
    }
}
