using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCatAttack : MonoBehaviour,IState
{
    public Animator animator;
    public BossCatState state;
    public void OnEnter()
    {
        animator=GetComponent<Animator>();
        state=GetComponent<BossCatState>();
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
       AnimatorStateInfo stateInfo= animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.normalizedTime>=0.99f&&state.canAttack)
        { state.TransState(EBossCatState.Attack); }
        if (stateInfo.normalizedTime >= 0.99f && !state.canAttack)
        { state.TransState(EBossCatState.Chase); }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
