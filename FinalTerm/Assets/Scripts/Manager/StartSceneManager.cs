using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 시작 화면 관리 매니저
public class StartSceneManager : Singleton<StartSceneManager>
{
    private const int MAINSCENENUMBER = 1;

    [SerializeField] GameObject achievePanel;
    [SerializeField] GameObject exitPanel;
    [SerializeField] public GameObject[] achieveItem;

    private void Start()
    {
    }


    private void Update()
    {
        // 뒤로가기 버튼 이벤트
        if (Input.GetKeyDown(KeyCode.Escape))
            // recordPanel이 활성화 된 상태인 경우 -> 비활성화
            if (achievePanel.activeSelf)
                achievePanel.SetActive(false);
            // recordPanel이 비활성화 된 상태인 경우 -> exitPanel 활성 or 비활성화
            else
                if (!exitPanel.activeSelf)
                exitPanel.SetActive(true);
            else
                exitPanel.SetActive(false);

    }

    // StartButton 이벤트 -> 메인 화면으로 전환
    public void StartButton()
    {
        SceneManager.LoadScene(MAINSCENENUMBER);
    }

    // AchieveButton 이벤트 -> 업적 패널 활성화
    public void AchieveButton()
    { 
        for(int i = 0; i < AchieveManager.Instance.achieveList.Count; i++)
            SetAchieveItem(AchieveManager.Instance.achieveList[i]);

        if (achievePanel != null)
            achievePanel.SetActive(true);
    }

    // CancelButton 이벤트 -> 업적 패널 비활성화
    public void CancelButton()
    {
        if (achievePanel != null)
            achievePanel.SetActive(false);
    }

    // ExitPanel 안의 Yes, no 버튼 이벤트
    public void ExitPanelButtons(bool isYes)
    {
        if (isYes == true)
            Application.Quit();
        else
            exitPanel.SetActive(false);
    }

    public void SetAchieveItem(Achievement achieve)
    {
        Text[] texts = achieveItem[achieve.index].GetComponentsInChildren<Text>();
        Image[] image = achieveItem[achieve.index].GetComponentsInChildren<Image>();

        texts[0].text = achieve.achieveTitle;
        if (achieve.isAchieve)
        {
            texts[1].text = achieve.achieveExplane;
            texts[2].text = "효과 : " + achieve.achieveEffect;
            image[3].sprite = AchieveManager.Instance.achieveImageList[achieve.index];
        }
        else
        {
            texts[1].text = "???";
            texts[2].text = "효과 : ???";
            image[3].sprite = AchieveManager.Instance.noAchieveSprite;
        }
    }

}
