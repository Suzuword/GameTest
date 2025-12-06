using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeipeiIdle : MonoBehaviour,IState
{
    Animator animator;
    public void OnEnter()
    {
        animator.Play("PeiPeiStand");
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
}
