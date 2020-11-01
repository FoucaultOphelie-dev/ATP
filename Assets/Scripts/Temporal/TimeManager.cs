using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [Header("Ralenti")]
    public KeyCode keyRalenti = KeyCode.A;
    public float ralentiValue = 0.5f;
    public float ralentiFacteur = 1;
    public bool doRalenti;

    [Header("Acceleration")]
    public KeyCode keyAcceleration = KeyCode.E;
    public float accelerationValue = 1.5f;
    public float accelerationFacteur = 1;
    public bool doAcceleration;
    
    [Header ("Jauge")]
    public float jauge = 0f;
    public float maxJauge = 10;
    public Image jaugeUI;

    [Header("Player")]
    public CharacterMove playerCharacterMove;

    private void Start()
    {
        if (!playerCharacterMove)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                CharacterMove moveComponent = player.GetComponent<CharacterMove>();
                if (moveComponent)
                    playerCharacterMove = moveComponent;
                else
                    Debug.LogError("Player Character Move script missing");
            }
        }

        if (!jaugeUI)
        {
            GameObject jaugeGameObject = GameObject.Find("TimeManagerJauge");
            if (jaugeGameObject)
            {
                Image jaugeImage = jaugeGameObject.GetComponent<Image>();
                if (jaugeImage)
                    jaugeUI = jaugeImage;
                else
                    Debug.LogError("Time Manager Jauge UI missing");
            }
        }
    }

    void Update()
    {
        Debug.Log(Time.timeScale);
        if(Input.GetKey(keyAcceleration) && !Input.GetKey(keyRalenti))
        {
            playerCharacterMove.scaled = false;
            Time.timeScale = accelerationValue;
            doAcceleration = true;
        }
        else if(Input.GetKey(keyRalenti) && !Input.GetKey(keyAcceleration) && jauge > 0f)
        {
            playerCharacterMove.scaled = true;
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
