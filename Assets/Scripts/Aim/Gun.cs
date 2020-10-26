using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public float range = 100f;
    public Camera fpsCam;
    public Text text;

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
                    text.text = "Perfect";
                    text.color = Color.yellow;
                    break;
                case "FirstCircle":
                    text.text = "Great";
                    text.color = Color.magenta;
                    break;
                case "SecondCircle":
                    text.text = "Good";
                    text.color = Color.red;
                    break;
                case "OuterCircle":
                    text.text = "Ok";
                    text.color = Color.cyan;
                    break;
                default:
                    break;
            }
        }
    }
}
