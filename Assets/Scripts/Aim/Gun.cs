using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.VFX;


public enum HitFeedback
{
    Perfect,
    VeryGood,
    Good,
    Ok
}

public struct Hit
{
    public HitFeedback feedback;
    public string text;
    public int score;
    public Color color;
}

public class Gun : MonoBehaviour
{

    public delegate void TargetHit(Hit hit, Transform target);
    public static event TargetHit OnTargetHit;

    public float range = 100f;
    public Camera fpsCam;
    public TextMeshProUGUI shotFeedback;
    public TextMeshProUGUI bullets;
    public TextMeshProUGUI reloadMessage;
    private int score;
    private string textFeedback;
    
    public int maxAmo;
    private int amountOfBullets;
    private bool reloading;
    private float reloadStartTime;
    public float reloadingTime;
    public VisualEffect tir;
    private string feedback;

    void Start()
    {
        //Find UI
        RectTransform gameplayCanvas = GameObject.Find("CanvasManager").transform.Find("GameplayCanvas").GetComponent<RectTransform>();
        if (gameplayCanvas)
        {
            if (!shotFeedback)
            {
                Transform feedbackGameObject = gameplayCanvas.Find("FeedbackAim");
                if (feedbackGameObject)
                {
                    TextMeshProUGUI feedbackText = feedbackGameObject.GetComponent<TextMeshProUGUI>();
                    if (feedbackText)
                        shotFeedback = feedbackText;
                    else
                        Debug.LogError("FeedbackAim Text component missing");
                }
                else
                {
                    Debug.LogError("FeedbackAim GameObject missing");
                }
            }
            if (!bullets)
            {
                Transform feedbackGameObject = gameplayCanvas.Find("ReloadLayer/Bullets");
                if (feedbackGameObject)
                {
                    TextMeshProUGUI bulletsFeedback = feedbackGameObject.GetComponent<TextMeshProUGUI>();
                    if (bulletsFeedback)
                        bullets = bulletsFeedback;
                    else
                        Debug.LogError("FeedbackAim Text component missing");
                }
                else
                {
                    Debug.LogError("FeedbackAim GameObject missing");
                }
            }
            if (!reloadMessage)
            {
                Transform feedbackGameObject = gameplayCanvas.Find("ReloadLayer/ReloadMessage");
                if (feedbackGameObject)
                {
                    TextMeshProUGUI reloadFeedback = feedbackGameObject.GetComponent<TextMeshProUGUI>();
                    if (reloadFeedback)
                        reloadMessage = reloadFeedback;
                    else
                        Debug.LogError("FeedbackAim Text component missing");
                }
                else
                {
                    Debug.LogError("FeedbackAim GameObject missing");
                }
            }
        }
        amountOfBullets = maxAmo;
        reloading = false;
        reloadMessage.color = Color.white;
        bullets.color = Color.white;
        reloadMessage.text = "";
        bullets.text = amountOfBullets.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if (ParkourManager.Instance())
        {
            if (ParkourManager.Instance().parkourState != ParkourState.Gameplay) return;
        }
        if (Input.GetButtonDown("Fire1") && !reloading)
        {
            if (amountOfBullets <= 0)
            {
                reloadMessage.text = "You need to reload ! (press R)";
            }
            else
            {
                shoot();
            }
        }

        if (getReloading() && Time.time - getReloadStartTime() > reloadingTime)
        {
            setReloading(false);
            reloadMessage.text = "";
        }
        if (getReloading())
        {
            reloadMessage.text = "Reloading";
        }
        if (Input.GetKeyDown("r") && getAmountOfBullets() < maxAmo)
        {
            reload();
        }
        if(bullets)
            bullets.text = getAmountOfBullets().ToString();
    }

