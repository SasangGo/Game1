using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    [SerializeField] float rayLength = 5f;

    private const float DEADLINE = -17f;
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

        
        if (rigid.velocity.y < -0.1f) // 플레이어가 아래로 떨어질때
        {
            
            // Ground 레이어를 감지하는 레이캐스트를 플레이어 아랫방향으로 쏨
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, rayLength, LayerMask.GetMask("Ground")))
            {
                // 플레이어가 지면에 닿을때(Raycast Hit)일 때
                isJump = false;
            }
        }
        //물리작용이므로 FixedUpdated에서 관리
        Move();
        Jump();
        if (transform.position.y < DEADLINE) StartCoroutine(DieOperate());
    }
    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal"); // 수평 움직임
        float moveZ = Input.GetAxis("Vertical"); // 수직 움직임

        Vector3 movePos = new Vector3(moveX * speed, rigid.velocity.y ,moveZ * speed);
        RotatePlayer(movePos); // 캐릭터 회전
        // 움직이고 있는 상태라면 걷는 애니메이션
        if (moveX != 0 || moveZ != 0) anim.SetBool("Walk", true);
        // 가만히 있다면 걷는 애니메이션 중지
        else anim.SetBool("Walk", false);
        // 속도는 일정
        rigid.velocity = movePos;
    }
    private void Jump()
    {
        // isJump == false 일때
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;
            rigid.AddForce(new Vector3(0, jumpPower, 0));
        }

        if (isJump) anim.SetBool("Jump", true);
        else anim.SetBool("Jump", false);
    }

    // 죽을때의 액션을 담당하는 코루틴
    public IEnumerator DieOperate()
    {
        anim.SetBool("Die",true);
        GameManager.Instance.isGameOver = true;
        this.gameObject.layer = 9; // 플레이어 무적상태
        
        // 1.2초 뒤 GameOver 함수 호출
        yield return new WaitForSeconds(1.2f);
        GameManager.Instance.GameOver();
    }
    
    // 캐릭터가 이동방향 쪽으로 회전하게 함
    private void RotatePlayer(Vector3 pos)
    {
        if (pos == Vector3.zero) return;
        Quaternion newRotate = Quaternion.LookRotation(pos);
        //한번에 돌아가는게 아닌 자연스럽게 돌아가게 함
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRotate, Time.deltaTime * speed);
        //pos.y *= -1;
        newRotate = Quaternion.LookRotation(pos);
    }
}
