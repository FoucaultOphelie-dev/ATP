using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingCheckpoint : MonoBehaviour
{
    public AK.Wwise.Event soundDepart123;
    public AK.Wwise.Event soundDepartGo;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void TriggerStartRun()
    {
        soundDepartGo.Post(gameObject);
        ParkourManager.Instance().StartRun();
        animator.ResetTrigger("Reset");
    }

    public void Trigger123Start()
    {
        soundDepart123.Post(gameObject);
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
