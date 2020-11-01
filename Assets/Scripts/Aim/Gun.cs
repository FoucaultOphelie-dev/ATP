using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public float range = 100f;
    public Camera fpsCam;
    public Text text;
    private int score;
    private string feedback;
    private void Start()
    {
        if (!text)
        {
            GameObject feedbackGameObject = GameObject.Find("FeedbackAim");
            if (feedbackGameObject)
            {
                Text feedbackText = feedbackGameObject.GetComponent<Text>();
                if (feedbackText)
                    text = feedbackText;
                else
                    Debug.LogError("FeedbackAim Text component missing");
            }
            else
            {
                Debug.LogError("FeedbackAim GameObject missing");
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            shoot();
        }
    }

    void shoot()
    {
        RaycastHit hit;
        if ( Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponentInParent<Target>();
            if(target != null)
            {
                target.takeAShot();
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
                    text.text = feedback;
                    text.color = Color.yellow;
                    break;
                case "FirstCircle":
                    score = 80 * target.multiplier; ;
                    feedback = "Great ";
                    if (score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    text.text = feedback;
                    text.color = Color.magenta;
                    break;
                case "SecondCircle":
                    score = 60 * target.multiplier;
                    feedback = "Good ";
                    if (score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    text.text = feedback;
                    text.color = Color.red;
                    break;
                case "OuterCircle":
                    score = 40 * target.multiplier;
                    feedback = "Ok ";
                    if (score > 0)
                    {
                        feedback += "+";
                    }
                    feedback += score;
                    text.text = feedback;
                    text.color = Color.cyan;
                    break;
                default:
                    break;
            }
        }
    }
}
