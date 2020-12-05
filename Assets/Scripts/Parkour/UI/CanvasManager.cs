﻿using System.Collections;
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

    [Header("Chrono")]
    public TextMeshProUGUI chronoDisplay;
    public TextMeshProUGUI chronoScoreDisplay;

    [Header("Hits")]
    public TextMeshProUGUI hitsLabel;
    public TextMeshProUGUI hitsScore;
    public TextMeshProUGUI hitsCount;
    int hitsScoreTotal = 0;
    //public TextMeshProUGUI[] hitType;

    [Header("Total")]
    public TextMeshProUGUI newBestScoreLabel;
    public TextMeshProUGUI totalScore;
    public Image totalMedal;
    private Image medal;
    public Sprite[] medals = new Sprite[4];


    public float timeToFullScore;
    private float startTime;

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

    [Header("Character")]
    public AnimationClip[] clips;

    public void Start()
    {
        ParkourManager.OnParkourSwitchState += SwitchCanvas;

        // Setup Retry buttons
        transform.Find("ScoringCanvas/Container/Footer/ButtonsRow/retry")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);

        transform.Find("ScoringCanvas/Container/Footer/ButtonsRowWithNextLevel/retry")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);


        // Next Level buttons
        Button button = transform.Find("ScoringCanvas/Container/Footer/ButtonsRowWithNextLevel/nextLevel")
            .GetComponent<Button>();
        button.onClick.AddListener(() => { Loader.Load(ParkourManager.Instance().parkourData.nextParkour.scene.SceneName); });

        // Setup return menu buttons

        transform.Find("ScoringCanvas/Container/Footer/ButtonsRow/return")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);

        transform.Find("ScoringCanvas/Container/Footer/ButtonsRowWithNextLevel/return")
            .GetComponent<Button>()
            .onClick.AddListener(ParkourManager.ResetGameplay);

        if (ParkourManager.Instance().parkourData.nextParkour)
        {
            transform.Find("ScoringCanvas/Container/Footer/ButtonsRowWithNextLevel").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("ScoringCanvas/Container/Footer/ButtonsRow").gameObject.SetActive(true);
        }

        //Setup Gameplay Canvas
        backgroundJauge.color = backgroundColor;
        filledJauge.color = filledColor;
        List<CheckPoint> checkPoints = ParkourManager.Instance().GetCheckpoints();
        if(checkPoints.Count >= 2)
        {
            float step = 1.0f / (checkPoints.Count - 1);
            for (int i = 0; i < checkPoints.Count; i++)
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
                ParkourManager.Instance().player.GetComponent<CharacterMove>().animator.Play(clips[UnityEngine.Random.Range(0, clips.Length-1)].name);
                scoringParkourNameDisplay.text = ParkourManager.Instance().parkourData.displayName;
                chronoDisplay.text = ChronoUI.TimerToChrono(ParkourManager.Instance().GetTimer());
                //chronoScoreDisplay.text = "+ " + ParkourManager.Instance().TimeScore().ToString();
                chronoScoreDisplay.text = "";
                //Dictionary<int, int> hits = new Dictionary<int, int>();
                //foreach (int i in Enum.GetValues(typeof(HitFeedback)))
                //{
                //    hits.Add(i, 0);
                //}
                hitsLabel.enabled = false;
                hitsCount.enabled = false;
                foreach (var hit in ParkourManager.Instance().hits)
                {
                    //hits[(int)hit.feedback]++;
                    hitsScoreTotal += hit.score;
                }
                //hitsScore.text = "+ " + hitsScoreTotal.ToString();
                hitsScore.text = "";
                hitsCount.text = ParkourManager.Instance().hits.Count.ToString();
                //for (int i = 0; i < hitType.Length; i++)
                //{

                //    hitType[i].text = hits[i].ToString();
                //}
                totalScore.text = "";
                int i = ParkourManager.Instance().parkourData.medals.Length - 1;
                while (i >= 0)
                {
                    if (ParkourManager.Instance().score < ParkourManager.Instance().parkourData.medals[i].score) break;
                    i--;
                }
                totalMedal.sprite = medals[i];
                ScoringCanvas.gameObject.SetActive(true);
                StartCoroutine("ScoringRoutine");
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

    IEnumerator ScoringRoutine()
    {
        float tempScore = 0;
        float accumulateScore = 0;
        float pourcentage = 0;
        startTime = Time.time;
        while (pourcentage < 1)
        {
            tempScore = (int)Mathf.Lerp(0, ParkourManager.Instance().TimeScore(), pourcentage);
            totalScore.text = (accumulateScore + tempScore).ToString();
            chronoScoreDisplay.text = "+ " + tempScore.ToString();
            pourcentage = (Time.time - startTime) / timeToFullScore;
            yield return null;
        }
        accumulateScore += tempScore;

        startTime = Time.time;
        tempScore = 0;
        pourcentage = 0;
        hitsLabel.enabled = true;
        hitsCount.enabled = true;
        if (hitsScoreTotal > 0)
        {
            while (pourcentage < 1)
            {
                tempScore = (int)Mathf.Lerp(0, hitsScoreTotal, pourcentage);
                hitsScore.text = "+ " + tempScore.ToString();
                totalScore.text = (accumulateScore + tempScore).ToString();
                pourcentage = (Time.time - startTime) / timeToFullScore;
                yield return null;
            }
            accumulateScore += tempScore;
        }
        totalMedal.color = Color.white;
        if (ParkourManager.Instance().newBestScore)
        {
            newBestScoreLabel.gameObject.SetActive(true);
        }
        yield return null;
    }
}
