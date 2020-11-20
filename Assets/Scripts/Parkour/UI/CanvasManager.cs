using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class CanvasManager : MonoBehaviour
{
    public Transform introCanvas;
    public TextMeshProUGUI parkourNameDisplay;

    public Transform GameplayCanvas;

    public Transform ScoringCanvas;
    public TextMeshProUGUI scoringParkourNameDisplay;

    // Time
    public TextMeshProUGUI chronoDisplay;
    public TextMeshProUGUI chronoScoreDisplay;
    // Hits
    public TextMeshProUGUI hitsScore;
    public TextMeshProUGUI[] hitType;

    public TextMeshProUGUI totalScore;

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
}
