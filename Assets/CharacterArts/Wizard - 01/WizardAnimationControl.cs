using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAnimationControl : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SetWalkingAnimation(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void SetAttackAnimation(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }

    public void OnAttackPlayEnd()
    {
        animator.SetBool("isAttacking", false);
    }
}
