﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : Singleton<SkillManager>
{
    public Text debugText;
    // 스킬의 Index
    public enum Skills { IncreaseMaxHp, IncreaseInvincibilityTime, IncreaseSpeed, IncreaseExp, Heal, ExpBall, RandomAll, Clone, Bomb, Dash, SizeDown, DoubleJump, SkillHeal, Teleport, Shield, InvincibilitySkill, Wall, PAWN, KNIGHT, BISHOP, ROOK, KING };

    public List<string> skillNames; // 스킬 이름을 담는 List
    public List<string> skill_Info; // 스킬의 정보를 담는 List
    public List<Sprite> skillSprites; // 스킬의 이미지를 담는 List

    public Skills[] actSkillButtonNumber; // 엑티브 스킬 버튼 i번째에 담긴 스킬 정보
    public int ActSkillIndex; // 엑티브 스킬 버튼 인덱스
    public int transformSkillIndex; // 엑티브 스킬 버튼 인덱스

    public bool[] isGetTransformSkill; // 각 스킬들의 상태가 Max인지 체크하기 위한 배열
    public bool[] isMaxSkillLevel; // 각 스킬들의 상태가 Max인지 체크하기 위한 배열
    public int[] skillLevel; // 각 스킬들 레벨을 체크하는 배열
    public int totalSkillsCount; // 이 게임에 존재하는 Skill 개수 변수
    public int maxSkillCount; // Max가 된 스킬들이 몇개인지 체크하기 위한 변수

    // 각 패시브 스킬 증가량
    public int HpIncrement;
    public int HealIncrement;
    public float speedIncrement;
    public float skillExpIncrement;
    public float onHitInvincibilityTimeIncrement;
    public float downSizeIncrement;

    // 각 패시브 스킬 최대값
    public int maxHpLevel;
    public int maxSpeedLevel;
    public int maxSkillExpLevel;
    public int maxOnHitInvincibilityTimeLevel;
    public int maxDownSizeLevel;

    [SerializeField] PlayerControl player; // player의 정보를 가지고 오기 위한 변수
    [SerializeField] MeshFilter[] types;
    [SerializeField] ParticleSystem changeEffect;

    void Start()
    {
        InitSkill();
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

        GetActiveSkill(Skills.SizeDown);

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 스킬 기본 설정, 여기서 설정 하면 됨
    public void InitSkill()
    {
        SetSkillsInfo("Hp 증가", "MaxHp를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetSkillsInfo("피격무적 시간 증가", "피격무격 시간을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/InvincibleTimeUpOnHit"));
        SetSkillsInfo("Speed 증가", "Speed를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/SpeedUp"));
        SetSkillsInfo("경험치 증가", "경험치 획득량을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/ExpUp"));
        SetSkillsInfo("Hp 회복", "Hp를 2 회복시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/Heal"));

        SetSkillsInfo("ExpBall", "ExpBall", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("RandomAll", "RandomAll", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("Clone", "Clone", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("Bomb", "Bomb", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("Dash", "Dash", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));

        SetSkillsInfo("SizeDown", "SizeDown", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("DoubleJump", "DoubleJump", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("SkillHeal", "SkillHeal", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
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
        downSizeIncrement = 0.8f;


        // 각 패시브 스킬 최대값 초기화
        maxHpLevel = 8;
        maxSpeedLevel = 5;
        maxOnHitInvincibilityTimeLevel = 5;
        maxSkillExpLevel = 5;
        maxDownSizeLevel = 5;
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
        if (totalSkillsCount - maxSkillCount == 1)
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
            while (randomNum.Contains(temp) || isMaxSkillLevel[temp])
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
            isMaxSkillLevel[(int)Skills.IncreaseInvincibilityTime] = true;
            maxSkillCount++;
        }
    }

    // 스피드 증가
    public void IncreaseSpeed(int maxLevel, float increment)
    {
        player.speed = player.speed + speedIncrement;

        skillLevel[(int)Skills.IncreaseSpeed]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.IncreaseSpeed] >= maxLevel)
        {
            isMaxSkillLevel[(int)Skills.IncreaseSpeed] = true;
            maxSkillCount++;
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
            isMaxSkillLevel[(int)Skills.IncreaseExp] = true;
            maxSkillCount++;
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
                break;
            case Skills.RandomAll:
                SkillRandomAll(Skills.ExpBall, Skills.Wall);
                break;
            case Skills.Clone:
                break;
            case Skills.Bomb:
                break;
            case Skills.Dash:
                SkillDash(100000f);
                break;
            case Skills.SizeDown:
                SkillSizeDown(0.5f, 5f);
                cooltime = 5f;
                break;
            case Skills.DoubleJump:
                break;
            case Skills.SkillHeal:
                break;
            case Skills.Teleport:
                break;
            case Skills.Shield:
                break;
            case Skills.InvincibilitySkill:
                SkillInvincibility();
                cooltime = 5f;
                break;
            case Skills.Wall:
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

        return cooltime;
    }

    public void SkillDash(float power)
    {
        player.rigid.AddForce(player.transform.up * 2000f);
        player.rigid.AddForce(player.transform.forward * power);
    }


    // ??? 스킬
    public void SkillRandomAll(Skills start, Skills end)
    {
        int random = 0;
        do
        {
            random = Random.Range((int)start, (int)end + 1);
        } while (random == (int)Skills.RandomAll);

        ActiveSkill((Skills)random);
    }
    public void SkillSizeDown(float increment, float duration)
    {
        float x = player.gameObject.transform.localScale.x;
        float y = player.gameObject.transform.localScale.y;
        float z = player.gameObject.transform.localScale.z;
        Vector3 origin = new Vector3(x, y, z);

        player.gameObject.transform.localScale = origin * increment;
        StartCoroutine(DurationSizeDown(duration, origin));
    }
    public IEnumerator DurationSizeDown(float duration, Vector3 origin)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        player.gameObject.transform.localScale = origin;
    }


    // 무적 스킬
    public void SkillInvincibility()
    {
        player.isSkillInvincibility = true;
        StartCoroutine(player.Invincibility(player.skillInvincibilityTime));
    }

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
            changeEffect.transform.position = player.transform.position;
            changeEffect.Play();
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
