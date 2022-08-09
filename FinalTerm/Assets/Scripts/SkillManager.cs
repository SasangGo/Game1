using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{

    // 스킬의 Index
    public enum Skills { IncreaseMaxHp, Heal, IncreaseInvincibilityTime, IncreaseSpeed, SizeDown, InvincibilitySkill, IncreaseExp };

    public List<string> skillNames; // 스킬 이름을 담는 List
    public List<string> skill_Info; // 스킬의 정보를 담는 List
    public List<Sprite> skillSprites; // 스킬의 이미지를 담는 List

    public bool[] isMaxSkillLevel; // 각 스킬들의 상태가 Max인지 체크하기 위한 배열
    public int[] skillLevel;
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

    void Start()
    {
        InitSkill();
        totalSkillsCount = skillNames.Count;

        maxSkillCount = 0;
        isMaxSkillLevel = new bool[totalSkillsCount];
        skillLevel = new int[totalSkillsCount];
        for (int i = 0; i < totalSkillsCount; i++)
        {
            isMaxSkillLevel[i] = false;
            skillLevel[i] = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 스킬 기본 설정, 여기서 설정 하면 됨
    public void InitSkill()
    {
        SetSkillsInfo("Hp 증가", "MaxHp를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/1"));
        SetSkillsInfo("Hp 회복", "Hp를 2 회복시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/2"));
        SetSkillsInfo("피격무적 시간 증가", "피격무격 시간을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/3"));
        SetSkillsInfo("Speed 증가", "Speed를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/4"));
        SetSkillsInfo("Size 감소", "Size를 감소시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/5"));
        SetSkillsInfo("무적 스킬", "사용시 무적이 된다", Resources.Load<Sprite>("Sprites/SkillSprites/6"));
        SetSkillsInfo("경험치 증가", "경험치 획득량을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/7"));

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
            // Hp 회복 스킬
            case Skills.Heal:
                Heal(HealIncrement);
                break;
            // 피격 무적 시간 증가 스킬
            case Skills.IncreaseInvincibilityTime:
                IncreaseOnHitInvincibilityTime(maxOnHitInvincibilityTimeLevel, onHitInvincibilityTimeIncrement);
                break;
            // 스피드 증가 스킬
            case Skills.IncreaseSpeed:
                IncreaseSpeed(maxSpeedLevel, speedIncrement);
                break;
            // 플레이어의 크기를 줄이는 스킬
            case Skills.SizeDown:
                SizeDown(maxDownSizeLevel, downSizeIncrement);
                break;
            // 엑티브 무적 스킬 획득
            case Skills.InvincibilitySkill:
                GetInvincibilitySkill();
                break;
            // 경험치 획득량 증가 스킬
            case Skills.IncreaseExp:
                IncreaseSkillExp(maxSkillExpLevel, skillExpIncrement);
                break;

        }
    }

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

    public void Heal(int increment)
    {
        player.hp = player.hp + increment;
        if (player.hp > player.maxHp)
            player.hp = player.maxHp;
        GameManager.Instance.HpImageUpdate();
    }

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

    public void SizeDown(int maxLevel, float increment)
    {
        Vector3 temp = player.gameObject.transform.localScale;
        temp = temp * increment;
        player.gameObject.transform.localScale = temp;

        skillLevel[(int)Skills.SizeDown]++;
        // 스킬이 max치가 되면
        if (skillLevel[(int)Skills.SizeDown] >= maxLevel)
        {
            isMaxSkillLevel[(int)Skills.SizeDown] = true;
            maxSkillCount++;
        }
    }

    public void GetInvincibilitySkill()
    {
        if (isMaxSkillLevel[(int)Skills.InvincibilitySkill])
            return;

        GameManager.Instance.ActiveSkillButtonActive(0, (int)Skills.InvincibilitySkill);
        skillLevel[(int)Skills.InvincibilitySkill]++;

        isMaxSkillLevel[(int)Skills.InvincibilitySkill] = true;
        maxSkillCount++;
    }

    public void InvincibilitySkill()
    {
        // StartCoroutine(player.Invincibility(player.skillInvincibilityTime));
        GameManager.Instance.activeSkillButtons[0].interactable = false;
        StartCoroutine(ActtiveSkillCoolDown(0, 5f));
        GameManager.Instance.activeSkillButtons[0].interactable = true;
    }

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

    public IEnumerator ActtiveSkillCoolDown(int index, float maxCoolTime)
    {
        float coolTime = maxCoolTime;
        while (coolTime > 0f)
        {
            GameManager.Instance.ShowLeftCoolTime(index, coolTime, maxCoolTime);
            coolTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //GameManager.Instance.ShowLeftCoolTime(index, 0f, maxCoolTime);
    }
}
