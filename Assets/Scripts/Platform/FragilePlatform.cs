using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    private float firstHitTime;
    public float breakingTime;
    public bool isTouched;
    private MeshDestroy meshDestroy;
    public AK.Wwise.Event craquementsEvent;
    public AK.Wwise.Event craquementStopEvent;
    public AK.Wwise.Event crashEvent;
    // Start is called before the first frame update
    void Start()
    {
        meshDestroy = GetComponent<MeshDestroy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouched && Time.time - firstHitTime >= breakingTime)
        {
            craquementStopEvent.Post(gameObject);
            crashEvent.Post(gameObject);
            meshDestroy.DestroyMesh();
            isTouched = false;
        }
    }

    public void TriggerCollision()
    {
        firstHitTime = Time.time;
        isTouched = true;
        craquementsEvent.Post(gameObject);
    }
    public void ResetPlatform()
    {
        isTouched = false;
    }

    private void OnCollisionExit(Collision collision)
    {
    }
}
