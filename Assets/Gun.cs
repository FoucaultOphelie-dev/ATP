using UnityEngine;

public class Gun : MonoBehaviour
{

    public float range = 100f;
    //public bool isAiming = false;
    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetButtonDown("Aim"))
        {
            isAiming = true;
        }*/
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
        }
    }
}
