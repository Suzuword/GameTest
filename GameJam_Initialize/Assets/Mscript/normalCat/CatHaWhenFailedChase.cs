using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHaWhenFailedChase : MonoBehaviour,IState
{
    CatState catState;
    Animator animator;
    public void OnEnter()
    {
       catState = GetComponent<CatState>();
        animator = GetComponent<Animator>();
        //²¥·Å¹þÆø¶¯»­
    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {
        AnimatorStateInfo state =animator.GetCurrentAnimatorStateInfo(0);
        if (state.normalizedTime > 0.99f) {
            catState.TransState(NormalCatState.Patorl);
        
        }

    }

   
}
