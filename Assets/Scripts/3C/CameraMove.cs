using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject m_camera;
    public float speed;
    public float maxY;
    public float minY;

    public bool inSlide;
    void Update()
    {
        if (!inSlide)
        {
            m_camera.transform.localRotation = new Quaternion(m_camera.transform.localRotation.x, 0.0f, 0.0f, m_camera.transform.localRotation.w);
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * Time.unscaledDeltaTime * speed, 0);
        }
        else
        {
            m_camera.transform.Rotate(0, Input.GetAxisRaw("Mouse X") * Time.unscaledDeltaTime * speed, 0, 0);
            m_camera.transform.localRotation = new Quaternion(m_camera.transform.localRotation.x, Mathf.Clamp(m_camera.transform.localRotation.y, -0.7f, 0.7f), 0.0f, m_camera.transform.localRotation.w);
        }
        m_camera.transform.Rotate(Input.GetAxisRaw("Mouse Y") * Time.unscaledDeltaTime * -speed, 0, 0);

        if (m_camera.transform.localRotation.x < minY)
        {
            m_camera.transform.localRotation = new Quaternion(minY, m_camera.transform.localRotation.y, m_camera.transform.localRotation.z, m_camera.transform.localRotation.w);
        }

        if (m_camera.transform.localRotation.x > maxY)
        {
            m_camera.transform.localRotation = new Quaternion(maxY, m_camera.transform.localRotation.y, m_camera.transform.localRotation.z, m_camera.transform.localRotation.w);
        }
    }
}
