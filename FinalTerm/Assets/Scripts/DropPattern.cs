using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPattern : APattern
{
    [SerializeField] int amountPerSpawn = 3;
    [SerializeField] GameObject dropPrefab;

    private const float PHASETIME = 10f;
    private const float MAXDELAY = 0.1f;
    private const float MINDELAY = 0.001f;
    private const float HEIGHT = 20f;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        isAlertEnd = true;
        StartCoroutine(PhaseTimer(PHASETIME));

    }
    protected override void StartPattern()
    {
        StartCoroutine(DropObstacle());
        
    }
    private IEnumerator DropObstacle()
    {
        while (!GameManager.Instance.isGameOver)
        {
            float delay = Random.Range(MINDELAY, MAXDELAY);
            yield return new WaitForSeconds(delay);

            for(int i = 0; i < amountPerSpawn; i++) SpawnDrop();
        }
    }
    private void SpawnDrop()
    {
        Vector3 start = transform.TransformDirection(sPos.position);
        Vector3 end = transform.TransformDirection(ePos.position);

        float posX = Random.Range(start.x-5, end.x+5);
        float posZ = Random.Range(start.z-5, end.z+5);
        GameObject drop = ObjectPool.Instance.GetObject(1);
        drop.transform.position = GetDropPosition(posX, posZ);
    }
    private Vector3 GetDropPosition(float x, float z)
    {
        Vector3 offset = transform.TransformDirection(sPos.position);

        int posX = (int)x / 5 * 5;
        int posZ = (int)z / 5 * 5;

        return new Vector3(posX, HEIGHT, posZ);
    }
}
