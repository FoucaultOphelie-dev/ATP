using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject blueTarget;
    public GameObject redTarget;
    public GameObject greenTarget;
    private int xPos;
    private int yPos;
    private int targetType;
    public static int targetAmount;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting spawner");
    }

    // Update is called once per frame
    void Update()
    {
        if (targetAmount < 5)
        {
            targetType = (int)Random.Range(1, 4);
            xPos = Random.Range(-10, 10);
            yPos = Random.Range(1, 13);
            switch (targetType)
            {
                case 1:
                    Instantiate(blueTarget, new Vector3(xPos, yPos, 10), Quaternion.Euler(90,0,0));
                    break;

                case 2:
                    Instantiate(redTarget, new Vector3(xPos, yPos, 10), Quaternion.Euler(90,0, 0));
                    break;

                case 3:
                    Instantiate(greenTarget, new Vector3(xPos, yPos, 10), Quaternion.Euler(90, 0, 0));
                    break;

                default:
                    Instantiate(greenTarget, new Vector3(xPos, yPos, 10), Quaternion.Euler(90, 0, 0));
                    break;
            }
            targetAmount++;
        }
    }

    public static void targetDestroyed()
    {
        targetAmount--;
    }
}