using UnityEngine;

public class Target : MonoBehaviour
{
    public int multiplier = 1;
    public bool hit = false;

    public void takeAShot()
    {
        hit = true;
        if (hit)
        {
            Destroy(gameObject);
            Spawner.targetDestroyed();
        }
    }
}
