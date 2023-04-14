using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKGrabber : MonoBehaviour
{
    [SerializeField] private bool ikActive = false;
    [SerializeField] private Transform rightHandObject = null;
    [SerializeField] private Transform lookObject = null;

    private Animator animator = null;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (ikActive)
        {
            // We want to look at the look object
            if(lookObject != null)
            {
                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(lookObject.position);
            }
            else
            {
                animator.SetLookAtWeight(0);
            }

            if(rightHandObject != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObject.position);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObject.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }
}