    void shoot()
    {
        amountOfBullets--;
        tir.SetFloat("alpha", 1);

        StartCoroutine("CoTir");
        RaycastHit hit;
        if ( Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponentInParent<Target>();
            if(target != null)
            {
                target.takeAShot();
                Hit hitfeedBack = new Hit();
                switch (hit.transform.name)
                {
                    case "InnerCircle":
                        hitfeedBack.score = 100 * target.multiplier;
                        hitfeedBack.feedback = HitFeedback.Perfect;
                        hitfeedBack.text = "Perfect ";
                        if (hitfeedBack.score > 0)
                        {
                            hitfeedBack.text += "+";
                        }
                        hitfeedBack.text += hitfeedBack.score;
                        hitfeedBack.color = Color.yellow;
                        break;
                    case "FirstCircle":
                        hitfeedBack.score = 80 * target.multiplier;
                        hitfeedBack.feedback = HitFeedback.VeryGood;
                        hitfeedBack.text = "Very Good ";
                        if (hitfeedBack.score > 0)
                        {
                            hitfeedBack.text += "+";
                        }
                        hitfeedBack.text += hitfeedBack.score;
                        hitfeedBack.color = Color.magenta;
                        break;
                    case "SecondCircle":
                        hitfeedBack.score = 60 * target.multiplier;
                        hitfeedBack.feedback = HitFeedback.Good;
                        hitfeedBack.text = "Good ";
                        if (hitfeedBack.score > 0)
                        {
                            hitfeedBack.text += "+";
                        }
                        hitfeedBack.text += hitfeedBack.score;
                        hitfeedBack.color = Color.red;
                        break;
                    case "OuterCircle":
                        hitfeedBack.score = 40 * target.multiplier;
                        hitfeedBack.feedback = HitFeedback.Ok;
                        hitfeedBack.text = "Ok ";
                        if (hitfeedBack.score > 0)
                        {
                            hitfeedBack.text += "+";
                        }
                        hitfeedBack.text += hitfeedBack.score;
                        hitfeedBack.color = Color.cyan;
                        break;
                    default:
                        break;
                }
                shotFeedback.text = hitfeedBack.text;
                shotFeedback.color = hitfeedBack.color;
                OnTargetHit?.Invoke(hitfeedBack, hit.transform);
            }
            DestructibleObstacle destructibleObstacle = hit.transform.GetComponentInParent<DestructibleObstacle>();
            if(destructibleObstacle != null)
            {
                destructibleObstacle.takeAShot();
            }
            switch (hit.transform.name)
            {
                case "InnerCircle":
                    score = 100 * target.multiplier;
                    feedback = "Perfect ";
                    if(score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    shotFeedback.text = feedback;
                    shotFeedback.color = Color.yellow;
                    break;
                case "FirstCircle":
                    score = 80 * target.multiplier; ;
                    feedback = "Great ";
                    if (score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    shotFeedback.text = feedback;
                    shotFeedback.color = Color.magenta;
                    break;
                case "SecondCircle":
                    score = 60 * target.multiplier;
                    feedback = "Good ";
                    if (score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    shotFeedback.text = feedback;
                    shotFeedback.color = Color.red;
                    break;
                case "OuterCircle":
                    score = 40 * target.multiplier;
                    feedback = "Ok ";
                    if (score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    shotFeedback.text = feedback;
                    shotFeedback.color = Color.cyan;
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator CoTir()
    {
        
        yield return new WaitForSeconds(0.1f);
        tir.SetFloat("alpha", -1.0f);
    } 
    private void reload()
    {
        setReloading(true);
        setReloadStartTime(Time.time);
        setAmountOfBullets(maxAmo);
        reloadMessage.text = "";
    }

    public float getReloadStartTime()
    {
        return reloadStartTime;
    }

    public bool getReloading()
    {
        return reloading;
    }

    public void setReloading(bool isReloading)
    {
        reloading = isReloading;
    }

    public int getAmountOfBullets()
    {
        return amountOfBullets;
    }
    public void setAmountOfBullets(int bullets)
    {
        amountOfBullets = bullets;
    }

    public void setReloadStartTime(float startTime)
    {
        reloadStartTime = startTime;
    }
}
