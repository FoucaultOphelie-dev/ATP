using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class ChronoUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ParkourManager.Instance().GetIsStarted())
        {
            
            textComponent.text = TimerToChrono(ParkourManager.Instance().GetTimer());
        }
    }

    string TimerToChrono(float timer)
    {
        //int minutes = Mathf.FloorToInt(timer / 60F);
        //int seconds = Mathf.FloorToInt(timer - minutes * 60);
        //int milliseconds = Mathf.FloorToInt(timer - seconds * 60);
        //return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, );
        return TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\:ff");
    }
}
