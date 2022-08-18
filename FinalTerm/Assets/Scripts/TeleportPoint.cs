using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    [SerializeField] Joystick joystick;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SkillManager.Instance.isTeleport)
            Move(10f);
    }

    private void Move(float speed)
    {
        float moveX = joystick.Horizontal; // 수평 움직임 값 조이스틱 변수에서 가져옴
        float moveZ = joystick.Vertical; // 수직 움직임 값 조이스틱 변수에서 가져옴

        Vector3 movePos = new Vector3(moveX * speed * Time.deltaTime, 0, moveZ * speed * Time.deltaTime);
        transform.position += movePos;
    }
}
