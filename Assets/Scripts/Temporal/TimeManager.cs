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
    private Image jaugeUI;

    [Header("Player")]
    private CharacterMove playerCharacterMove;
    public bool powerActivate = false;

    [Header("PostProcess")]
    private ChromaticAberration m_chroma;
    private Vignette m_vignette;
    private ColorAdjustments m_color;

    private bool wasSlow;
    private bool wasFast;
    private float wasJaugeValue;
    private float vignetteValue;
    private float chromaValue;

    public Volume volume;

    [Header("Sounds")]
    public AK.Wwise.Event wwiseEventSlow;
    public AK.Wwise.Event wwiseEventFast;

    private bool soundRalentiDone = false;


    private void Start()
    {
        ParkourManager.OnCheckpointDone += (int index, float time, float lastTime) => {
            wasSlow = doRalenti;
            wasFast = doAcceleration;
            wasJaugeValue = jauge;
            vignetteValue = m_vignette.intensity.value;
            chromaValue = m_chroma.intensity.value;

        };
        ParkourManager.OnCheckpointReset += () => {
            doRalenti = wasSlow;
            doAcceleration = wasFast;
            jauge = wasJaugeValue;
            m_vignette.intensity.value = vignetteValue;
            m_chroma.intensity.value = chromaValue;
        };
        ParkourManager.OnParkourReset += () => {
            doRalenti = false;
            doAcceleration = false;
            jauge = 0;
            m_vignette.intensity.value = 0;
            m_chroma.intensity.value = 0;
            Debug.Log("jauge" + jauge);
        };
        volume.profile.TryGet<Vignette>(out m_vignette);
        volume.profile.TryGet<ChromaticAberration>(out m_chroma);
        volume.profile.TryGet<ColorAdjustments>(out m_color);

        if (!playerCharacterMove)
        {
            GameObject player = GameObject.FindGameObjectWithTag("PlayerRoot");
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
            GameObject jaugeGameObject = GameObject.FindObjectOfType<CanvasManager>().transform.Find("GameplayCanvas/TimeManagerJauge").gameObject;
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
        if (powerActivate)
        {
            if (Input.GetKeyDown(keyAcceleration))
            {
                wwiseEventFast.Post(gameObject);
            }
            if (Input.GetKeyDown(keyRalenti) && jauge > 0)
            {
                soundRalentiDone = true;
                wwiseEventSlow.Post(gameObject);
            }
            if (Input.GetKeyUp(keyAcceleration))
            {
                wwiseEventSlow.Post(gameObject);
            }
            if (Input.GetKeyUp(keyRalenti) && soundRalentiDone)
            {
                soundRalentiDone = false;
                wwiseEventFast.Post(gameObject);
            }
            //Debug.Log(Time.timeScale);
            if (Input.GetKey(keyAcceleration) && !Input.GetKey(keyRalenti))
            {
                playerCharacterMove.scaled = false;
                Time.timeScale = accelerationValue;
                doAcceleration = true;
            }
            else if (Input.GetKey(keyRalenti) && !Input.GetKey(keyAcceleration) && jauge > 0f)
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
            }
            else
            {
                StartCoroutine("ReturnChroma");
                //m_chroma.intensity.value = Mathf.Lerp(m_chroma.intensity.value, 0, 0.1f);
                //m_chroma.intensity.value = 0;
            }

            if (doRalenti)
            {
                m_vignette.intensity.value = Mathf.Lerp(m_vignette.intensity.value, 0.5f, 0.1f);
                m_color.colorFilter.value = Color.Lerp(m_color.colorFilter.value, new Color(0.3568441f, 0.752427f, 0.764151f, 0), 0.1f);
                jauge -= Time.unscaledDeltaTime * ralentiFacteur;
                if (jauge < 0f)
                {
                    jauge = 0f;
                }
            }
            else
            {
                StartCoroutine("ReturnColor");
                //m_color.colorFilter.value = Color.Lerp(m_color.colorFilter.value, new Color(1, 1, 1, 0), 0.1f);
                //m_color.colorFilter.value = new Color(1, 1, 1, 0);
            }

            if (!doAcceleration && !doRalenti)
            {
                StartCoroutine("ReturnVignette");
                //m_vignette.intensity.value = Mathf.Lerp(m_vignette.intensity.value, 0, 0.1f);
                //m_vignette.intensity.value = 0;
            }
        }
        jaugeUI.fillAmount = jauge / maxJauge;
    }

    IEnumerator ReturnColor()
    {
        yield return new WaitForSeconds(0.5f);
        //m_color.colorFilter.value = Color.Lerp(m_color.colorFilter.value, new Color(1, 1, 1, 0), 0.1f);
        if(!doRalenti) m_color.colorFilter.value = new Color(1, 1, 1, 0);
    }

    IEnumerator ReturnChroma()
    {
        yield return new WaitForSeconds(0.5f);
        //m_chroma.intensity.value = Mathf.Lerp(m_chroma.intensity.value, 0, 0.1f);
        if(!doAcceleration) m_chroma.intensity.value = 0;
    }
    IEnumerator ReturnVignette()
    {
        yield return new WaitForSeconds(0.5f);
        //m_vignette.intensity.value = Mathf.Lerp(m_vignette.intensity.value, 0, 0.1f);
        if (!doAcceleration && !doRalenti)
        {
            m_vignette.intensity.value = 0;
            //Debug.Log("Retour de la vignette");
        }
    }
}
