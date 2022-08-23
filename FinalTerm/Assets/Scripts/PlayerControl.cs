using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] public float jumpPower;
    // 조이스틱 변수
    [SerializeField] public FloatingJoystick joystick;
    [SerializeField] public ParticleSystem damageObject;
    [SerializeField] public ParticleSystem changeEffect;
    [SerializeField] public ParticleSystem BombEffect;
    [SerializeField] GameObject[] checkPoints;

    private const float DEADLINE = -17f;
    private Animator anim;
    public bool isDoubleJump;
    public Rigidbody rigid;
    public int jumpCount;


    // 플레이어의 현재 상태 변수들
    public int maxHp; // 플레이어 현재 최대 체력
    public int hp; // 플레이어 현재 체력
    public int maxLevel; // 플레이어 최대 레벨
    public int level; // 플레이어 현재 레벨
    public int shieldCount;
    public float maxExp; // 플레이어 최대 경험치
    public float exp; // 플레이어 현재 경험치
    public float speed; // 플레이어 현재 스피드
    public float skillExp; // 경험치 증가 스킬로 얻는 추가 경험치량
    public float onHitInvincibilityTime; // 피격 무적 시간
    public float skillInvincibilityTime; // 스킬 무적 시간
    public float coolTimeDecrement;
    public bool isSkillInvincibility;
    public bool isOnHitInvincibility;

    // 경험치 관련 변수들
    public float patternExp;// 패턴 마다 얻는 경험치량
    public float timePerExp;// ??초 마다 얻는 경험치
    public float timer;// 시간을 재는 변수

    //플레어어의 현재 체스말 상태를 나타냄
    public enum State
    {
      //폰/나이트/룩/비숍/킹 순서
        PAWN,KNIGHT, BISHOP, ROOK,KING
    }
    public State state;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        jumpPower = 2000f;
        isDoubleJump = false;
        jumpCount = 0;

        // 플레이어 현재 상태 변수들 초기 값 초기화
        maxHp = 3;
        hp = maxHp;
        maxLevel = 30;
        if (AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxLevel].isAchieve) maxLevel += 10;
        level = 1;
        shieldCount = 0;
        maxExp = 3f;
        exp = 0;
        speed = 20f;
        skillExp = 0;
        onHitInvincibilityTime = 2f;
        skillInvincibilityTime = 5f;
        coolTimeDecrement = 0;

        // 경험치 관련 변수들 초기화
        patternExp = 0;
        timePerExp = 0;
        timer = 0;

        GameManager.Instance.HpImageUpdate();
    }

    private void Update()
    {
        if (level < maxLevel) // 레벨이 최대치에 도달했는지 체크
        {
            //GetExp(0.25f);
            if (exp >= maxExp) // 경험치가 100% 다 채웠는지 체크
                LevelUp();
        }
        else if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxLevel].isAchieve)
        {
            AchieveManager.Instance.AchieveMaxLevel();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver) return;

        //물리작용이므로 FixedUpdated에서 관리
        Move();
        if (transform.position.y < DEADLINE)
        {
            OnDamaged();
            Respawn();
        }
    }
    private void Move()
    {
        if (SkillManager.Instance.isTeleport)
            return;

        float moveX = joystick.Horizontal; // 수평 움직임 값 조이스틱 변수에서 가져옴
        float moveZ = joystick.Vertical; // 수직 움직임 값 조이스틱 변수에서 가져옴

        Vector3 movePos;

        // 점프 상태일때만 y 좌표를 받아 걸을때는 넘어지지 않게 함
        // if(isJump) movePos = new Vector3(moveX * speed, rigid.velocity.y ,moveZ * speed);
        // else movePos = new Vector3(moveX, 0, moveZ) * speed;

        movePos = new Vector3(moveX * speed, rigid.velocity.y, moveZ * speed);

        RotatePlayer(movePos); // 캐릭터 회전

        // 움직이고 있는 상태라면 걷는 애니메이션
        if ((moveX != 0 || moveZ != 0))
        {
            //anim.SetBool("Walk", true);
            if(!SoundManager.Instance.playerAudioSource.isPlaying && jumpCount == 0)
                SoundManager.Instance.PlaySound(SoundManager.Instance.playerAudioSource, SoundManager.Instance.MoveSound);
        }
        // 가만히 있다면 걷는 애니메이션 중지
        else
        {
            //anim.SetBool("Walk", false);
            SoundManager.Instance.StopSound(SoundManager.Instance.playerAudioSource);
        }
        // 속도는 일정
        rigid.velocity = movePos;
    }
    public void Jump()
    {
        // TimeScale == 0 즉 게임이 멈췄을 때 버튼 동작X
        if ((!isDoubleJump && jumpCount > 0) || (isDoubleJump && jumpCount > 1) || Time.timeScale == 0) return;

        SoundManager.Instance.PlaySound(SoundManager.Instance.playerAudioSource, SoundManager.Instance.JumpSound);

        jumpCount++;
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(new Vector3(0, jumpPower, 0));
        //anim.SetBool("Jump", true);
    }

    // 죽을때의 액션을 담당하는 코루틴
    public IEnumerator DieOperate()
    {
        //anim.SetBool("Die",true);
        GameManager.Instance.isGameOver = true;
        this.gameObject.layer = 9; // 플레이어 무적상태
        
        // 1.2초 뒤 GameOver 함수 호출
        yield return new WaitForSeconds(1.2f);
        GameManager.Instance.GameOver();
    }
    
    // 캐릭터가 이동방향 쪽으로 회전하게 함
    private void RotatePlayer(Vector3 pos)
    {
        pos = new Vector3(pos.x, 0f, pos.z);
        if (pos == Vector3.zero) return;
        Quaternion newRotate = Quaternion.LookRotation(pos);
        //한번에 돌아가는게 아닌 자연스럽게 돌아가게 함
        rigid.rotation = Quaternion.Slerp(rigid.rotation, newRotate, Time.deltaTime * speed);
        //pos.y *= -1;
    }


    // 플레이어 피격 시 호출되는 함수
    public void OnDamaged()
    {
        if(shieldCount > 0)
        {
            shieldCount--;
            if(shieldCount == 0)
            {
                Renderer mesh = GetComponentInChildren<MeshRenderer>(); 
                Color color = new Color(195f / 255f, 202f / 255f, 219f / 255f);
                mesh.material.color = color;
            }


            return;
        }

        hp--;
        GameManager.Instance.HpImageUpdate();

        isOnHitInvincibility = true;
        if (hp <= 0)
            StartCoroutine(DieOperate());
        else
            StartCoroutine(Invincibility(onHitInvincibilityTime));

    }
    // 무적 상태 코루틴, 매개변수 time초 만큼 무적
    public IEnumerator Invincibility(float time)
    {
        this.gameObject.layer = 9;

        //데미지 이펙트를 충돌위치로 옮김
        if (!damageObject.isPlaying)
        {
            damageObject.transform.position = transform.position;
            damageObject.Play();
        }

        // 색 변경
        Renderer mesh = GetComponentInChildren<MeshRenderer>();
        mesh.material.color = Color.green;

        yield return new WaitForSeconds(time);
        if (isSkillInvincibility && isOnHitInvincibility)
        {
            isOnHitInvincibility = false;
            yield break;
        }
        else
        {
            this.gameObject.layer = 10;
        }

        isSkillInvincibility = false;
        isOnHitInvincibility = false;

        // 색 돌아옴
        Color color = new Color(195f/255f, 202f/255f, 219f/255f);
        mesh.material.color = color;

    }

    // 레벨 업 함수
    public void LevelUp()
    {
        level++;
        if (level < maxLevel)
        {
            exp = exp - maxExp;
            //maxExp = maxExp * 1.15f + level;
            Debug.Log("MaxExp : " + maxExp);
        }
        else // 레벨이 최대치이면 maxExp로 고정(경험치바 UI가 꽉차보이게)
            exp = maxExp;
        // 스킬 선택창 띄우기
        GameManager.Instance.SetSkillPanels();
        GameManager.Instance.ActivePanel(GameManager.Instance.skillChoicePanel, true);
        SoundManager.Instance.StopSound(SoundManager.Instance.playerAudioSource);
        SoundManager.Instance.PlaySound(SoundManager.Instance.uIAudioSource, SoundManager.Instance.LevelUpSound);
    }

    // 경험치 얻는 함수
    public void GetExp(float delayTime)
    {
        // 시간 당 얻는 경험치 양 계산식
        timePerExp = timePerExp + ( patternExp + skillExp )* Time.deltaTime;

        // 매개변수 delayTime초 마다 경험치를 획득할 수 있하는 코드
        timer = timer + Time.deltaTime;
        if (timer >= delayTime)
        {
            exp = exp + timePerExp;
            timePerExp = 0;
            timer = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj == null) return;
        if (collision.gameObject.layer == 8)
        {
            jumpCount = 0;
            //anim.SetBool("Jump", false);
        }
        else if(obj.GetComponent<AObstacle>() != null)
        {
            AObstacle obstacle = obj.GetComponent<AObstacle>();
            StartCoroutine(obstacle.ReturnObstacle(0, obstacle.Index));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AObstacle obj = other.GetComponent<AObstacle>();
        if (obj == null) return;

        if (other.gameObject.tag == "ExpBall")
            exp += other.gameObject.GetComponent<ExpBall>().exp;
        else
            OnDamaged();
        StartCoroutine(obj.ReturnObstacle(0, obj.Index));
    }


    private void Respawn()
    {
        int idx = Random.Range(0, checkPoints.Length);
        int OFFSET = 2;
        do
        {
            idx = Random.Range(0, checkPoints.Length);
        } while (!checkPoints[idx].activeSelf);

        Vector3 pos = checkPoints[idx].transform.position;

        transform.position = pos + Vector3.up * OFFSET;
    }
}
