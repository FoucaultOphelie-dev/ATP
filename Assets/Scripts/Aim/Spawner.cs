using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject blueTarget;
    public GameObject redTarget;
    public GameObject greenTarget;
    private int zPos;
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
            zPos = (int) Random.Range(transform.position.z-10, transform.position.z+10);
            yPos = (int) Random.Range(transform.position.y-5, transform.position.y+5);
            switch (targetType)
            {
                case 1:
                    Instantiate(blueTarget, new Vector3(transform.position.x+10, yPos, zPos), Quaternion.Euler(0,0,90));
                    break;

                case 2:
                    Instantiate(redTarget, new Vector3(transform.position.x+10, yPos, zPos), Quaternion.Euler(0,0,90));
                    break;

                case 3:
                    Instantiate(greenTarget, new Vector3(transform.position.x + 10, yPos, zPos), Quaternion.Euler(0,0,90));
                    break;

                default:
                    Instantiate(greenTarget, new Vector3(transform.position.x + 10, yPos, zPos), Quaternion.Euler(0,0,90));
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