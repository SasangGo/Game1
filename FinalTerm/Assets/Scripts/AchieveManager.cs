using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager : Singleton<AchieveManager>
{
    public List<Achievement> AchieveList;
    public List<Sprite> AchieveImageList;

    [SerializeField] PlayerControl player;
    private void Start()
    {
        AchieveList = new List<Achievement>();
        AchieveImageList = new List<Sprite>();

        InitAchievements();
        AchieveMaxLevel();

        for (int i = 0; i < AchieveList.Count; i++)
            GameManager.Instance.SetAchieveItem(i, AchieveList[i]);
    }

    public void InitAchievements()
    {
        SetAchievements(new Achievement("내가 만렙..?", "최대 레벨 달성 할 때 주는 업적", "최대 레벨이 10상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement("미구현", "미구현", "미구현", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetAchievements(new Achievement("미구현", "미구현", "미구현", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetAchievements(new Achievement("미구현", "미구현", "미구현", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetAchievements(new Achievement("미구현", "미구현", "미구현", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetAchievements(new Achievement("미구현", "미구현", "미구현", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement("미구현", "미구현", "미구현", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
    }

    public void SetAchievements(Achievement achievement, Sprite sprite)
    {
        AchieveList.Add(achievement);
        AchieveImageList.Add(sprite);
    }
    // 달성 조건 및 달성 기능
    public void AchieveMaxLevel()
    {
        AchieveList[0].isAchieve = true;
        player.maxLevel += 10;
    }   
}
