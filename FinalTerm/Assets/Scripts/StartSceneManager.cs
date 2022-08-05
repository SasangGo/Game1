using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 시작 화면 관리 매니저
public class StartSceneManager : Singleton<StartSceneManager>
{
    private const int MAINSCENENUMBER = 1;

    [SerializeField] GameObject recordPanel;
    [SerializeField] GameObject exitPanel;
    [SerializeField] Text RecordText;

    private void Update()
    {
        // 뒤로가기 버튼 이벤트
        if (Input.GetKeyDown(KeyCode.Escape))
            // recordPanel이 활성화 된 상태인 경우 -> 비활성화
            if (recordPanel.activeSelf)
                recordPanel.SetActive(false);
            // recordPanel이 비활성화 된 상태인 경우 -> exitPanel 활성 or 비활성화
            else
                if(!exitPanel.activeSelf)
                    exitPanel.SetActive(true);
                else
                    exitPanel.SetActive(false);

    }

    // StartButton 이벤트 -> 메인 화면으로 전환
    public void StartButton()
    {
        SceneManager.LoadScene(MAINSCENENUMBER);
    }

    // RecordButton 이벤트 -> 레코드 패널 활성화
    public void RecordButton()
    {
        UpdateRecordText();
        if (recordPanel != null)
            recordPanel.SetActive(true);
    }

    // CancelButton 이벤트 -> 레코드 패널 비활성화
    public void CancelButton()
    {
        if (recordPanel != null)
            recordPanel.SetActive(false);
    }

    // ExitPanel 안의 Yes, no 버튼 이벤트
    public void ExitPanelButtons(bool isYes)
    {
        if (isYes == true)
            Application.Quit();
        else
            exitPanel.SetActive(false);
            
    }

    // ExitPanel 안의 Yes, no 버튼 이벤트
    private void UpdateRecordText()
    {
        RecordText.text = "";
        for (int i = 1; i <= 10 && PlayerPrefs.HasKey("Record_" + i); i++)
            RecordText.text = RecordText.text + (i + ". " + PlayerPrefs.GetInt("Record_" + i) + "\n");
    }
}
