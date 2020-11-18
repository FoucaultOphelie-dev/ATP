using UnityEngine;

public class Target : MonoBehaviour
{
    public int multiplier = 1;

    public void takeAShot()
    {
        HideTarget();
    }

    public void ResetTarget()
    {
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
