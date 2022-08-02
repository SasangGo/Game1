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
    [SerializeField] GameObject[] patterns;
    [SerializeField] float intervalTime;

    private int idx;
    private int preIdx = -1;
    private Animator startTextAnim;

    private void Start()
    {
        Score = 0;
        intervalTime = 2f;
        isPhaseEnd = true;
        isGameOver = false;
        startTextAnim = startText.GetComponent<Animator>();

        StartCoroutine(StartGame());
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
        StopAllCoroutines(); // 모든 코루틴 종료
        CancelInvoke(); // 모든 함수 종료
    }
    // 게임 재시작
    public void ReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
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
}
