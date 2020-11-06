using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
public class Gun : MonoBehaviour
{

    public float range = 100f;
    public Camera fpsCam;
    public TextMeshProUGUI shotFeedback;
    public TextMeshProUGUI bullets;
    public TextMeshProUGUI reloadMessage;
    private int score;
    private string feedback;
    
    public int maxAmo;
    private int amountOfBullets;
    private bool reloading;
    private float reloadStartTime;
    public float reloadingTime;

    private void Start()
    {
        if (!shotFeedback)
        {
            GameObject feedbackGameObject = GameObject.Find("FeedbackAim");
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
            GameObject feedbackGameObject = GameObject.Find("Bullets");
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
            GameObject feedbackGameObject = GameObject.Find("ReloadMessage");
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
        if (Input.GetButtonDown("Fire1") && !reloading)
        {
            if(amountOfBullets <= 0)
            {
                reloadMessage.text = "You need to reload ! (press R)";
            } else
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
        bullets.text = getAmountOfBullets().ToString();
    }

    void shoot()
    {
        amountOfBullets--;
        RaycastHit hit;
        if ( Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponentInParent<Target>();
            if(target != null)
            {
                target.takeAShot();
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
