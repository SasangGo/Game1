using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    [SerializeField] float rayLength = 5f;

    private Animator anim;
    private const float MAXY = 30f; 
    private const float MINY = -30F; 
    private bool isJump;
    private Rigidbody rigid;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        speed = 20f;
        jumpPower = 2000f;
        isJump = false;

    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver) return;

        if (rigid.velocity.y < -0.1f)
        {
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, rayLength, LayerMask.GetMask("Ground")))
            {
                isJump = false;
            }
        }
        Move();
        Jump();
        
    }
    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movePos = new Vector3(moveX * speed, rigid.velocity.y ,moveZ * speed);
        RotatePlayer(movePos);
        if (moveX != 0 || moveZ != 0) anim.SetBool("Walk", true);
        else anim.SetBool("Walk", false);
        rigid.velocity = movePos;
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;
            rigid.AddForce(new Vector3(0, jumpPower, 0));
        }

        if (isJump) anim.SetBool("Jump", true);
        else anim.SetBool("Jump", false);
    }

    public IEnumerator DieOperate()
    {
        anim.SetBool("Die",true);
        GameManager.Instance.isGameOver = true;
        this.gameObject.layer = 9;
        yield return new WaitForSeconds(1.2f);
        GameManager.Instance.GameOver();
    }
    
    private void RotatePlayer(Vector3 pos)
    {
        if (pos == Vector3.zero) return;
        Quaternion newRotate = Quaternion.LookRotation(pos);
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRotate, Time.deltaTime * speed);
        //pos.y *= -1;
        newRotate = Quaternion.LookRotation(pos);
    }
}
