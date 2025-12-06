using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum NormalCatState
{
    Patorl,Chase,Ha,Attack,GetHurt,Die,HaWhenFailedChase
}
public class CatState : MonoBehaviour
{

   NormalCatState catState;
  public  GameObject[] patorlPos;
   public IState currentState;
    public Transform Player;
   public IState patrol;
  public  IState chase;

    IState attack;
   public IState die;
    IState getHurt;
    public CatAttackDetection attackDetection;
  
    public void Start()
    {
      patrol = GetComponent<CatPatrol>();
      chase= GetComponent<CatChase>();
      attack = GetComponent<CatAttack>();
      die = GetComponent<CatDie>();
      getHurt = GetComponent<CatGetHurt>();
        TransState(NormalCatState.Patorl);
    }
    public void TransState(NormalCatState state)
    {
        IState newState = state switch
        {
            NormalCatState.Patorl => this.patrol,
            NormalCatState.Chase => this.chase,
      
            NormalCatState.Attack => this.attack,
            NormalCatState.Die => this.die,
            NormalCatState.GetHurt => this.getHurt,
    
            _ => this.patrol
        };
        
        
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    private void Update()
    {
        currentState.OnKeep();
    }
}
