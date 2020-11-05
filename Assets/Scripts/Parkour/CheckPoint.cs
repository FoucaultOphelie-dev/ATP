using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CheckPoint : MonoBehaviour
{
    public int index;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && index > 0)
        {
            if (ParkourManager.Instance().PlayerEnterCheckpoint(index))
            {
                animator.SetTrigger("isDone");
                animator.ResetTrigger("isReset");
            }
        }
    }

    public void ResetCheckpoint()
    {
        animator.SetTrigger("isReset");
        animator.ResetTrigger("isDone");
    }
}
