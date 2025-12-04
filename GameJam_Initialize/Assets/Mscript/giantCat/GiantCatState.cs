using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGiantCatState { Patrol, Attack, GetHurt, Die, }
public class GiantCatState : MonoBehaviour
{
    NormalCatState catState;
    public GameObject[] patorlPos;
    public IState currentState;
    public Transform Player;
    IState patrol;
    IState attack;
    IState gethurt;
  public  IState die;
    public CatAttackDetection attackDetection;
    public bool canAttack;
    void Start()
    {
        patrol=GetComponent<GiantCatPatrol>();
        attack=GetComponent<GiantCatAttack>();
        gethurt=GetComponent<GiantCatGetHurt>();
        die=GetComponent<GiantCatDie>();    
        TransState(EGiantCatState.Patrol);
    }

    public void TransState(EGiantCatState state)
    {
        IState newState = state switch
        {
            EGiantCatState.Patrol => this.patrol,
            EGiantCatState.Attack => this.attack,
            EGiantCatState.Die => this.die,
            EGiantCatState.GetHurt => this.gethurt,
        
            _ => this.patrol
        };

       
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
    // Update is called once per frame
    void Update()
    {
        currentState.OnKeep();
    }
}
