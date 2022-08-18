using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // player 변수
    [SerializeField] GameObject[] target;

    // player와의 Offset
    [SerializeField] float topOffsetY = 30f;
    [SerializeField] float topOffsetX = 0;
    [SerializeField] float topOffsetZ = -25f;
    // 선형 보정값
    private float DelayTime = 5f;

    // Update is called once per frame    
    private void Update()
    {
        if(!SkillManager.Instance.isTeleport)
            CameraMove(0, 0, 30f, -25f, 45f);
        else
            CameraMove(1, 0, 50f, 0, 90f);
    }

    private void CameraMove(int index, float offsetX, float offsetY, float offsetZ, float rotate)
    {
        Vector3 pos = target[index].transform.position;
        // Camera 위치 계산 및 선형 보정
        Vector3 FixedPos = new Vector3(pos.x + offsetX, pos.y + offsetY, pos.z + offsetZ);
        transform.position = Vector3.Lerp(transform.position, FixedPos, Time.deltaTime * DelayTime);
        // Camera 각도
        transform.eulerAngles = new Vector3(rotate, 0, 0);
    }


}
