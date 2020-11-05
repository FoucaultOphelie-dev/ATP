using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [Header("Ralenti")]
    public KeyCode keyRalenti;
    public float ralentiValue;
    public float ralentiFacteur = 1;
    public bool doRalenti;

    [Header("Acceleration")]
    public KeyCode keyAcceleration;
    public float accelerationValue;
    public float accelerationFacteur = 1;
    public bool doAcceleration;
    
    [Header ("Jauge")]
    public float jauge = 0f;
    public float maxJauge;
    public Image jaugeUI;

    [Header("Player")]
    public CharacterMove player;
    
    void Update()
    {
        //Debug.Log(Time.timeScale);
        if(Input.GetKey(keyAcceleration) && !Input.GetKey(keyRalenti))
        {
            player.scaled = false;
            Time.timeScale = accelerationValue;
            doAcceleration = true;
        }
        else if(Input.GetKey(keyRalenti) && !Input.GetKey(keyAcceleration) && jauge > 0f)
        {
            player.scaled = true;
            Time.timeScale = ralentiValue;
            doRalenti = true;
        }
        else
        {
            Time.timeScale = 1;
            doAcceleration = false;
            doRalenti = false;
        }


        if (doAcceleration)
        {
            jauge += Time.unscaledDeltaTime * accelerationFacteur;
            if (jauge > maxJauge)
            {
                jauge = maxJauge;
            }
            jaugeUI.fillAmount = jauge / maxJauge;
        }

        if (doRalenti)
        {
            jauge -= Time.unscaledDeltaTime * ralentiFacteur;
            if (jauge < 0f)
            {
                jauge = 0f;
            }
            jaugeUI.fillAmount = jauge / maxJauge;
        }

        
    }
}
