using UnityEngine;

public class Platform : MonoBehaviour
{
    public Vector3[] points;
    private int pointNumber;
    private Vector3 currentTarget;

    private float tolerance;
    public float speed;
    private Vector3 heading;

    public bool isFragile;
    private float firstHitTime;
    public float breakingTime;
    private bool isTouched;

    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if(points.Length > 0)
        {
            currentTarget = points[0];
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
        if(isFragile && isTouched && Time.time - firstHitTime >= breakingTime)
        {
            Destroy(gameObject);
        }
    }

    void movePlatform()
    {
        heading = currentTarget - transform.position;
        rigidbody.MovePosition(transform.position + (heading / heading.magnitude) * speed * Time.deltaTime);
        if (heading.magnitude < tolerance)
        {
            transform.position = currentTarget;
        }
    }

    void updateTarget()
    {
        if(points.Length > 0)
        {
            pointNumber++;
            if (pointNumber >= points.Length)
            {
                pointNumber = 0;
            }
            currentTarget = points[pointNumber];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if(collision.transform.name == "Ethan")
        {
            firstHitTime = Time.time;
            isTouched = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
    }

    public Vector3 getHeading()
    {
        return heading;
    }
}

