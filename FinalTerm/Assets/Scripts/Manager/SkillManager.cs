using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : Singleton<SkillManager>
{
    public Text debugText;
    public Text debugText2;
    // 스킬의 Index
    public enum Skills { IncreaseMaxHp, IncreaseInvincibilityTime, IncreaseSpeed, IncreaseExp, DecreaseCoolTime, IncreaseJump, Heal, ExpBall, RandomAll, Clone, Bomb, Dash, SizeDown, DoubleJump, SkillHeal, Teleport, Shield, InvincibilitySkill, Wall, PAWN, KNIGHT, BISHOP, ROOK, KING };

    // 스킬 세팅
    public List<string> skillNames; // 스킬 이름을 담는 List
    public List<string> skill_Info; // 스킬의 정보를 담는 List
    public List<Sprite> skillSprites; // 스킬의 이미지를 담는 List

    public Skills[] actSkillButtonNumber; // 엑티브 스킬 버튼 i번째에 담긴 스킬 정보
    public int ActSkillIndex; // 엑티브 스킬 버튼 인덱스
    public int transformSkillIndex; // 변신 스킬 버튼 인덱스

    public bool[] isGetTransformSkill; // 각 스킬들의 상태가 Max인지 체크하기 위한 배열
    public bool[] isMaxSkillLevel; // 각 스킬들의 상태가 Max인지 체크하기 위한 배열
    public int[] skillLevel; // 각 스킬들 레벨을 체크하는 배열
    public int totalSkillsCount; // 이 게임에 존재하는 Skill 개수 변수
    public int maxSkillCount; // Max가 된 스킬들이 몇개인지 체크하기 위한 변수

    // 텔레포트 관련 변수
    public bool isTeleport;
    public bool isTeleportInvokeEnd;
    public int teleportButtonIndex;
    public float teleportCoolTime;

    // 각 패시브 스킬 증가량
    public int HpIncrement, HealIncrement;
    public float speedIncrement, coolTimeDecrease, onHitInvincibilityTimeIncrement, skillExpIncrement, jumpIncrement;

    // 각 패시브 스킬 최대값
    public int maxHpLevel, maxSpeedLevel, maxSkillExpLevel, maxOnHitInvincibilityTimeLevel, maxCoolTimeLevel, maxJumpLevel;

    [SerializeField] Transform sPos; // 셀의 시작 위치(거리 체크용)
    [SerializeField] Transform ePos;// 셀의 끝 위치(거리 체크용)
    [SerializeField] PlayerControl player; // player의 정보를 가지고 오기 위한 변수
    [SerializeField] GameObject clone;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject teleportPoint; // 텔레포트 좌표
    [SerializeField] MeshFilter[] types;    

    private const float HEIGHT = -7f;

    void Start()
    {
        InitSkill();
        if (AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxOnHit].isAchieve) maxOnHitInvincibilityTimeLevel += 1;
        if (AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxSpeed].isAchieve) maxSpeedLevel += 1;
        if (AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxExpSkill].isAchieve) maxSkillExpLevel += 1;
        if (AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxCoolTime].isAchieve) maxCoolTimeLevel += 1;
        if (AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxJump].isAchieve) maxJumpLevel += 1;

        totalSkillsCount = (int)Skills.Heal + 1;

        ActSkillIndex = 0;
        transformSkillIndex = 2;
        actSkillButtonNumber = new Skills[GameManager.Instance.activeSkillButtons.Length];

        maxSkillCount = 0;
        isMaxSkillLevel = new bool[totalSkillsCount];
        skillLevel = new int[totalSkillsCount];
        for (int i = 0; i < totalSkillsCount; i++)
        {
            isMaxSkillLevel[i] = false;
            skillLevel[i] = 1;
        }

        GetActiveSkill(Skills.RandomAll);
        GetActiveSkill(Skills.Bomb);

    }

    // Update is called once per frame
    void Update()
    {
        if (isTeleportInvokeEnd)
        {
            isTeleportInvokeEnd = false;
            StartCoroutine(SkillCoolDown(teleportButtonIndex, teleportCoolTime));
        }

    }

    // 스킬 기본 설정, 여기서 설정 하면 됨
    public void InitSkill()
    {
        SetSkillsInfo("Hp 증가", "MaxHp를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetSkillsInfo("피격무적 시간 증가", "피격무격 시간을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/InvincibleTimeUpOnHit"));
        SetSkillsInfo("Speed 증가", "Speed를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/SpeedUp"));
        SetSkillsInfo("경험치 증가", "경험치 획득량을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        SetSkillsInfo("스킬 쿨타임 감소", "스킬 쿨타임이 10% 감소한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        SetSkillsInfo("점프력 증가", "점프 높이가 상승한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        
        SetSkillsInfo("Hp 회복", "Hp를 2 회복시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/Heal"));

        SetSkillsInfo("ExpBall", "ExpBall", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("RandomAll", "RandomAll", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("Clone", "Clone", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("Bomb", "Bomb", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("Dash", "Dash", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));

        SetSkillsInfo("SizeDown", "SizeDown", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("DoubleJump", "DoubleJump", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("SkillHeal", "SkillHeal", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("Teleport", "Teleport", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("Shield", "Shield", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));

        SetSkillsInfo("InvincibilitySkill", "InvincibilitySkill", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("Wall", "Wall", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("폰", "폰으로 변신한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        SetSkillsInfo("나이트", "나이트로 변신한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        SetSkillsInfo("비숍", "비숍으로 변신한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));

        SetSkillsInfo("룩", "룩으로 변신한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        SetSkillsInfo("킹", "킹으로 변신한다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));

        // 각 패시브 스킬 증가량 초기화
        HpIncrement = 1;
        HealIncrement = 2;
        speedIncrement = 2f;
        onHitInvincibilityTimeIncrement = 1f;
        skillExpIncrement = 1f;
        coolTimeDecrease = 10f;
        jumpIncrement = 200f;

        // 각 패시브 스킬 최대값 초기화
        maxHpLevel = 8;
        maxSpeedLevel = 5;
        maxOnHitInvincibilityTimeLevel = 5;
        maxSkillExpLevel = 5;
        maxCoolTimeLevel = 5;
        maxJumpLevel = 5;
    }

    // 스킬 정보 설정 함수
    // SetSkillsInfo(스킬 이름, 스킬 설명, 스킬 이미지)
    public void SetSkillsInfo(string skillName, string skillExplan, Sprite skillSprite)
    {
        skillNames.Add(skillName);
        skill_Info.Add(skillExplan);
        skillSprites.Add(skillSprite);
    }

    // 스킬들 중, 랜덤으로 N 선택
    public List<int> RandomSkill(int N)
    {
        // 랜덤 숫자 N개를 담을 List 생성
        List<int> randomNum = new List<int>();

        // Max치가 정해져 있는 스킬들이 Max상태 일 경우
        if (totalSkillsCount - maxSkillCount <= 1)
        {
            for (int i = 0; i < N; i++)
                randomNum.Add((int)Skills.Heal);
            return randomNum;
        }

        // 랜덤 스킬 중복 제외하고 뽑음
        int temp = Random.Range(0, totalSkillsCount);
        for (int i = 0; i < N; i++)
        {
            // List에 포함되 있는지, 뽑은 스킬이 이미 Max인지 체크 해서 제외시키기
            while (randomNum.Contains(temp) || isMaxSkillLevel[temp] || (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxHp].isAchieve && temp == (int)Skills.Heal))
                temp = Random.Range(0, totalSkillsCount);

            randomNum.Add(temp);
        }

        return randomNum;
    }

    // 각 스킬 기능 함수
    public void ChoiceSkillApply(Skills skill)
    {
        switch (skill)
        {
            // Hp 최대치 증가 스킬
            case Skills.IncreaseMaxHp:
                InscreaseMaxHp(maxHpLevel, HpIncrement);
                break;
            // 피격 무적 시간 증가 스킬
            case Skills.IncreaseInvincibilityTime:
                IncreaseOnHitInvincibilityTime(maxOnHitInvincibilityTimeLevel, onHitInvincibilityTimeIncrement);
                break;
            // 스피드 증가 스킬
            case Skills.IncreaseSpeed:
                IncreaseSpeed(maxSpeedLevel, speedIncrement);
                break;
            // 경험치 획득량 증가 스킬
            case Skills.IncreaseExp:
                IncreaseSkillExp(maxSkillExpLevel, skillExpIncrement);
                break;
            // 쿨타임 감소 스킬
            case Skills.DecreaseCoolTime:
                DecreaseCoolTime(maxCoolTimeLevel, coolTimeDecrease);
                break;
            // 점프력 증가 스킬
            case Skills.IncreaseJump:
                IncreaseJump(maxJumpLevel, jumpIncrement);
                break;
            // Hp 회복 스킬
            case Skills.Heal:
                Heal(HealIncrement);
                break;
        }
    }

    // Hp 최대치 증가
    public void InscreaseMaxHp(int maxLevel, int increment)
    {
        player.maxHp = player.maxHp + increment;

        skillLevel[(int)Skills.IncreaseMaxHp]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.IncreaseMaxHp] >= maxLevel)
        {
            isMaxSkillLevel[(int)Skills.IncreaseMaxHp] = true;
            maxSkillCount++;
            if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxHp].isAchieve)
            {
                AchieveManager.Instance.AchieveMaxHp();
            }
        }
        GameManager.Instance.HpImageUpdate();
    }

    // 피격무적 시간 증가
    public void IncreaseOnHitInvincibilityTime(int maxLevel, float increment)
    {
        player.onHitInvincibilityTime = player.onHitInvincibilityTime + increment;

        skillLevel[(int)Skills.IncreaseInvincibilityTime]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.IncreaseInvincibilityTime] >= maxLevel)
        {
            if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxOnHit].isAchieve)
            {
                AchieveManager.Instance.AchieveMaxOnHit();
            }
            else
            {
                isMaxSkillLevel[(int)Skills.IncreaseInvincibilityTime] = true;
                maxSkillCount++;
            }
        }
    }

    // 스피드 증가
    public void IncreaseSpeed(int maxLevel, float increment)
    {
        player.speed = player.speed + increment;

        skillLevel[(int)Skills.IncreaseSpeed]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.IncreaseSpeed] >= maxLevel)
        {
            if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxSpeed].isAchieve)
            {
                AchieveManager.Instance.AchieveMaxSpeed();
            }
            else
            {
                isMaxSkillLevel[(int)Skills.IncreaseSpeed] = true;
                maxSkillCount++;
            }
        }
    }

    // 경험치 획득량 증가
    public void IncreaseSkillExp(int maxLevel, float increment)
    {
        player.skillExp = player.skillExp + increment;

        skillLevel[(int)Skills.IncreaseExp]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.IncreaseExp] >= maxLevel)
        {
            if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxExpSkill].isAchieve)
            {
                AchieveManager.Instance.AchieveMaxExpSkill();
            }
            else
            {
                isMaxSkillLevel[(int)Skills.IncreaseExp] = true;
                maxSkillCount++;
            }
        }
    }

    // 쿨타임 감소
    public void DecreaseCoolTime(int maxLevel, float decrement)
    {
        player.coolTimeDecrement = player.coolTimeDecrement + decrement;

        skillLevel[(int)Skills.DecreaseCoolTime]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.DecreaseCoolTime] >= maxLevel)
        {
            if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxCoolTime].isAchieve)
            {
                AchieveManager.Instance.AchieveMaxCoolTime();
            }
            else
            {
                isMaxSkillLevel[(int)Skills.DecreaseCoolTime] = true;
                maxSkillCount++;
            }
        }
    }

    // 점프력 증가
    public void IncreaseJump(int maxLevel, float increment)
    {
        player.jumpPower = player.jumpPower + increment;

        skillLevel[(int)Skills.IncreaseJump]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.IncreaseJump] >= maxLevel)
        {
            if (!AchieveManager.Instance.achieveList[(int)AchieveManager.Achieve.MaxJump].isAchieve)
            {
                AchieveManager.Instance.AchieveMaxJump();
            }
            else
            {
                isMaxSkillLevel[(int)Skills.IncreaseJump] = true;
                maxSkillCount++;
            }
        }
    }

    // Hp 회복
    public void Heal(int increment)
    {
        player.hp = player.hp + increment;
        if (player.hp > player.maxHp)
            player.hp = player.maxHp;
        GameManager.Instance.HpImageUpdate();
    }

    // 엑티브 스킬 얻는 함수
    public void GetActiveSkill(Skills skill)
    {
        int buttonIndex = 0;
        if (skill >= Skills.PAWN)
            buttonIndex = transformSkillIndex;
        else
            buttonIndex = ActSkillIndex;

        GameManager.Instance.ActiveSkillButtonActive(buttonIndex, (int)skill); // UI 설정
        actSkillButtonNumber[buttonIndex] = skill; // 버튼에 어떤 스킬이 연결되어 있는지 설정

        if (skill == Skills.DoubleJump)
        {
            GameManager.Instance.activeSkillButtons[buttonIndex].interactable = false;
            player.isDoubleJump = true;
        }
        if(skill == Skills.Teleport)
            teleportButtonIndex = buttonIndex;

        if (skill >= Skills.PAWN)
            transformSkillIndex++;
        else
            ActSkillIndex++;
    }

    // 액티브 스킬 and 변신 버튼 클릭 이벤트
    public void ClickActSkillButton(int buttonIndex)
    {
        float cooltime = ActiveSkill(actSkillButtonNumber[buttonIndex]);
        StartCoroutine(SkillCoolDown(buttonIndex, cooltime));
    }

    public float ActiveSkill(Skills skill)
    {
        float cooltime = 5f;
        switch (skill)
        {
            case Skills.ExpBall:
                SkillExpBall(5, 5f);
                cooltime = 5f;
                break;
            case Skills.RandomAll:
                SkillRandomAll(Skills.ExpBall, Skills.Wall);
                cooltime = 5f;
                break;
            case Skills.Clone:
                SkillClone();
                cooltime = 5f;
                break;
            case Skills.Bomb:
                SkillBomb();
                cooltime = 0.5f;
                break;
            case Skills.Dash:
                SkillDash(100000f, 1500f);
                break;
            case Skills.SizeDown:
                SkillSizeDown(0.5f, 5f);
                cooltime = 5f;
                break;
            case Skills.DoubleJump:
                // 구현 완료
                break;
            case Skills.SkillHeal:
                Heal(1);
                cooltime = 5f;
                break;
            case Skills.Teleport:
                SkillTeleport(4f);
                if(isTeleport)
                    return 0;
                cooltime = 5f;
                teleportCoolTime = cooltime;
                break;
            case Skills.Shield:
                SkillShield(1);
                break;
            case Skills.InvincibilitySkill:
                SkillInvincibility();
                cooltime = 5f;
                break;
            case Skills.Wall:
                SkillWall(5f);
                cooltime = 5f;
                break;
            case Skills.PAWN:
                ChangeType(PlayerControl.State.PAWN);
                cooltime = 5f;
                break;
            case Skills.KNIGHT:
                ChangeType(PlayerControl.State.KNIGHT);
                cooltime = 5f;
                break;
            case Skills.BISHOP:
                ChangeType(PlayerControl.State.BISHOP);
                cooltime = 5f;
                break;
            case Skills.ROOK:
                ChangeType(PlayerControl.State.ROOK);
                cooltime = 5f;
                break;
            case Skills.KING:
                ChangeType(PlayerControl.State.KING);
                cooltime = 5f;
                break;
        }

        return cooltime * (1 - player.coolTimeDecrement / 100);
    }

    // 경험치 구슬 스킬
    public void SkillExpBall(int count, float time)
    {
        ExpBall expBall;
        for (int i = 0; i<count; i++)
        {
            expBall = ObjectPool.Instance.GetObject(3).GetComponent<ExpBall>();
            StartCoroutine(expBall.ReturnObstacle(time, 3));
        }
    }

    // 필드 중 랜덤 위치 생성
    public Vector3 GetRandomPosition()
    {
        Vector3 start = transform.TransformDirection(sPos.position);
        Vector3 end = transform.TransformDirection(ePos.position);

        // 셀 범위 내에서 랜덤한 위치에 공이 생성되고 떨어짐
        float x = Random.Range(start.x - 5, end.x + 5);
        float z = Random.Range(start.z - 5, end.z + 5);

        int posX = (int)x / 5 * 5;
        int posZ = (int)z / 5 * 5;

        return new Vector3(posX, HEIGHT, posZ);
    }

    // ??? 스킬
    public void SkillRandomAll(Skills start, Skills end)
    {
        int random = 0;
        do
        {
            random = Random.Range((int)start, (int)end + 1);
        } while (random == (int)Skills.RandomAll);
        debugText2.text = "" + (Skills)random;
        ActiveSkill((Skills)random);
    }

    // 분신 소환 스킬
    public void SkillClone()
    {
        clone.transform.position = player.transform.position;
        clone.SetActive(true);
    }

    // 탄막 터뜨리기
    public void SkillBomb()
    {
        player.BombEffect.transform.position = player.transform.position;
        player.BombEffect.gameObject.SetActive(true);
        player.BombEffect.Play();
    }

    // 대쉬 스킬
    public void SkillDash(float xPower, float yPower)
    {
        player.rigid.AddForce(player.transform.forward * xPower);
        player.rigid.AddForce(player.transform.up * yPower);
    }

    // 소소화
    public void SkillSizeDown(float increment, float duration)
    {
        float x = player.gameObject.transform.localScale.x;
        float y = player.gameObject.transform.localScale.y;
        float z = player.gameObject.transform.localScale.z;
        Vector3 origin = new Vector3(x, y, z);

        StartCoroutine(DurationSizeDown(origin, increment, duration));
    }

    // 소소화 지속 시간
    public IEnumerator DurationSizeDown(Vector3 origin, float increment, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            if (time < 1f)
                player.transform.localScale = Vector3.Lerp(origin * increment, origin, Time.fixedDeltaTime);
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        player.gameObject.transform.localScale = origin;
    }

    // 텔레포트 스킬
    public void SkillTeleport(float time)
    {
        if (isTeleport)
        {
            TeleportEnd();
            return;
        }

        Renderer mesh = player.GetComponent<MeshRenderer>();
        Color color = new Color(195f / 255f, 202f / 255f, 219f / 255f , 0f);
        mesh.material.color = color;

        Button[] button = GameManager.Instance.activeSkillButtons;
        for(int i=0;i< button.Length; i++)
        {
            if(i != teleportButtonIndex)
                button[i].interactable = false;
        }

        player.gameObject.layer = 9;
        player.rigid.isKinematic = true;
        teleportPoint.SetActive(true);
        isTeleport = true;
        Invoke("TeleportEnd", time);
    }

    public void TeleportEnd()
    {
        Renderer mesh = player.GetComponent<MeshRenderer>();
        Color color = new Color(195f / 255f, 202f / 255f, 219f / 255f, 255f);
        mesh.material.color = color;

        Button[] button = GameManager.Instance.activeSkillButtons;
        for (int i = 0; i < button.Length; i++)
        {
            if (i != teleportButtonIndex)
                button[i].interactable = true;
        }

        player.gameObject.layer = 10;
        player.rigid.isKinematic = false;
        player.transform.position = teleportPoint.transform.position;
        isTeleport = false;
        teleportPoint.SetActive(false);

        if (IsInvoking("TeleportEnd"))
        {
            CancelInvoke("TeleportEnd");
            return;
        }

        isTeleportInvokeEnd = true;
    }
    // 쉴드 스킬
    public void SkillShield(int maxCount)
    {
        player.shieldCount++;
        if (player.shieldCount > maxCount)
            player.shieldCount = maxCount;
        Renderer mesh = player.GetComponentInChildren<MeshRenderer>();
        mesh.material.color = Color.yellow;
    }

    // 무적 스킬
    public void SkillInvincibility()
    {
        player.isSkillInvincibility = true;
        StartCoroutine(player.Invincibility(player.skillInvincibilityTime));
    }

    // 벽 세우기 스킬
    public void SkillWall(float time)
    {
        float directX = player.transform.forward.x;
        float directZ = player.transform.forward.z; 
        int directIntX = (int)(directX * 10);
        int directIntZ = (int)(directZ * 10);

        int posX = (int)player.transform.position.x / 5 * 5;
        int posZ = (int)player.transform.position.z / 5 * 5;


        debugText.text = "" + directIntX;
        debugText.text += "\n" + directIntZ;


        if (-7 <= directIntX && directIntX <= 7)
        {
            if(directIntZ >= 6)
                posZ += 10;
            else if(directIntZ <= -6)
                posZ -= 10;

            wall.transform.eulerAngles = new Vector3(0, 90f, 0);
        }
        else if(-7 <= directIntZ && directIntZ <= 7)
        {
            if (directIntX >= 6)
                posX += 10;
            else if(directIntX <= -6)
                posX -= 10;

            wall.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        wall.transform.position = new Vector3(posX, sPos.position.y + 5f, posZ);
        wall.SetActive(true);
        Invoke("RemoveWall", time);
    }

    // 벽 없애기(Invoke 용)
    public void RemoveWall()
    {
        wall.SetActive(false);
    }

    //변신
    public void ChangeType(PlayerControl.State state)
    {
        int index = (int)state;

        Mesh model = types[index].mesh;
        if (model != null)
        {
            MeshFilter pType = player.GetComponent<MeshFilter>();
            pType.sharedMesh = model;

            MeshCollider pCollider = player.GetComponent<MeshCollider>();
            pCollider.sharedMesh = model;

            player.state = (PlayerControl.State)index;
            player.changeEffect.transform.position = player.transform.position;
            player.changeEffect.Play();
        }
    }

    // 스킬 쿨타임
    public IEnumerator SkillCoolDown(int index, float maxCoolTime)
    {
        GameManager.Instance.activeSkillButtons[index].interactable = false;

        float coolTime = maxCoolTime;
        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            if (coolTime < 0f)
                coolTime = 0f;

            GameManager.Instance.ShowLeftCoolTime(index, coolTime, maxCoolTime);
            yield return new WaitForFixedUpdate();
        }

        GameManager.Instance.activeSkillButtons[index].interactable = true;
    }

}
