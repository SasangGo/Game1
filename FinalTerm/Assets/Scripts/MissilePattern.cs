using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePattern : APattern
{
    [SerializeField] int amountPerSpawn;
    private const float PHASETIME = 10f;
    private const float HEIGHT = -5f;
    private const float OFFSET = 50f;
    private float delay = 2f;


    protected override void OnEnable()
    {
        base.OnEnable();
        isAlertEnd = true;
        StartCoroutine(PhaseTimer(PHASETIME));

    }
    protected override void StartPattern()
    {
        base.StartPattern();
        for(int i = 0; i < amountPerSpawn; i++)
            InvokeRepeating("SpawnMissile",2f,delay);
    }

    private void SpawnMissile()
    {
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
        }
    }
}
