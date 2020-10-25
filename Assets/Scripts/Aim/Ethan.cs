using UnityEngine;

public class Ethan : MonoBehaviour
{
    [SerializeField]
    public GameObject gun;

    //private bool isAiming = false;

    private void Start()
    {
        gun.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            aim();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            stopAiming();
        }
    }


    void aim()
    {
        //isAiming = true;
        gun.SetActive(true);
    }

    void stopAiming()
    {
        //isAiming = false;
        gun.SetActive(false);
    }
}
