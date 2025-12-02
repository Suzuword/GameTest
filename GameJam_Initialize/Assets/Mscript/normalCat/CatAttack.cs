using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttack : MonoBehaviour,IState
{
    Animator animator;
    CatState catstate;
    public void OnEnter()
    {
       //²¥·Å¹¥»÷¶¯»­
       animator = GetComponent<Animator>();
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
       AnimatorStateInfo stateInfo=animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.99f)
        { catstate.TransState(NormalCatState.Chase); }
    }

}
