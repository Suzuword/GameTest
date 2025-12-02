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
    public int HaInt;
   NormalCatState catState;
  public  GameObject[] patorlPos;
   public IState currentState;
    public Transform Player;
    IState patrol;
  public  IState chase;
  public  IState ha;
    IState attack;
    IState die;
    IState getHurt;
    IState haWhenFailedChase;
    public void Start()
    {
      patrol = GetComponent<CatPatrol>();
      chase= GetComponent<CatChase>();
      ha= GetComponent<CatHa>();
      attack = GetComponent<CatAttack>();
      die = GetComponent<CatDie>();
      getHurt = GetComponent<CatGetHurt>();
haWhenFailedChase=GetComponent<CatHaWhenFailedChase>();
        TransState(NormalCatState.Patorl);
    }
    public void TransState(NormalCatState state)
    {
        IState newState = state switch
        {
            NormalCatState.Patorl => this.patrol,
            NormalCatState.Chase => this.chase,
            NormalCatState.Ha => this.ha,
            NormalCatState.Attack => this.attack,
            NormalCatState.Die => this.die,
            NormalCatState.GetHurt => this.getHurt,
            NormalCatState.HaWhenFailedChase => this.haWhenFailedChase,
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
