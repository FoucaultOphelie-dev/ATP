using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public GameObject introCanvas;
    public TextMeshProUGUI parkourNameDisplay;

    public GameObject GameplayCanvas;

    public GameObject ScoringCanvas;
    public TextMeshProUGUI scoringParkourNameDisplay;
    public TextMeshProUGUI chronoDisplay;
    public TextMeshProUGUI chronoScoreDisplay;

    public void Start()
    {
        ParkourManager.onParkourSwitchState += SwitchCanvas;
    }

    public void SwitchCanvas(ParkourState state)
    {
        introCanvas.SetActive(false);
        GameplayCanvas.SetActive(false);
        ScoringCanvas.SetActive(false);
        switch (state)
        {
            case ParkourState.Intro:
                introCanvas.SetActive(true);
                parkourNameDisplay.text = ParkourManager.Instance().parkourData.displayName;
                break;
            case ParkourState.Gameplay:
                GameplayCanvas.SetActive(true);
                break;
            case ParkourState.Scoring:
                scoringParkourNameDisplay.text = ParkourManager.Instance().parkourData.displayName;
                chronoDisplay.text = ChronoUI.TimerToChrono(ParkourManager.Instance().GetTimer());
                chronoScoreDisplay.text = ParkourManager.Instance().score.ToString();
                ScoringCanvas.SetActive(true);
                break;
        }
    }
}
