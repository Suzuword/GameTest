using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FairyGUI.RTLSupport;

public class PeipeiAttack : MonoBehaviour,IState
{
    public Animator animator;
    public PeipeiState state;
    public void OnEnter()
    {
        animator.Play("PeiPeiAttack");
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.99f)
        {
            if (state.canAttack)
            { state.TransState(EPeipeiState.Attack); }
            else
            { state.TransState(EPeipeiState.Chase); }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();
        state=GetComponent<PeipeiState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
