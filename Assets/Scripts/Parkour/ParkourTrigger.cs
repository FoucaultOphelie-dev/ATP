using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParkourTrigger : MonoBehaviour
{
    public UnityEvent onEnterAction;
    public UnityEvent onExitAction;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            onEnterAction?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            onExitAction?.Invoke();
        }
    }
}
