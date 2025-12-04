using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EBossCatState { Idle,Chase,Attack,Die}
public class BossCatState : MonoBehaviour
{
  public  EBossCatState bossCatState;
    public IState currentState;
    public Transform Player;
    public bool canAttack;

    public IState idle;
    public IState chase;
    public IState attack;
    public IState die;
    public bool findPlayer;
    public void Start()
    {
        
        chase = GetComponent<BossCatChase>();     
        attack = GetComponent<BossCatAttack>();
        die = GetComponent<BossCatDie>();
       idle=GetComponent<BossCatIdle>();
        TransState(EBossCatState.Idle);
    }
    public void TransState(EBossCatState state)
    {
        IState newState = state switch
        {

            EBossCatState.Chase => this.chase,

            EBossCatState.Attack => this.attack,
            EBossCatState.Die => this.die,
            EBossCatState.Idle => this.idle,

                 _ => this.idle
           
        };
        TransState(EBossCatState.Idle);

        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
}

