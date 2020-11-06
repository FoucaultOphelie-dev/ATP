﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public float range = 100f;
    public Camera fpsCam;
    public Text shotFeedback;
    public Text bullets;
    public Text reloadMessage;
    private int score;
    private string feedback;
    public int maxAmo;
    private int amountOfBullets;
    private bool reloading;
    private float reloadStartTime;
    public float reloadingTime;

    private void Start()
    {
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
