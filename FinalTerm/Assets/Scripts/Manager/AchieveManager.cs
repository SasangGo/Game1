using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager : Singleton<AchieveManager>
{
    public enum Achieve { MaxLevel, MaxOnHit, MaxSpeed, MaxExpSkill, MaxCoolTime, MaxJump, MaxHp, FirstClear, NoFalling, NoStat, NoTransform, Highest, NoHit, NoHitNight, NoHitBishop, NoHitRook, AllHit, Revive, FiveClear }

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
        SetAchievements(new Achievement((int)Achieve.FirstClear, "시작이 반이다", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.NoFalling, "지구는 둥글다", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.NoStat, "변태...?", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        
        SetAchievements(new Achievement((int)Achieve.NoTransform, "튜닝의 끝은 순정이다", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.Highest, "정상에 선 자", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.NoHit, "쉽네 쉬워", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.NoHitNight, "쉽네 쉬워(나이트)", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.NoHitBishop, "쉽네 쉬워(비숍)", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        
        SetAchievements(new Achievement((int)Achieve.NoHitRook, "쉽네 쉬워(룩)", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.AllHit, "피했다고!(못피함)", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.Revive, "지옥에서 돌아온자", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement((int)Achieve.FiveClear, "이걸 5번이나 했다고?", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
    }

    public void SetAchievements(Achievement achievement, Sprite sprite)
    {
        achieveList.Add(achievement);
        achieveImageList.Add(sprite);
    }
    // 캐릭터 만렙 달성, 만렙 +10
    public void AchieveMaxLevel()
    {
        achieveList[(int)Achieve.MaxLevel].isAchieve = true;
        SkillManager.Instance.maxLevel += 10;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxLevel]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxLevel]);
        DataManager.Instance.SaveAchieve();
    }
    // 피격무적 패시브 레벨 만렙, 피격 무적 시간 만렙 +1
    public void AchieveMaxOnHit()
    {
        achieveList[(int)Achieve.MaxOnHit].isAchieve = true;
        SkillManager.Instance.maxOnHitInvincibilityTimeLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxOnHit]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxOnHit]);
        DataManager.Instance.SaveAchieve();
    }
    // 스피드 패시브 레벨 만렙, 스피드 만렙 +1
    public void AchieveMaxSpeed()
    {
        achieveList[(int)Achieve.MaxSpeed].isAchieve = true;
        SkillManager.Instance.maxSpeedLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxSpeed]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxSpeed]);
        DataManager.Instance.SaveAchieve();
    }
    // 경험치 획득량 패시브 레벨 만렙, 경험치 획득량 만렙 +1
    public void AchieveMaxExpSkill()
    {
        achieveList[(int)Achieve.MaxExpSkill].isAchieve = true;
        SkillManager.Instance.maxSkillExpLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxExpSkill]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxExpSkill]);
        DataManager.Instance.SaveAchieve();
    }
    // 쿨타임 감소 패시브 레벨 만렙, 쿨타임 감소 만렙 +1
    public void AchieveMaxCoolTime()
    {
        achieveList[(int)Achieve.MaxCoolTime].isAchieve = true;
        SkillManager.Instance.maxCoolTimeLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxCoolTime]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxCoolTime]);
        DataManager.Instance.SaveAchieve();
    }
    // 점프력 패시브 레벨 만렙, 점프력 만렙 +1
    public void AchieveMaxJump()
    {
        achieveList[(int)Achieve.MaxJump].isAchieve = true;
        SkillManager.Instance.maxJumpLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxJump]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxJump]);
        DataManager.Instance.SaveAchieve();
    }
    // 체력 최대치 증가 레벨 만렙, 체력 최대치 증가 만렙 +1
    public void AchieveMaxHp()
    {
        achieveList[(int)Achieve.MaxHp].isAchieve = true;
        SkillManager.Instance.maxHpLevel += 1;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.MaxHp]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.MaxHp]);
        DataManager.Instance.SaveAchieve();
    }
    // 게임 클리어 1회, 경험치 구슬 스킬
    public void AchieveFirstClear()
    {
        achieveList[(int)Achieve.FirstClear].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.FirstClear]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.FirstClear]);
        DataManager.Instance.SaveAchieve();
    }
    // 한 번도 떨어지지 않고 게임 클리어, 2단 점프
    public void AchieveNoFalling()
    {
        achieveList[(int)Achieve.NoFalling].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoFalling]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoFalling]);
        DataManager.Instance.SaveAchieve();
    }
    // 스탯을 찍지 않고 게임 클리어, 게임 시작 시 스킬 하나 주고 시작
    public void AchieveNoStat()
    {
        achieveList[(int)Achieve.NoStat].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoStat]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoStat]);
        DataManager.Instance.SaveAchieve();
    }
    // 변신 하지 않고 게임 클리어, ??? 스킬
    public void AchieveNoTransform()
    {
        achieveList[(int)Achieve.NoTransform].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoTransform]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoTransform]);
        DataManager.Instance.SaveAchieve();
    }
    // 정상에 선 자(일정 높이 이상 올라가면 주는 업적), 2단 점프
    public void AchieveHighest()
    {
        achieveList[(int)Achieve.Highest].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.Highest]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.Highest]);
        DataManager.Instance.SaveAchieve();
    }
    //  한번도 맞지 않고 클리어, 명예의 전당(구글 리더보드)
    public void AchieveNoHit()
    {
        achieveList[(int)Achieve.NoHit].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoHit]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoHit]);
        DataManager.Instance.SaveAchieve();
    }
    // 나이트 한대도 안맞고 클리어, 소소화 
    public void AchieveNoHitNight()
    {
        achieveList[(int)Achieve.NoHitNight].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoHitNight]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoHitNight]);
        DataManager.Instance.SaveAchieve();
    }
    // 비숍 한대도 안맞고 클리어, 텔포
    public void AchieveNoHitBishop()
    {
        achieveList[(int)Achieve.NoHitBishop].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoHitBishop]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoHitBishop]);
        DataManager.Instance.SaveAchieve();
    }
    // 룩 한대도 안맞고 클리어, 벽세우기
    public void AchieveNoHitRook()
    {
        achieveList[(int)Achieve.NoHitRook].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.NoHitRook]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.NoHitRook]);
        DataManager.Instance.SaveAchieve();
    }
    // 피했다고!(못피함)(한 게임 내에 모든 패턴 1대 이상 맞음), 체력 +1
    public void AchieveAllHit()
    {
        achieveList[(int)Achieve.AllHit].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.AllHit]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.AllHit]);
        DataManager.Instance.SaveAchieve();
    }
    // 죽음에서 돌아온자(피1에서 맥스까지), 쉴드
    public void AchieveRevive()
    {
        achieveList[(int)Achieve.Revive].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.Revive]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.Revive]);
        DataManager.Instance.SaveAchieve();
    }
    // 이걸 5번이나 했다고?(5회 클리어), 명예의 전당
    public void AchieveFiveClear()
    {
        achieveList[(int)Achieve.FiveClear].isAchieve = true;
        GameManager.Instance.PopUpAchieve(achieveList[(int)Achieve.FiveClear]);
        GameManager.Instance.SetAchieveItem(achieveList[(int)Achieve.FiveClear]);
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
