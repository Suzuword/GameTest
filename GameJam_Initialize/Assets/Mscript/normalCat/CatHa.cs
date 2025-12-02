using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHa : MonoBehaviour,IState
{
    Animator HA;
    CatState catstate;
    public void OnEnter()
    {
       
       HA = GetComponent<Animator>();
        catstate=GetComponent<CatState>();
        if (catstate.HaInt < 2)
        {//²¥·Å¹þÆø¶¯»­ 
        }
        if(catstate.HaInt>=2)
        { catstate.TransState(NormalCatState.Chase); }

    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
        if (catstate.HaInt >=3) catstate.HaInt=0;
        AnimatorStateInfo state = HA.GetCurrentAnimatorStateInfo(0);
        if(state.normalizedTime>=0.95f)
        { catstate.HaInt++; }
    
    }

    
}
