using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPattern : APattern // APattern 상속
{
    [SerializeField] int amountPerSpawn = 3; // 초당 떨어지는 공 개수
    [SerializeField] GameObject dropPrefab; // 공 프리팹

    private const float PHASETIME = 10f; // 진행시간
    private const float MAXDELAY = 0.1f; // 최소 딜레이
    private const float MINDELAY = 0.001f; // 최대 딜레이
    private const float HEIGHT = 20f; // 떨어지는 높이
    

    protected override void OnEnable()
    {
        base.OnEnable();
        isAlertEnd = true; // 이 패턴은 경고없이 바로 실행
        StartCoroutine(PhaseTimer(PHASETIME)); //바로 시작

    }
    protected override void StartPattern()
    {
        StartCoroutine(DropObstacle());
        
    }
    // 공 소환 딜레이 코루틴
    private IEnumerator DropObstacle()
    {
        while (!GameManager.Instance.isGameOver)
        {
            float delay = Random.Range(MINDELAY, MAXDELAY);
            yield return new WaitForSeconds(delay);

            for(int i = 0; i < amountPerSpawn; i++) SpawnDrop();
        }
    }
    // 공 소환
    private void SpawnDrop()
    {
        Vector3 start = transform.TransformDirection(sPos.position);
        Vector3 end = transform.TransformDirection(ePos.position);

        // 셀 범위 내에서 랜덤한 위치에 공이 생성되고 떨어짐
        float posX = Random.Range(start.x-5, end.x+5);
        float posZ = Random.Range(start.z-5, end.z+5);
        GameObject drop = ObjectPool.Instance.GetObject(1);
        drop.transform.position = GetDropPosition(posX, posZ);
    }
    //위치를 셀 위치에 떨어지게 하기 위해 픽셀단위 벡터를 반환
    private Vector3 GetDropPosition(float x, float z)
    {
        Vector3 offset = transform.TransformDirection(sPos.position);

        int posX = (int)x / 5 * 5;
        int posZ = (int)z / 5 * 5;

        return new Vector3(posX, HEIGHT, posZ);
    }
}
