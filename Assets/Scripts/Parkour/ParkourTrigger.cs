using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParkourTrigger : MonoBehaviour
{
    public UnityEvent action;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            action?.Invoke();
        }
    }
}
