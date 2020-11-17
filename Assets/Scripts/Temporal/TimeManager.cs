using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
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

    [Header("PostProcess")]
    private ChromaticAberration m_chroma;
    private Vignette m_vignette;
    public Volume volume;
    

    private void Start()
    {
        volume.profile.TryGet<Vignette>(out m_vignette);
        volume.profile.TryGet<ChromaticAberration>(out m_chroma);

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
        //Debug.Log(Time.timeScale);
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
            //postPorecessing.profile.TryGetSubclassOf<>
            m_vignette.intensity.value = Mathf.Lerp(m_vignette.intensity.value, 0.5f, 0.1f);
            m_chroma.intensity.value = Mathf.Lerp(m_chroma.intensity.value, 0.5f, 0.1f);
            
            jauge += Time.unscaledDeltaTime * accelerationFacteur;
            if (jauge > maxJauge)
            {
                jauge = maxJauge;
            }
            jaugeUI.fillAmount = jauge / maxJauge;
        }
        else
        {
            m_vignette.intensity.value = 0;
            m_chroma.intensity.value = 0;
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
