using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObstacle : Platform
{

    private bool isFragile = true;
    private bool hit = false;
    private MeshDestroy meshDestroy;

    public AK.Wwise.Event soundEvent;
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
        meshDestroy = GetComponent<MeshDestroy>();
    }

    public void takeAShot()
    {
        if (isFragile)
        {
            hit = true;
            if (hit)
            {
                soundEvent.Post(gameObject);
                meshDestroy.DestroyMesh();
            }
        }
    }
}
