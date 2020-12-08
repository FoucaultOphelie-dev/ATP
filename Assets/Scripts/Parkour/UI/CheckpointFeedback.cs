using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckpointFeedback : MonoBehaviour
{
    public float feedbackDuration;
    public float fadeOutDuration;

    public Color betterTime;
    public Color SameTime;
    public Color worstTime;

    public float initalSize;
    public float fadeOutSize;

    private ParkourManager instance;
    private TextMeshProUGUI timeTextComponent;
    private TextMeshProUGUI timeDiffTextComponent;


    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        ParkourManager.OnCheckpointReset += StopFeedbackCoroutine;
        ParkourManager.OnParkourReset += StopFeedbackCoroutine;
        instance = ParkourManager.Instance();
        if (instance == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        timeTextComponent = transform.Find("TimeFeedback").GetComponent<TextMeshProUGUI>();
        timeDiffTextComponent = transform.Find("TimeDiffFeedback").GetComponent<TextMeshProUGUI>();
        initalSize = timeTextComponent.fontSize;
        ParkourManager.OnCheckpointDone += UpdateFeedback;
    }

    // Update is called once per frame
    void UpdateFeedback(int index, float time, float previousTime)
    {
        StopFeedbackCoroutine();
        coroutine = WaitAndFadeOut();
        timeTextComponent.alpha = 1;
        timeTextComponent.color = Color.white;
        timeTextComponent.text = TimerToChrono(time);
        if(previousTime != -1)
        {
            timeDiffTextComponent.alpha = 1;
            string diff;
            if (time < previousTime)
            {
                timeDiffTextComponent.color = betterTime;
                diff = "-";
            }
            else if (time > previousTime)
            {
                timeDiffTextComponent.color = worstTime;
                diff = "+";
            }
            else
            {
                timeDiffTextComponent.color = SameTime;
                diff = "=";
            }
            timeDiffTextComponent.text = diff + TimerToChrono(Mathf.Abs(time - previousTime));
        }
        StartCoroutine(coroutine);
    }

    string TimerToChrono(float timer)
    {
        return TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\:ff");
    }

    IEnumerator WaitAndFadeOut()
    {
        yield return new WaitForSeconds(feedbackDuration);
        float elapsedTime = 0, waitTime = fadeOutDuration;
        while (elapsedTime <= waitTime)
        {
            float alpha = Mathf.Lerp(1, 0, (elapsedTime / waitTime));
            float fontSize = Mathf.Lerp(initalSize, fadeOutSize, (elapsedTime / waitTime));

            // alpha
            timeTextComponent.alpha = alpha;
            timeDiffTextComponent.alpha = alpha;
            // Size
            timeTextComponent.fontSize = fontSize;
            timeDiffTextComponent.fontSize = fontSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // alpha
        timeTextComponent.alpha = 0;
        timeDiffTextComponent.alpha = 0;
        // Size
        timeTextComponent.fontSize = initalSize;
        timeDiffTextComponent.fontSize = initalSize;
    }

    private void StopFeedbackCoroutine()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        timeTextComponent.alpha = 0;
        timeDiffTextComponent.alpha = 0;
        timeTextComponent.fontSize = initalSize;
        timeDiffTextComponent.fontSize = initalSize;
    }
}