using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatGetHurt : MonoBehaviour,IState
{
    Animator animator;
    CatState state;
    public void OnEnter()
    {
        
        animator = GetComponent<Animator>();
        state = GetComponent<CatState>();
        animator.Play("smallMaoDieHurt");
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.normalizedTime>0.99f)
        { state.TransState(NormalCatState.Chase); }
    }

   
}
