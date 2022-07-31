using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 시작 화면 관리 매니저
public class StartSceneManager : MonoBehaviour
{
    private const int mainSceneNumber = 1;

    public GameObject recordPanel;


    // StartButton 이벤트 -> 메인 화면으로 전환
    public void StartButton()
    {
        SceneManager.LoadScene(mainSceneNumber);
    }

    // RecordButton 이벤트 -> 레코드 패널 활성화
    public void RecordButton()
    {
        if (recordPanel != null)
            recordPanel.SetActive(true);
    }

    // CancelButton 이벤트 -> 레코드 패널 비활성화
    public void CancelButton()
    {
        if (recordPanel != null)
            recordPanel.SetActive(false);
    }
}
