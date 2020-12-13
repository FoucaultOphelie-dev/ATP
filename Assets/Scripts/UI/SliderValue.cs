using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    public TextMeshProUGUI textValue;
    public Slider slider;
    public void SetValue()
    {
        if (slider && textValue)
        {
            textValue.text = slider.value.ToString();
        }
    }
}
