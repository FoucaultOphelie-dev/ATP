using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingCheckpoint : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void TriggerStartRun()
    {
        ParkourManager.Instance().StartRun();
        animator.ResetTrigger("Reset");
    }
    private void Update()
    {

    }

    public void StartRun()
    {
        animator.SetTrigger("Start");
    }

    public void ResetRun()
    {
        animator.SetTrigger("Reset");
    }
}
