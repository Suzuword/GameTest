using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantCatAttack : MonoBehaviour,IState
{
    Animator animator;
    GiantCatState catstate;
    public void OnEnter()
    {
        //²¥·Å¹¥»÷¶¯»­
        animator = GetComponent<Animator>();
        catstate=GetComponentInParent<GiantCatState>();
    }

    public void OnExit()
    {

    }

    public void OnKeep()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.99f)
        {
            if (catstate.attackDetection.canAttack)
                catstate.TransState(EGiantCatState.Attack);
            else
            {catstate.TransState(EGiantCatState.Patrol);}
        }
    }
}
