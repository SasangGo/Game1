using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{

    // 스킬 Enum 클래스
    public enum Skills { IncreaseMaxHp, Heal, IncreaseArmmor, IncreaseInvincibilityTime, IncreaseSpeed, SizeDown, InvincibilitySkill, IncreaseExp };

    public List<string> skillNames;
    public List<string> skillExplans;
    public List<Sprite> skillSprites;

    private int skillsLength;
    private int skillPanelLength;
    void Start()
    {
        skillsLength = System.Enum.GetValues(typeof(Skills)).Length;
        skillPanelLength = GameManager.Instance.skillPanel.Length;
        InitSkill();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 스킬 기본 설정
    public void InitSkill()
    {
        SetSkillsInfo("Hp 증가", "MaxHp를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/1"));
        SetSkillsInfo("Hp 증가", "Hp를 2 회복시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/1"));
        SetSkillsInfo("Armmor 증가", "Armmor를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/2"));
        SetSkillsInfo("피격무적 시간 증가", "피격무격 시간을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/3"));
        SetSkillsInfo("Speed 증가", "Speed를 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/4"));
        SetSkillsInfo("Size 감소", "Size를 감소시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/5"));
        SetSkillsInfo("무적 스킬", "사용시 무적이 된다", Resources.Load<Sprite>("Sprites/SkillSprites/6"));
        SetSkillsInfo("경험치 증가", "경험치 획득량을 1 증가시킨다", Resources.Load<Sprite>("Sprites/SkillSprites/7"));
    }

    // 스킬 정보 설정 함수
    // SetSkillsInfo(스킬 이름, 스킬 설명, 스킬 이미지)
    public void SetSkillsInfo(string skillName, string skillExplan, Sprite skillSprite)
    {
        skillNames.Add(skillName);
        skillExplans.Add(skillExplan);
        skillSprites.Add(skillSprite);
    }

    // 랜덤 스킬 번호 리스트 생성, 2
    public List<int> RandomSkill()
    {
        List<int> randomNum = new List<int>();

        int temp = Random.Range(0, skillsLength);

        for (int i = 0; i < skillPanelLength; i++)
        {
            while (randomNum.Contains(temp))
                temp = Random.Range(0, skillsLength);

            randomNum.Add(temp);
        }

        return randomNum;
    }

    // 스킬 선택 시 적용되는 함수
    public void SkillChoice(Skills skill)
    {
        GameManager.Instance.ActiveSkillChoicePanel();

        switch (skill)
        {
            case Skills.IncreaseMaxHp:
                break;
            case Skills.Heal:
                break;
            case Skills.IncreaseArmmor:
                break;
            case Skills.IncreaseInvincibilityTime:
                break;
            case Skills.IncreaseSpeed:
                break;
            case Skills.SizeDown:
                break;
            case Skills.InvincibilitySkill:
                break;
            case Skills.IncreaseExp:
                break;

        }

        GameManager.Instance.UnActiveSkillChoicePanel();
    }
}
