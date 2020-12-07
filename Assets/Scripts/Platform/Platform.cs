using UnityEngine;

public class Platform : MonoBehaviour
{
    public Vector3[] points;
    protected int pointNumber;
    protected Vector3 currentTarget;
    protected Vector3 positionOnLastCheckpoint;
    protected Vector3 initialPosition;
    protected Vector3 targetOnLastCheckpoint;

    protected float tolerance;
    public float speed;
    protected Vector3 heading;

    // Start is called before the first frame update
    void Start()
    {
        if (points.Length > 0)
        {

            initialPosition = transform.position;
            positionOnLastCheckpoint = transform.position;
            ParkourManager.OnCheckpointDone += (int index, float time, float previousTime) =>
            {
                positionOnLastCheckpoint = transform.position;
                targetOnLastCheckpoint = currentTarget;
            };
            targetOnLastCheckpoint = points[0];
            currentTarget = points[0];
        }
        else
        {
            enabled = false;
        }
        tolerance = speed * Time.deltaTime;
    }

    void Update()
    {
        if(transform.position != currentTarget)
        {
            movePlatform();
        } else
        {
            updateTarget();
        }
    }

    void movePlatform()
    {
        heading = currentTarget - transform.position;
        Vector3 newPos = transform.position + (heading / heading.magnitude) * speed * Time.deltaTime;
        if((newPos - transform.position).magnitude < heading.magnitude)
        {
            transform.Translate((heading / heading.magnitude) * speed * Time.deltaTime);
        }
        else
        {
            transform.position = currentTarget;
            updateTarget();
        }
        //if (heading.magnitude < tolerance)
        //{
        //    transform.position = currentTarget;
        //}
    }

    void updateTarget()
    {
        if(points.Length > 0)
        {
            pointNumber = (pointNumber + 1) % points.Length;
            currentTarget = points[pointNumber];
        }
    }

    public Vector3 getHeading()
    {
        return heading;
    }

    public void SoftReset()
    {
        if (points.Length > 0)
        {
            transform.position = positionOnLastCheckpoint;
            currentTarget = targetOnLastCheckpoint;
        }
    }

    public void HardReset()
    {
        if (points.Length > 0)
        {
            transform.position = initialPosition;
            currentTarget = points[0];
        }
    }
}

