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

    [SerializeField] Text startText;
    [SerializeField] Text resultText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject AchievementPanel;
    [SerializeField] public GameObject skillChoicePanel;
    [SerializeField] public GameObject[] skillPanel;
    [SerializeField] public GameObject[] AchieveItem;
    [SerializeField] public Text[] skillPanelHeadText;
    [SerializeField] public Text[] skillPanelInfoText;
    [SerializeField] public Image[] skillPanelImage;
    [SerializeField] public Image[] hpImages;
    [SerializeField] public Image[] inHpImages;
    [SerializeField] public Button[] activeSkillButtons;
    [SerializeField] public Image[] activeSkillButtonImage;
    [SerializeField] public Image[] activeSkillCoolTimeImage;
    [SerializeField] Slider levelBar;
    [SerializeField] Text levelText;
    [SerializeField] GameObject[] patterns;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] float intervalTime;
    [SerializeField] PlayerControl player;

    private int idx;
    private int preIdx = -1;
    private Animator startTextAnim;
    private List<int> recordList;
    private List<int> randomSkillNumbers;

    private const int STARTSCENENUMBER = 0;
    private const int BOSSINTEVAR = 20;

    private int phase;

    private void Start()
    {
        phase = 0;
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
        {
            if(AchievementPanel.activeSelf)
                ActivePanel(AchievementPanel, !AchievementPanel.activeSelf);
            else
                ActivePanel(optionPanel, !optionPanel.activeSelf);
        }

        LevelBarUpdate();
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
            player.patternExp = pattern.expAmount;
            preIdx = idx;
        }
    }
    public void SummonBoss(int stage)
    {
    }
    // 다음 패턴 불러옴
    public void NextPhase()
    {
        if (isGameOver) return;
        isPhaseEnd = false;

        phase++;
        Debug.Log("phase :" + phase);

        // 일정 페이즈마다 보스 스테이지 진행
        int stage = (phase / BOSSINTEVAR) + 1;
        if (phase % BOSSINTEVAR == 0) SummonBoss(stage);
        else StartPattern();
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
    public void ClickReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    // 게임 나가기
    public void ClickExitGame()
    {
        SceneManager.LoadScene(STARTSCENENUMBER);
    }
    // optiomButtom 클릭 이벤트
    public void ClickOptionButton()
    {
        ActivePanel(optionPanel, !optionPanel.activeSelf);
        if(AchievementPanel.activeSelf)
            AchievementPanel.SetActive(false);
    }
    // AchieveButton 클릭 이벤트
    public void ClickAchievementButton()
    {
        ActivePanel(AchievementPanel, !AchievementPanel.activeSelf);
        if (optionPanel.activeSelf)
            optionPanel.SetActive(false);
    }

    // 캔슬 버튼 클릭 이벤트
    public void ClickCancleButton(string button)
    {
        switch (button)
        {
            case "Option":
                ActivePanel(optionPanel, !optionPanel.activeSelf);
                break;
            case "Achievement":
                ActivePanel(AchievementPanel, !AchievementPanel.activeSelf);
                break;
        }
    }

    public void ActivePanel(GameObject panel, bool togleActive)
    {
        if (togleActive)
        {
            joystick.UseJoystick(false);
            panel.SetActive(true);

            Time.timeScale = 0;
        }
        else
        {
            joystick.UseJoystick(true);
            panel.SetActive(false);

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
            yield return null;
        }
    }

    // 레벨업바 UI 업데이트
    public void LevelBarUpdate()
    {
        levelText.text = "Lv. " + player.level;
        levelBar.value = (player.exp / player.maxExp) * 100;
    }

    // 플레이어 체력 UI 업데이트 함수
    public void HpImageUpdate()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < player.maxHp)
                hpImages[i].gameObject.SetActive(true);
            else
                hpImages[i].gameObject.SetActive(false);

            if (i < player.hp)
                inHpImages[i].gameObject.SetActive(true);
            else
                inHpImages[i].gameObject.SetActive(false);
        }
    }

    // 스킬 패널 세팅
    public void SetSkillPanels()
    {
        // skillPanel.Length(선택할 수 있는 스킬의 개수) 만큼 랜덤 스킬을 뽑음
        randomSkillNumbers = SkillManager.Instance.RandomSkill(skillPanel.Length);

        // SkillPanel들의 UI 업데이트
        int index = 0;
        foreach(int skillNum in randomSkillNumbers)
        {
            skillPanelHeadText[index].text = "" + SkillManager.Instance.skillNames[skillNum];
            if(skillNum != (int)SkillManager.Skills.Heal && skillNum != (int)SkillManager.Skills.InvincibilitySkill)
                skillPanelHeadText[index].text += " Lv." + SkillManager.Instance.skillLevel[skillNum];
            skillPanelInfoText[index].text = "" + SkillManager.Instance.skill_Info[skillNum];
            skillPanelImage[index].sprite = SkillManager.Instance.skillSprites[skillNum];
            index++;
        }
    }

    // 스킬 선택 창에서 하나의 스킬을 선택하면 호출되는 함수
    public void ClickSkillPanel(int skillIndex)
    {
        // 스킬매니저에서 선택한 스킬을 적용함
        SkillManager.Instance.ChoiceSkillApply((SkillManager.Skills)randomSkillNumbers[skillIndex]);
        ActivePanel(skillChoicePanel, false);
        SoundManager.Instance.PlaySound(SoundManager.Instance.uIAudioSource, SoundManager.Instance.SkillGetSound);
    }

    public void ActiveSkillButtonActive(int index, int skillNumber)
    {
        activeSkillButtonImage[index].sprite = SkillManager.Instance.skillSprites[skillNumber];
        activeSkillButtons[index].gameObject.SetActive(true);
    }

    public void ShowLeftCoolTime(int index, float currentCoolTime, float maxCoolTime)
    {
        activeSkillCoolTimeImage[index].fillAmount = currentCoolTime / maxCoolTime;
    }

    public void SetAchieveItem(int index, Achievement achieve)
    {
        Text[] texts = AchieveItem[index].GetComponentsInChildren<Text>();
        Image[] image = AchieveItem[index].GetComponentsInChildren<Image>();

        texts[0].text = achieve.achieveTitle;
        texts[1].text = achieve.achieveExplane;
        texts[2].text = "효과 : " + achieve.achieveEffect;
        image[3].sprite = AchieveManager.Instance.AchieveImageList[index];
    }

    // 스코어 기록 함수
    private void RecordScore()
    {
        recordList = new List<int>();

        // 저장되어 있던 기록표들 가져옴
        for(int i = 1; i<=10 && PlayerPrefs.HasKey("Record_" + i); i++)
        {
            recordList.Add(PlayerPrefs.GetInt("Record_" + i));
        }
        recordList.Add((int)Score);

        // 저장되어 있던 기록 + 새 기록 -> 내림차순 정렬
        recordList.Sort((delegate (int A, int B) //내림차순; 오름차순 정렬의 경우 return값을 반대로 해주면 된다 1<-> -1
        {
            if (A < B) return 1;
            else if (A > B) return -1;
            return 0; //동일한 값일 경우
        }));

        // 다시 기록
        int j = 1;
        foreach (int record in recordList)
        {
            if (j <= 10)
                PlayerPrefs.SetInt("Record_" + j, record);
            j++;
        }

    }
}
