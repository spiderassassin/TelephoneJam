using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorHelper : MonoBehaviour
{
    // For now it only works to change the talking parameter in the animator
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetTalking(bool isTalking)
    {
        animator.SetBool("talking", isTalking);
    }

}
