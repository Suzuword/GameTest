using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeipeiGethurt : MonoBehaviour,IState
{
  public  Animator animator;
  public  PeipeiState state;

    private void Start()
    {
        animator = GetComponent<Animator>();
        state=GetComponent<PeipeiState>();
        
    }
    public void OnEnter()
    {
        if (state.currentState != state.die)
            animator.Play("PeiPeiHurt");
    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.99f&&state.currentState!=state.die)
        { state.TransState(EPeipeiState.Chase); }
    }

 
}
