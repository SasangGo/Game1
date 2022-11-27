﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>

{
    // 변하지 않아야 할 상수값들을 게임매니저에 정의해두고 사용
    public int MAP_START_X { get; private set; } = -10;
    public int MAP_START_Z { get; private set; } = -20;
    public int MAP_END_X { get; private set; } = 35;
    public int MAP_END_Z { get; private set; } = 25;
    public int CELL_SIZE { get; private set; } = 5;
    public int CELL_OFFSET_Y { get; private set; } = -6;

    public int DEADLINE { get; private set; } = -17;

    public bool isPhaseEnd; //페이즈 체크
    public bool isGameOver; // 게임오버 체크
    public float Score { get; private set; } // 스코어

    [SerializeField] Text startText;
    [SerializeField] Text resultText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameClearPanel;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject AchievementPanel;
    [SerializeField] public GameObject AchievePopUpPanel;
    [SerializeField] Text PopUpAchieveText;
    [SerializeField] Image PopUpAchieveImage;
    [SerializeField] public GameObject statChoicePanel;
    [SerializeField] public GameObject skillChoicePanel;
    [SerializeField] public GameObject slotChoicePanel;
    [SerializeField] public GameObject[] AchieveItem;
    [SerializeField] public GameObject[] statPanel;
    [SerializeField] public Text[] statPanelHeadText;
    [SerializeField] public Text[] statPanelInfoText;
    [SerializeField] public Image[] statPanelImage;

    [SerializeField] public GameObject[] skillPanel;
    [SerializeField] public Text[] skillPanelHeadText;
    [SerializeField] public Text[] skillPanelInfoText;
    [SerializeField] public Image[] skillPanelImage;

    [SerializeField] public GameObject[] slotPanel;
    [SerializeField] public Text[] slotPanelHeadText;
    [SerializeField] public Text[] slotPanelInfoText;
    [SerializeField] public Image[] slotPanelImage;

    [SerializeField] public Text[] statText;
    [SerializeField] public Image[] hpImages;
    [SerializeField] public Image[] inHpImages;
    [SerializeField] public Button[] activeSkillButtons;
    [SerializeField] public Image[] activeSkillButtonImage;
    [SerializeField] public Image[] activeSkillCoolTimeImage;
    [SerializeField] Slider levelBar;
    [SerializeField] Text levelText;
    [SerializeField] Text stageText;
    [SerializeField] GameObject[] patterns;
    [SerializeField] GameObject[] bosses;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] float intervalTime;
    [SerializeField] PlayerControl player;

    private int idx;
    private int preIdx = -1;
    private Animator startTextAnim;
    private List<int> randomSkillNumbers;

    private const int STARTSCENENUMBER = 0;
    private const int BOSSINTEVAR = 5;

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
            if (AchievementPanel.activeSelf)
                ClickAchievementButton();
            else
                ClickOptionButton();
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
        bosses[stage].SetActive(true);

    }
    
    // 다음 패턴 불러옴
    public void NextPhase()
    {
        if (isGameOver) return;
        isPhaseEnd = false;

        phase++;
        stageText.text = "Stage : " + phase;

        // 일정 페이즈마다 보스 스테이지 진행
        int stage = (phase / BOSSINTEVAR) - 1;
        if(phase == 60)
        {
            EndGame();
            return;
        }

        if (phase % BOSSINTEVAR == 0) SummonBoss(stage);
        else StartPattern();
    }

    public void IntervalNextPhase()
    {
        Invoke("NextPhase", intervalTime);
    }

    // 해당 패턴 종료하고 일정 시간후 다음 페이즈 시작
    public void EndPhase(GameObject pattern)
    {
        isPhaseEnd = true;
        pattern.SetActive(false);
        Invoke("NextPhase", intervalTime);
    }

    public void EndGame()
    {
        gameOverPanel.gameObject.SetActive(true);
        resultText.text = Mathf.CeilToInt(Score) + "초";
        Time.timeScale = 0f;
        StopAllCoroutines(); // 모든 코루틴 종료
        CancelInvoke(); // 모든 함수 종료

        DataManager.Instance.clearCount++;


        if (DataManager.Instance.clearCount == 1)
            AchieveManager.Instance.AchieveFirstClear();
        if (!player.isFall)
            AchieveManager.Instance.AchieveNoFalling();
        if (DataManager.Instance.clearCount == 5)
            AchieveManager.Instance.AchieveFiveClear();
        if (!SkillManager.Instance.isStatGet)
            AchieveManager.Instance.AchieveNoStat();
        if (!SkillManager.Instance.isTrans)
            AchieveManager.Instance.AchieveNoTransform();
        if (!player.isHit)
            AchieveManager.Instance.AchieveNoHit();




        DataManager.Instance.SaveAchieve();
    }

    // 게임 오버
    public void GameOver()
    {
        DataManager.Instance.SaveAchieve();
        Debug.Log("게임오버");
        gameOverPanel.gameObject.SetActive(true);
        resultText.text = Mathf.CeilToInt(Score) + "초";
        Time.timeScale = 0f;
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
        SetStatPanel();
        ActivePanel(optionPanel, !optionPanel.activeSelf);
        if(AchievementPanel.activeSelf)
            AchievementPanel.SetActive(false);
    }
    // AchieveButton 클릭 이벤트
    public void ClickAchievementButton()
    {
        for (int i = 0; i < AchieveManager.Instance.achieveList.Count; i++)
            SetAchieveItem(AchieveManager.Instance.achieveList[i]);
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

    // 스탯 패널 세팅
    public void SetStatPanels()
    {
        // skillPanel.Length(선택할 수 있는 스킬의 개수) 만큼 랜덤 스킬을 뽑음
        randomSkillNumbers = SkillManager.Instance.RandomSkill(statPanel.Length, true);

        // SkillPanel들의 UI 업데이트
        int index = 0;
        foreach(int skillNum in randomSkillNumbers)
        {
            statPanelHeadText[index].text = "" + SkillManager.Instance.skillNames[skillNum];
            if(skillNum != (int)SkillManager.Skills.Heal && skillNum != (int)SkillManager.Skills.InvincibilitySkill)
                statPanelHeadText[index].text += " Lv." + SkillManager.Instance.skillLevel[skillNum];
            statPanelInfoText[index].text = "" + SkillManager.Instance.skill_Info[skillNum];
            statPanelImage[index].sprite = SkillManager.Instance.skillSprites[skillNum];
            index++;
        }
    }

    // 스킬 선택 창에서 하나의 스킬을 선택하면 호출되는 함수
    public void ClickStatPanel(int statIndex)
    {        
        if (statIndex == 2) // 스킬 포인트 획득 선택시
            SkillManager.Instance.ChoiceStatApply(SkillManager.Skills.ActiveSkillPoint);
        else // 그 외에 스킬매니저에서 선택한 스킬을 적용함
            SkillManager.Instance.ChoiceStatApply((SkillManager.Skills)randomSkillNumbers[statIndex]);

        if (skillChoicePanel.activeSelf)
            statChoicePanel.SetActive(false);
        else
            ActivePanel(statChoicePanel, false);
        SoundManager.Instance.PlaySound(SoundManager.Instance.uIAudioSource, SoundManager.Instance.SkillGetSound);
    }

    public void SetSkillPanels()
    {
        // skillPanel.Length(선택할 수 있는 스킬의 개수) 만큼 랜덤 스킬을 뽑음
        randomSkillNumbers = SkillManager.Instance.RandomSkill(skillPanel.Length, false);

        // SkillPanel들의 UI 업데이트
        int index = 0;
        foreach (int skillNum in randomSkillNumbers)
        {
            skillPanelHeadText[index].text = "" + SkillManager.Instance.skillNames[skillNum];
            skillPanelInfoText[index].text = "" + SkillManager.Instance.skill_Info[skillNum];
            skillPanelImage[index].sprite = SkillManager.Instance.skillSprites[skillNum];
            index++;
        }
    }

    // 스킬 선택 창에서 하나의 스킬을 선택하면 호출되는 함수
    public void ClickSkillPanel(int skillIndex)
    {
        if (skillIndex == 1)// 스킬 포인트 획득 선택시
        {
            SkillManager.Instance.ChoiceStatApply(SkillManager.Skills.ActiveSkillPoint);
            ActivePanel(skillChoicePanel, false);
        }
        else // 그 외에 스킬매니저에서 선택한 스킬을 적용함
        {
            if(SkillManager.Instance.ActSkillIndex > 1)
            {
                SetSlotPanels();
                ActivePanel(skillChoicePanel, false);
                ActivePanel(slotChoicePanel, true);
            }
            else
            {
                SkillManager.Instance.GetActiveSkill((SkillManager.Skills)randomSkillNumbers[skillIndex], SkillManager.Instance.ActSkillIndex++);
                ActivePanel(skillChoicePanel, false);
            }

        }
        SoundManager.Instance.PlaySound(SoundManager.Instance.uIAudioSource, SoundManager.Instance.SkillGetSound);
    }

    public void SetSlotPanels()
    {
        // skillPanel.Length(선택할 수 있는 스킬의 개수) 만큼 랜덤 스킬을 뽑음
        for(int i = 0; i < slotPanel.Length; i++)
        {
            int skillNum = (int)SkillManager.Instance.actSkillButtonNumber[i];
            slotPanelHeadText[i].text = "" + SkillManager.Instance.skillNames[skillNum];
            slotPanelInfoText[i].text = "" + SkillManager.Instance.skill_Info[skillNum];
            slotPanelImage[i].sprite = SkillManager.Instance.skillSprites[skillNum];
        }
    }

    public void ClickSlotPanel(int slotIndex)
    {
        SkillManager.Instance.GetActiveSkill((SkillManager.Skills)randomSkillNumbers[0], slotIndex);
        ActivePanel(slotChoicePanel, false);

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

    public void SetAchieveItem(Achievement achieve)
    {
        Text[] texts = AchieveItem[achieve.index].GetComponentsInChildren<Text>();
        Image[] image = AchieveItem[achieve.index].GetComponentsInChildren<Image>();

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

    public void PopUpAchieve(Achievement achieve)
    {
        PopUpAchieveText.text = achieve.achieveTitle;
        PopUpAchieveImage.sprite = AchieveManager.Instance.achieveImageList[achieve.index];

        RectTransform popupRect = AchievePopUpPanel.GetComponent<RectTransform>();
        popupRect.anchoredPosition = new Vector2(0, 75f);
        AchievePopUpPanel.SetActive(true);

        StopCoroutine(PopDownAchieve(popupRect));
        StartCoroutine(PopDownAchieve(popupRect));
    }

    public IEnumerator PopDownAchieve(RectTransform popupRect)
    {
        float delayTime = 0;
        float speed = 3f;
        while (delayTime < 1f)
        {
            delayTime += Time.fixedDeltaTime;
            popupRect.anchoredPosition = Vector2.Lerp(popupRect.anchoredPosition, new Vector2(0, -75f), speed * Time.fixedDeltaTime);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        yield return new WaitForSecondsRealtime(2f);
        delayTime = 0;
        while (delayTime < 1f)
        {
            delayTime += Time.fixedDeltaTime;
            popupRect.anchoredPosition = Vector2.Lerp(popupRect.anchoredPosition, new Vector2(0, 75f), speed * Time.fixedDeltaTime);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }


        AchievePopUpPanel.SetActive(false);
    }

    public void SetStatPanel()
    {
        statText[0].text = "" + SkillManager.Instance.skillLevel[(int)SkillManager.Skills.IncreaseInvincibilityTime];
        statText[1].text = "" + SkillManager.Instance.skillLevel[(int)SkillManager.Skills.IncreaseSpeed];
        statText[2].text = "" + SkillManager.Instance.skillLevel[(int)SkillManager.Skills.IncreaseExp];
        statText[3].text = "" + SkillManager.Instance.skillLevel[(int)SkillManager.Skills.DecreaseCoolTime];
        statText[4].text = "" + SkillManager.Instance.skillLevel[(int)SkillManager.Skills.IncreaseJump];
        statText[5].text = "" + SkillManager.Instance.skillLevel[(int)SkillManager.Skills.ActiveSkillPoint];
    }
}
