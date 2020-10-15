using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject m_camera;
    public float speed;
    public float maxY;
    public float minY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * Time.unscaledDeltaTime * speed, 0);
        m_camera.transform.Rotate(Input.GetAxisRaw("Mouse Y") * Time.unscaledDeltaTime * -speed, 0, 0);

        if (m_camera.transform.localRotation.x < minY)
        {
            Debug.Log("trop haut");
            m_camera.transform.localRotation = new Quaternion(minY, m_camera.transform.localRotation.y, m_camera.transform.localRotation.z, m_camera.transform.localRotation.w);
        }

        if (m_camera.transform.localRotation.x > maxY)
        {
            Debug.Log("trop bas");
            m_camera.transform.localRotation = new Quaternion(maxY, m_camera.transform.localRotation.y, m_camera.transform.localRotation.z, m_camera.transform.localRotation.w);
        }
    }
}
