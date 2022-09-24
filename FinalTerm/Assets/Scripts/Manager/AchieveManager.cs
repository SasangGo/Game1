using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager : Singleton<AchieveManager>
{
    public enum Achieve { MaxLevel, MaxOnHit, MaxSpeed, MaxExpSkill, MaxCoolTime, MaxJump, MaxHp }

    public List<Achievement> achieveList;
    public List<Sprite> achieveImageList;
    public Sprite noAchieveSprite;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        achieveList = new List<Achievement>();
        achieveImageList = new List<Sprite>();

        InitAchievements();

        DataManager.Instance.LoadAchieve(Application.dataPath, "AchievementList");
    }

    public void InitAchievements()
    {
        SetAchievements(new Achievement((int)Achieve.MaxLevel, "내가 만렙..?", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.MaxOnHit, "아프니까 청춘이다!", "피격무적 시간 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "피격무적 시간 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetAchievements(new Achievement((int)Achieve.MaxSpeed, "그건 제.. 잔상입니다만?", "스피드 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "스피드 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetAchievements(new Achievement((int)Achieve.MaxExpSkill,"머리만 컸어...", "획득 경험치 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "획득 경험치 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.MaxCoolTime, "아이오니아 장화", "쿨타임 감소 스킬 최대 레벨을 달성 할 때 주는 업적", "쿨타임 감소 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetAchievements(new Achievement((int)Achieve.MaxJump, "이걸 찍네", "점프력 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "점프력 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetAchievements(new Achievement((int)Achieve.MaxHp, "바위처럼 단단하게", "최대 체력 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "레벨 업 스킬 선택지에 \'회복\' 선택지가 추가된다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
    }

    public void SetAchievements(Achievement achievement, Sprite sprite)
    {
        achieveList.Add(achievement);
        achieveImageList.Add(sprite);
    }
    // 달성 조건 및 달성 기능
    public void AchieveMaxLevel()
    {
        achieveList[(int)Achieve.MaxLevel].isAchieve = true;
        SkillManager.Instance.maxLevel += 10;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxLevel]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxLevel]);
        DataManager.Instance.SaveAchieve();
    }
    public void AchieveMaxOnHit()
    {
        achieveList[(int)Achieve.MaxOnHit].isAchieve = true;
        SkillManager.Instance.maxOnHitInvincibilityTimeLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxOnHit]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxOnHit]);
        DataManager.Instance.SaveAchieve();
    }
    public void AchieveMaxSpeed()
    {
        achieveList[(int)Achieve.MaxSpeed].isAchieve = true;
        SkillManager.Instance.maxSpeedLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxSpeed]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxSpeed]);
        DataManager.Instance.SaveAchieve();
    }
    public void AchieveMaxExpSkill()
    {
        achieveList[(int)Achieve.MaxExpSkill].isAchieve = true;
        SkillManager.Instance.maxSkillExpLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxExpSkill]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxExpSkill]);
        DataManager.Instance.SaveAchieve();
    }
    public void AchieveMaxCoolTime()
    {
        achieveList[(int)Achieve.MaxCoolTime].isAchieve = true;
        SkillManager.Instance.maxCoolTimeLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxCoolTime]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxCoolTime]);
        DataManager.Instance.SaveAchieve();
    }
    public void AchieveMaxJump()
    {
        achieveList[(int)Achieve.MaxJump].isAchieve = true;
        SkillManager.Instance.maxJumpLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxJump]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxJump]);
        DataManager.Instance.SaveAchieve();
    }
    public void AchieveMaxHp()
    {
        achieveList[(int)Achieve.MaxHp].isAchieve = true;
        SkillManager.Instance.maxHpLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxHp]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxHp]);
        DataManager.Instance.SaveAchieve();
    }
}

public class Achievement
{
    public int index;
    public string achieveTitle;
    public string achieveExplane;
    public string achieveEffect;
    public bool isAchieve;

    public Achievement(int index, string title, string explane, string effect, bool isAchieve)
    {
        this.index = index;
        this.achieveTitle = title;
        this.achieveExplane = explane;
        this.achieveEffect = effect;
        this.isAchieve = isAchieve;
    }
}
