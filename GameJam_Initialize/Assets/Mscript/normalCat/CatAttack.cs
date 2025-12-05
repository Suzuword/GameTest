using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttack : MonoBehaviour,IState
{
    Animator animator;
    CatState catstate;

    private void Start()
    {
         animator = GetComponent<Animator>();
        catstate = GetComponent<CatState>();
    }
    public void OnEnter()
    {
        animator.Play("smallMaoDieAttack");
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
       AnimatorStateInfo stateInfo=animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.99f)
        { 
            if (catstate.attackDetection.canAttack)
            { catstate.TransState(NormalCatState.Attack); }
            else
            { catstate.TransState(NormalCatState.Chase); }
        }
    }
}
