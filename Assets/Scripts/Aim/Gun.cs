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
    public GameObject mesh;
    
    public int maxAmo;
    private int amountOfBullets;
    private bool reloading;
    public float reloadingTime;
    private float reloadingTimer;
    //public VisualEffect tir;
    private string feedback;

    private int lastCheckAmmo;
    private bool lastCheckReloading = false;
    private float lastCheckReloadingTimer = 0;

    public CharacterMove playerMove;
    public AK.Wwise.Event wwiseEvent;

    void Start()
    {
        lastCheckAmmo = maxAmo;
        ParkourManager.OnCheckpointDone += (int index, float time, float lastTime) => {
            lastCheckAmmo = amountOfBullets;
            lastCheckReloading = reloading;
            lastCheckReloadingTimer = reloadingTimer;
        };
        ParkourManager.OnCheckpointReset += () => {
            amountOfBullets = lastCheckAmmo;
            reloading = lastCheckReloading;
            reloadingTimer = lastCheckReloadingTimer;
        };
        ParkourManager.OnParkourReset += () => {
            amountOfBullets = maxAmo;
            reloading = false;
            reloadingTimer = 0;
        };
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
        if(!playerMove) playerMove = FindObjectOfType<CharacterMove>();
        if (!playerMove) Debug.LogError("no player move on gun");
    }
    // Update is called once per frame
    void Update()
    {
        if (ParkourManager.Instance())
        {
            if (ParkourManager.Instance().parkourState != ParkourState.Gameplay) return;
        }
        if (playerMove.isAiming)
        {
            if (!mesh.activeSelf) mesh.SetActive(true);
            if (reloading)
            {
                reloadingTimer += Time.deltaTime;
                if (reloadingTimer >= reloadingTime)
                {
                    setReloading(false);
                    reloadMessage.text = "";
                    reloadingTimer = 0;
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
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
                if (Input.GetKeyDown("r") && getAmountOfBullets() < maxAmo)
                {
                    reload();
                    reloadMessage.text = "Reloading";
                }
            }
        }
        else
        {
            if (mesh.activeSelf) mesh.SetActive(false);
        }
        if(bullets)
            bullets.text = amountOfBullets.ToString();
    }

    void shoot()
    {
        amountOfBullets--;
        //tir.SetFloat("alpha", 1);
        wwiseEvent.Post(gameObject);

        StartCoroutine("CoTir");
        RaycastHit hit;
        LayerMask hittableLayer = LayerMask.GetMask("Hittable");
        if ( Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, hittableLayer))
        {
            Target target = hit.transform.GetComponentInParent<Target>();
            if(target != null)
            {
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
                //destructibleObstacle.takeAShot();
            }
            ParkourTrigger trigger = hit.transform.GetComponentInParent<ParkourTrigger>();
            if(trigger != null)
            {
                if (trigger.method == ParkourTrigger.TriggerMethod.Hit) trigger.DoTrigger();
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
    private void reload()
    {
        setReloading(true);
        setAmountOfBullets(maxAmo);
        reloadMessage.text = "";
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

    public bool IsReloading()
    {
        return reloading;
    }
    private IEnumerator CoTir()
    {

        yield return new WaitForSeconds(0.1f);
        //tir.SetFloat("alpha", -1.0f);
    }
}
