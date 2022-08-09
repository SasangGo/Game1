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
    public int totalSkillsCount; // 이 게임에 존재하는 Skill 개수 변수
    public int maxSkillCount; // Max가 된 스킬들이 몇개인지 체크하기 위한 변수

    [SerializeField] PlayerControl player; // player의 정보를 가지고 오기 위한 변수

    void Start()
    {
        InitSkill();
        totalSkillsCount = skillNames.Count;

        maxSkillCount = 0;
        isMaxSkillLevel = new bool[totalSkillsCount];
        for (int i = 0; i < totalSkillsCount; i++)
            isMaxSkillLevel[i] = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 스킬 기본 설정, 여기서 설정 하면 됨
    public void InitSkill()
    {
        SetSkillsInfo("Hp 증가", "MaxHp를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetSkillsInfo("Hp 회복", "Hp를 2 회복시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/Heal"));
        SetSkillsInfo("피격무적 시간 증가", "피격무격 시간을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetSkillsInfo("Speed 증가", "Speed를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/SpeedUp"));
        SetSkillsInfo("Size 감소", "Size를 감소시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetSkillsInfo("무적 스킬", "사용시 무적이 된다", Resources.Load<Sprite>("Sprites/SkillSprites/6"));
        SetSkillsInfo("경험치 증가", "경험치 획득량을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/7"));
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
                player.maxHp++;
                // 스킬이 max치가 되면
                if (player.maxHp >= player.maxHpIncrement)
                {
                    player.maxHp = 10;

                    isMaxSkillLevel[(int)Skills.IncreaseMaxHp] = true;
                    maxSkillCount++;
                }
                GameManager.Instance.HpImageUpdate();
                break;
            // Hp 회복 스킬
            case Skills.Heal:
                player.hp = player.hp + 2;
                if (player.hp > player.maxHp)
                    player.hp = player.maxHp;
                GameManager.Instance.HpImageUpdate();
                break;
            // 피격 무적 시간 증가 스킬
            case Skills.IncreaseInvincibilityTime:
                player.onHitInvincibilityTime = player.onHitInvincibilityTime + 1f;
                // 스킬이 max치가 되면
                if (player.onHitInvincibilityTime >= player.maxOnHitInvincibilityTimeIncrement)
                {
                    player.onHitInvincibilityTime = player.maxOnHitInvincibilityTimeIncrement;

                    isMaxSkillLevel[(int)Skills.IncreaseInvincibilityTime] = true;
                    maxSkillCount++;
                }
                break;
            // 스피드 증가 스킬
            case Skills.IncreaseSpeed:
                player.speed = player.speed + 2f;
                // 스킬이 max치가 되면
                if (player.speed >= player.maxSpeedIncrement)
                {
                    player.speed = player.maxSpeedIncrement;

                    isMaxSkillLevel[(int)Skills.IncreaseSpeed] = true;
                    maxSkillCount++;
                }
                break;
            // 플레이어의 크기를 줄이는 스킬
            case Skills.SizeDown:
                Vector3 temp = player.gameObject.transform.localScale;
                temp = temp * player.downSize;
                // 스킬이 max치가 되면
                if (temp.x <= player.maxDownSizeIncrement)
                {
                    temp = new Vector3(player.maxDownSizeIncrement, player.maxDownSizeIncrement, player.maxDownSizeIncrement);

                    isMaxSkillLevel[(int)Skills.SizeDown] = true;
                    maxSkillCount++;
                }
                player.gameObject.transform.localScale = temp;
                break;
            // 엑티브 무적 스킬 획득
            case Skills.InvincibilitySkill:
                GameManager.Instance.ActiveSkillButtonActive(0, (int)Skills.InvincibilitySkill);

                isMaxSkillLevel[(int)Skills.InvincibilitySkill] = true;
                maxSkillCount++;
                break;
            // 경험치 획득량 증가 스킬
            case Skills.IncreaseExp:
                player.skillExp = player.skillExp + 1f;
                // 스킬이 max치가 되면
                if (player.skillExp >= player.maxSkillExpIncrement)
                {
                    player.skillExp = player.maxSkillExpIncrement;

                    isMaxSkillLevel[(int)Skills.IncreaseExp] = true;
                    maxSkillCount++;
                }
                break;

        }
    }
}
