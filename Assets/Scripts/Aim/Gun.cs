using UnityEngine;

public class Gun : MonoBehaviour
{

    public float range = 100f;
    public Camera fpsCam;
    private bool isAiming = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            aim();
        }
        if (isAiming && Input.GetButtonDown("Fire1"))
        {
            shoot();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            stopAiming();
        }
    }

    void shoot()
    {
        RaycastHit hit;
        if ( Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.takeAShot();
            }
        }
    }

    void aim()
    {
        isAiming = true;
    }

    void stopAiming()
    {
        isAiming = false;
    }
}
