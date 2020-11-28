using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class CanvasManager : MonoBehaviour
{
    [Header("Canvas reference")]
    public Transform introCanvas;
    public TextMeshProUGUI parkourNameDisplay;

    public Transform GameplayCanvas;

    public Transform ScoringCanvas;
    public TextMeshProUGUI scoringParkourNameDisplay;

    [Header("Time")]
    public TextMeshProUGUI chronoDisplay;
    public TextMeshProUGUI chronoScoreDisplay;

    [Header("Hits")]
    public TextMeshProUGUI hitsScore;
    public TextMeshProUGUI[] hitType;

    public TextMeshProUGUI totalScore;

    [Header("Completion Jauge")]
    public Image backgroundJauge;
    public Image filledJauge;
    public Color backgroundColor;
    public Color filledColor;

    [Header("Checkpoints Completed")]
    public RectTransform CheckpointsUIFolder;
    public GameObject checkpointUIPrefab;
    public List<GameObject> checkpointsUI;
    public Color checkpointValidatedColor;

    public void Start()
    {
        ParkourManager.OnParkourSwitchState += SwitchCanvas;

        // Setup Retry buttons
        transform.Find("ScoringCanvas/Image/ButtonsRow/retry")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);

        transform.Find("ScoringCanvas/Image/ButtonsRowWithNextLevel/retry")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);


        // Next Level buttons
        Button button = transform.Find("ScoringCanvas/Image/ButtonsRowWithNextLevel/nextLevel")
            .GetComponent<Button>();
        button.onClick.AddListener(() => { Loader.Load(ParkourManager.Instance().parkourData.nextParkour.scene.SceneName); });

        // Setup return menu buttons

        transform.Find("ScoringCanvas/Image/ButtonsRow/return")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);

        transform.Find("ScoringCanvas/Image/ButtonsRowWithNextLevel/return")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);

        if (ParkourManager.Instance().parkourData.nextParkour)
        {
            transform.Find("ScoringCanvas/Image/ButtonsRowWithNextLevel").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("ScoringCanvas/Image/ButtonsRow").gameObject.SetActive(true);
        }

        //Setup Gameplay Canvas
        backgroundJauge.color = backgroundColor;
        filledJauge.color = filledColor;
        List<CheckPoint> checkPoints = ParkourManager.Instance().GetCheckpoints();
        float step = 1.0f / (checkPoints.Count-1);
        for(int i = 0; i < checkPoints.Count; i++)
        {
            checkpointsUI.Add(GameObject.Instantiate(checkpointUIPrefab, CheckpointsUIFolder));
            RectTransform uiTransform = checkpointsUI[checkpointsUI.Count - 1].GetComponent<RectTransform>();
            uiTransform.anchorMin = new Vector2(step * i, 0);
            uiTransform.anchorMax = new Vector2(step * i, 1);
        }
        ValidCheckpointUI(0, 0, 0);//Validate the start;
        ParkourManager.OnCheckpointDone += ValidCheckpointUI;
        ParkourManager.OnParkourReset += ResetGameplayCanvas;
    }

    public void SwitchCanvas(ParkourState state)
    {
        introCanvas.gameObject.SetActive(false);
        GameplayCanvas.gameObject.SetActive(false);
        ScoringCanvas.gameObject.SetActive(false);
        switch (state)
        {
            case ParkourState.Intro:
                introCanvas.gameObject.SetActive(true);
                parkourNameDisplay.text = ParkourManager.Instance().parkourData.displayName;
                break;
            case ParkourState.Gameplay:
                GameplayCanvas.gameObject.SetActive(true);
                break;
            case ParkourState.Scoring:
                scoringParkourNameDisplay.text = ParkourManager.Instance().parkourData.displayName;
                chronoDisplay.text = ChronoUI.TimerToChrono(ParkourManager.Instance().GetTimer());
                chronoScoreDisplay.text = ParkourManager.Instance().TimeScore().ToString();
                Dictionary<int, int> hits = new Dictionary<int, int>();
                int hitsScoreTotal = 0;
                foreach (int i in Enum.GetValues(typeof(HitFeedback)))
                {
                    hits.Add(i, 0);
                }
                foreach (var hit in ParkourManager.Instance().hits)
                {
                    hits[(int)hit.feedback]++;
                    hitsScoreTotal += hit.score;
                }
                hitsScore.text = hitsScoreTotal.ToString();
                for (int i = 0; i < hitType.Length; i++)
                {

                    hitType[i].text = hits[i].ToString();
                }
                totalScore.text = ParkourManager.Instance().score.ToString();
                ScoringCanvas.gameObject.SetActive(true);
                break;
        }
    }
    void Update()
    {
        if (!ParkourManager.Instance()) return;
        switch (ParkourManager.Instance().parkourState)
        {
            case ParkourState.Intro:
                break;
            case ParkourState.Gameplay:
                UpdateGameplayCanvas();
                break;
            case ParkourState.Scoring:
                break;
            default:
                break;
        }
    }

    private void UpdateGameplayCanvas()
    {
        filledJauge.fillAmount = (ParkourManager.Instance().alreadyDone + ParkourManager.Instance().maxCompletion) / ParkourManager.Instance().parkourLenght;
    }

    private void ResetGameplayCanvas()
    {
        filledJauge.fillAmount = 0;
        for(int i = 1; i < checkpointsUI.Count; i++) checkpointsUI[i].GetComponent<Image>().color = checkpointUIPrefab.GetComponent<Image>().color;
    }

    private void ValidCheckpointUI(int index, float time, float previousTime)
    {
        checkpointsUI[index].GetComponent<Image>().color = checkpointValidatedColor;
    }
}
