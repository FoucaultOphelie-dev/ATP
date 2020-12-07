using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParkourTrigger : MonoBehaviour
{
    public enum TriggerMethod
    {
        None,
        Trigger,
        Collision,
        Hit
    }
    public delegate void Trigger(ParkourTrigger trigger);
    public static event Trigger OnTrigger;

    public bool onlyOnce;
    public TriggerMethod method = TriggerMethod.Trigger;
    public bool resetPosition = true;
    private bool alreadyTriggered = false;

    public UnityEvent onEnterAction;
    public UnityEvent onExitAction;
    public UnityEvent onResetAction;


    private void OnTriggerEnter(Collider other)
    {
        if (method != TriggerMethod.Trigger) return;
        if(other.tag == "Player")
        {
            DoTrigger();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (method != TriggerMethod.Collision) return;
        if (collision.transform.tag == "Player")
        {
            DoTrigger();
        }
    }

    public void DoTrigger()
    {
        if (alreadyTriggered && onlyOnce) return;
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
