using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // player 변수
    [SerializeField] GameObject target;
    
    // player와의 Offset
    private float topOffsetX = 0;
    private float topOffsetY = 20f;
    private float topOffsetZ = -25f;
    // 선형 보정값
    private float DelayTime = 5f;

    // Update is called once per frame    
    private void Update()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        // Camera 위치 계산 및 선형 보정
        Vector3 FixedPos = new Vector3(target.transform.position.x + topOffsetX, target.transform.position.y + topOffsetY, target.transform.position.z + topOffsetZ);
        transform.position = Vector3.Lerp(transform.position, FixedPos, Time.deltaTime * DelayTime);
        // Camera 각도
        transform.eulerAngles = new Vector3(45f, 0, 0);
    }

}
