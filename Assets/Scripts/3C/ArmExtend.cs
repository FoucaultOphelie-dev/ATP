using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmExtend : MonoBehaviour
{
    public bool extend = false;
    public GameObject mir;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (extend)
        {
            if (mir != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("RightHandReach"));
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, animator.GetFloat("RightHandReach"));
                animator.SetIKPosition(AvatarIKGoal.RightHand, mir.transform.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, mir.transform.rotation);
            }
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetLookAtWeight(0);
        }
    }
}
