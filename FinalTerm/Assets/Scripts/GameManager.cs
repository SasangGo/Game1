using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool isPhaseEnd;
    public bool isGameOver;
    public float Score { get; private set; }

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
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        startText.text = "Go!!";
        startTextAnim.Play("StartText",-1,0f);
        yield return new WaitForSeconds(1.5f);
        startText.gameObject.SetActive(false);
        StartCoroutine(ScoreTimer());
        Invoke("NextPhase", intervalTime);
    }
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
    public void NextPhase()
    {
        if (isGameOver) return;
        isPhaseEnd = false;
        StartPattern();
    }
    public void EndPhase(GameObject pattern)
    {
        isPhaseEnd = true;
        pattern.SetActive(false);
        Invoke("NextPhase", intervalTime);
    }
    public void GameOver()
    {
        Debug.Log("게임오버");
        gameOverPanel.gameObject.SetActive(true);
        resultText.text = Mathf.CeilToInt(Score) + "초";
        Time.timeScale = 0f;
        StopAllCoroutines();
        CancelInvoke();
    }
    public void ReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
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
