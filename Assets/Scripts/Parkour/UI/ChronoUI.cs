using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class ChronoUI : MonoBehaviour
{
    private ParkourManager instance;
    private TextMeshProUGUI textComponent;
    // Start is called before the first frame update
    void Start()
    {
        instance = ParkourManager.Instance();
        if (instance == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = TimerToChrono(ParkourManager.Instance().GetTimer());
    }

    string TimerToChrono(float timer)
    {
        return TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\.ff");
    }
}
