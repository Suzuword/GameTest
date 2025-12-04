using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatDie : MonoBehaviour,IState
{
    Animator animator;
    CatState state;

    private void Start()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<CatState>();
    }
    public void OnEnter()
    {

        animator.Play("smallMaoDieDead");
    }

    public void OnExit()
    {

    }

    public void OnKeep()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.99f)
        { Invoke("DestroyObject", 1f); }
    }
    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
