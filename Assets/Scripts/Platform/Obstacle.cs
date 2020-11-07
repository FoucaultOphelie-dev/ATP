using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3[] points;
    private int pointNumber;
    private Vector3 currentTarget;

    private float tolerance;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        if (points.Length > 0)
        {
            currentTarget = points[0];
        }
        tolerance = speed * Time.deltaTime;
    }

    void Update()
    {
        if (transform.position != currentTarget)
        {
            moveObstacle();
        }
        else
        {
            updateTarget();
        }
    }

    void moveObstacle()
    {
        Vector3 heading = currentTarget - transform.position;
        transform.position += (heading / heading.magnitude) * speed * Time.deltaTime;
        if (heading.magnitude < tolerance)
        {
            transform.position = currentTarget;
        }
    }

    void updateTarget()
    {
        pointNumber++;
        if (pointNumber >= points.Length)
        {
            pointNumber = 0;
        }
        currentTarget = points[pointNumber];
    }
}

