using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatPatrol : MonoBehaviour, IState
{
    CatState catState;
    Transform currentTarget;
    int currentIndex;
  public  Animator animator;
    public float patrolSpeed;
    private void Start()
    {
        catState=GetComponent<CatState>();
        currentTarget = catState.patorlPos[0].transform;
        animator = GetComponent<Animator>();
    }
    public void OnEnter()
    {
        animator.Play("smallMaoDieWalk");

    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {
        
        transform.position = Vector2.MoveTowards(transform.position,currentTarget.transform.position,patrolSpeed*Time.deltaTime);
       if (Vector2.Distance(currentTarget.transform.position, transform.position) < 0.21f)
        { AddPatrolIndex(); }
    }
    public void AddPatrolIndex()
    {
        currentIndex++; 
        if(currentIndex>=catState.patorlPos.Length)
        { currentIndex = 0; }
        currentTarget = catState.patorlPos[currentIndex].transform;
        if(currentIndex==0)
        { this.gameObject.transform.localScale = new Vector2(1, 1); }
        if(currentIndex==1)
        { this.gameObject.transform.localScale = new Vector2(-1, 1); }

    }
}
