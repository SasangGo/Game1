using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class DataManager : Singleton<DataManager>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }

    public void SaveAchieve()
    {
        List<Achievement> AchieveList = AchieveManager.Instance.achieveList;
        bool[] isAchieve = new bool[AchieveList.Count];
        for(int i = 0; i < AchieveList.Count; i++)
        {
            isAchieve[i] = AchieveList[i].isAchieve;
        }

        SaveData data = new SaveData(isAchieve);
        CreateJsonFile(data, Application.dataPath, "AchievementList");
    }

    public void LoadAchieve(string loadPath, string fileName)
    {
        List<Achievement> AchieveList = AchieveManager.Instance.achieveList;
        SaveData data = LoadJsonFile<SaveData>(loadPath, fileName);
        if (data == null)
        {
            for (int i = 0; i < AchieveList.Count; i++)
            {
                AchieveList[i].isAchieve = false;
            }
        }
        else
        {
            for (int i = 0; i < AchieveList.Count; i++)
            {
                AchieveList[i].isAchieve = data.isAchieve[i];
            }
        }
    }

    public void CreateJsonFile(object obj, string createPath, string fileName)
    {
        string jsonData = JsonUtility.ToJson(obj, true);
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileInfo fileInfo = new FileInfo(string.Format("{0}/{1}.json", loadPath, fileName));
        if (!fileInfo.Exists)
        {
            CreateJsonFile(null, loadPath, fileName);
        }

        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);

        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }
}

[System.Serializable]
public class SaveData
{
    public bool[] isAchieve;

    public SaveData(bool[] isAchieve)
    {
        this.isAchieve = isAchieve;
    }
}