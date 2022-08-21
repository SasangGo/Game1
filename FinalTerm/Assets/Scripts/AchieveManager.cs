using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

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

        string jsonData = ObjectToJson(AchieveList[0]);
        string jsonData2 = ObjectToJson(AchieveList[1]);
        string jsonData3 = ObjectToJson(AchieveList[2]);
        SkillManager.Instance.debugText.text = jsonData;

        CreateJsonFile(Application.dataPath, "AchievementList", jsonData + jsonData2 + jsonData3);

    }

    public void InitAchievements()
    {
        SetAchievements(new Achievement("내가 만렙..?", "플레이어 최대 레벨 달성 할 때 주는 업적", "플레이어 최대 레벨이 10 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement("아프니까 청춘이다!", "피격무적 시간 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "피격무적 시간 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetAchievements(new Achievement("그건 제.. 잔상입니다만?", "스피드 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "스피드 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetAchievements(new Achievement("머리만 컸어...", "획득 경험치 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "획득 경험치 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/HealthUp"));
        SetAchievements(new Achievement("아이오니아 장화", "쿨타임 감소 스킬 최대 레벨을 달성 할 때 주는 업적", "쿨타임 감소 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
        SetAchievements(new Achievement("이걸 찍네", "점프력 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "점프력 증가 스킬 최대 레벨이 1 상승한다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/Invincibility"));
        SetAchievements(new Achievement("바위처럼 단단하게", "최대 체력 증가 스킬 최대 레벨을 달성 할 때 주는 업적", "레벨 업 스킬 선택지에 \'회복\' 선택지가 추가된다.", false), Resources.Load<Sprite>("Sprites/SkillSprites/SizeDown"));
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
    public void AchieveMaxOnHit()
    {
        AchieveList[1].isAchieve = true;
        SkillManager.Instance.maxOnHitInvincibilityTimeLevel += 1;
    }
    public void AchieveMaxSpeed()
    {
        AchieveList[2].isAchieve = true;
        SkillManager.Instance.maxSpeedLevel += 1;
    }
    public void AchieveMaxExpSkill()
    {
        AchieveList[3].isAchieve = true;
        SkillManager.Instance.maxSkillExpLevel += 1;
    }
    public void AchieveMaxCoolTime()
    {
        AchieveList[4].isAchieve = true;
        SkillManager.Instance.maxCoolTimeLevel += 1;
    }
    public void AchieveMaxJump()
    {
        AchieveList[5].isAchieve = true;
        SkillManager.Instance.maxJumpLevel += 1;
    }
    public void AchieveMaxHp()
    {
        AchieveList[6].isAchieve = true;
        SkillManager.Instance.maxHpLevel += 1;
    }



    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj, true);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }
}

[System.Serializable]
public class Achievement
{
    public string achieveTitle;
    public string achieveExplane;
    public string achieveEffect;
    public bool isAchieve;

    public Achievement(string title, string explane, string effect, bool isAchieve)
    {
        this.achieveTitle = title;
        this.achieveExplane = explane;
        this.achieveEffect = effect;
        this.isAchieve = isAchieve;
    }
}
