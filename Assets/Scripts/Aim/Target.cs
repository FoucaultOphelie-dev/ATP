using UnityEngine;

public class Target : MonoBehaviour
{
    public int multiplier = 1;
    public bool initialState;


    public void takeAShot()
    {
        HideTarget();
    }

    public void ResetTarget()
    {
        if (initialState)
            ShowTarget();
    }

    public void ShowTarget()
    {
        gameObject.SetActive(true);
    }

    public void HideTarget()
    {
        gameObject.SetActive(false);
    }
}
