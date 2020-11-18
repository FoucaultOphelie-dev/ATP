using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParkourTrigger : MonoBehaviour
{
    public delegate void Trigger(ParkourTrigger trigger);
    public static event Trigger OnTrigger;

    public bool onlyOnce;
    private bool alreadyTriggered = false;

    public UnityEvent onEnterAction;
    public UnityEvent onExitAction;
    public UnityEvent onResetAction;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!onlyOnce)
            {
                DoTrigger();
            }
            else
            {
                if(!alreadyTriggered) DoTrigger();
            }
        }
    }

    public void DoTrigger()
    {
        onEnterAction?.Invoke();
        OnTrigger?.Invoke(this);
        alreadyTriggered = true;
    }

    public void ResetTrigger()
    {
        onResetAction?.Invoke();
        alreadyTriggered = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            onExitAction?.Invoke();
        }
    }
}
