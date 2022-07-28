using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform player;
    private const float MAXY = 30f;
    private const float MINY = -30f;
    private const float OFFSETY = 6f;
    private const float OFFSETZ = -4f;

    [SerializeField] float sensity;

    float xRot;
    float yRot;
    // Start is called before the first frame update
    void Start()
    {
        xRot = 0;
        yRot = 0;
        sensity = 100f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver) return;

        //MoveCamera();
        RotateCamera();
    }
    private void RotateCamera()
    {
        xRot = Input.GetAxis("Mouse X") * sensity * Time.deltaTime;
        yRot = Input.GetAxis("Mouse Y") * sensity * Time.deltaTime;
        yRot = Mathf.Clamp(yRot, MINY, MAXY);

        transform.Rotate(Vector3.up, xRot,Space.World);
        transform.Rotate(Vector3.right, yRot);

    }
    private void MoveCamera()
    {

        Vector3 camPos = new Vector3(player.position.x + xRot, player.position.y + OFFSETY, player.position.z + OFFSETZ);
        transform.position = camPos;
    }
}
