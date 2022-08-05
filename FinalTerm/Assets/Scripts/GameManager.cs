using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool isPhaseEnd; //페이즈 체크
    public bool isGameOver; // 게임오버 체크
    public float Score { get; private set; } // 스코어

    [SerializeField] Text scoreText;
    [SerializeField] Text startText;
    [SerializeField] Text resultText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject[] patterns;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] float intervalTime;

    private int idx;
    private int preIdx = -1;
    private Animator startTextAnim;
    private List<int> recordList;


    private const int STARTSCENENUMBER = 0;

    private void Start()
    {
        Score = 0;
        intervalTime = 2f;
        isPhaseEnd = true;
        isGameOver = false;
        startTextAnim = startText.GetComponent<Animator>();
        // GameOver함수에서 timeScale이 0이 된 경우 방지
        Time.timeScale = 1f;

        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OptionEvent();
    }

    // 게임 시작 문구 코루틴
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        startText.text = "Go!!";
        startTextAnim.Play("StartText",-1,0f);
        yield return new WaitForSeconds(1.5f);
        startText.gameObject.SetActive(false);

        // 시작 문구가 끝나면 score타이머가 돌아가고 다음 페이즈 타이머가 Invoke
        StartCoroutine(ScoreTimer());
        Invoke("NextPhase", intervalTime);
    }
    // 패턴을 랜덤으로 결정하고 불러옴
    public void StartPattern()
    {
        do
        {
            idx = Random.Range(0, patterns.Length);
        } while (preIdx == idx);
        APattern pattern = patterns[idx].GetComponent<APattern>();
        if (pattern != null)
        {
            Debug.Log(idx);
            pattern.gameObject.SetActive(true);
            preIdx = idx;
        }
    }
    // 다음 패턴 불러옴
    public void NextPhase()
    {
        if (isGameOver) return;
        isPhaseEnd = false;
        StartPattern();
    }
    // 해당 패턴 종료하고 일정 시간후 다음 페이즈 시작
    public void EndPhase(GameObject pattern)
    {
        isPhaseEnd = true;
        pattern.SetActive(false);
        Invoke("NextPhase", intervalTime);
    }
    // 게임 오버
    public void GameOver()
    {
        Debug.Log("게임오버");
        gameOverPanel.gameObject.SetActive(true);
        resultText.text = Mathf.CeilToInt(Score) + "초";
        Time.timeScale = 0f;
        RecordScore();
        StopAllCoroutines(); // 모든 코루틴 종료
        CancelInvoke(); // 모든 함수 종료
    }
    // 게임 재시작
    public void ReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    // 게임 나가기
    public void ExitGame()
    {
        SceneManager.LoadScene(STARTSCENENUMBER);
    }
    // optiomButtom 클릭 이벤트
    public void OptionButton()
    {
        OptionEvent();
    }

    // 옵션 창 띄우기
    private void OptionEvent()
    {
        if (!optionPanel.activeSelf)
        {
            optionPanel.gameObject.SetActive(true);
            joystick.RangeReSize(100f, 100f);
            Time.timeScale = 0f;
        }
        else
        {
            optionPanel.gameObject.SetActive(false);
            joystick.RangeReSize(650f, 650f);
            // 게임 오버 상태 체크 : 게임 오버 후 움직이번 안되니께
            if (!isGameOver)
                Time.timeScale = 1f;
        }
    }


    // 스코어 타이머
    private IEnumerator ScoreTimer()
    {
        while (!isGameOver)
        {
            Score += Time.deltaTime;
            scoreText.text = Mathf.CeilToInt(Score) + "";
            yield return null;
        }
    }

    // 스코어 기록 함수
    private void RecordScore()
    {
        recordList = new List<int>();
        Debug.Log("Record 1");

        // 저장되어 있던 기록표들 가져옴
        for(int i = 1; i<=10 && PlayerPrefs.HasKey("Record_" + i); i++)
        {
            recordList.Add(PlayerPrefs.GetInt("Record_" + i));
        }
        recordList.Add((int)Score);


        Debug.Log("Record 2");
        // 저장되어 있던 기록 + 새 기록 -> 내림차순 정렬
        recordList.Sort((delegate (int A, int B) //내림차순; 오름차순 정렬의 경우 return값을 반대로 해주면 된다 1<-> -1
        {
            if (A < B) return 1;
            else if (A > B) return -1;
            return 0; //동일한 값일 경우
        }));
        Debug.Log("Record 3");

        // 다시 기록
        int j = 1;
        foreach (int record in recordList)
        {
            Debug.Log(j + ". " + record);
            if (j <= 10)
                PlayerPrefs.SetInt("Record_" + j, record);
            j++;
        }

        Debug.Log("Record 4");
    }
}
