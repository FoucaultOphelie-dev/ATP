using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    private float firstHitTime;
    public float breakingTime;
    private bool isTouched;
    private MeshDestroy meshDestroy;
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
            meshDestroy.DestroyMesh();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if (collision.transform.tag == "Player")
        {
            firstHitTime = Time.time;
            isTouched = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
    }
}
