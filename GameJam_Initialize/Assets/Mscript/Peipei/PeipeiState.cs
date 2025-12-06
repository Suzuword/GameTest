using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPeipeiState { Attack,Idle,Chase,GetHurt,Die}
public class PeipeiState : MonoBehaviour
{
    EPeipeiState peipeiState;
    public IState currentState;
    public IState chase;

    IState attack;
    public IState die;
    IState getHurt;
    public IState idle;
    public Transform Player;
    public bool canAttack;

    public void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
        chase = GetComponent<PeipeiChase>();
        attack = GetComponent<PeipeiAttack>();
        die = GetComponent<PeipeiDie>();
        getHurt = GetComponent<PeipeiGethurt>();
        idle = GetComponent<PeipeiIdle>();
        TransState(EPeipeiState.Idle);
    }
    public void TransState(EPeipeiState state)
    {
        IState newState = state switch
        {
          
            EPeipeiState.Chase => this.chase,

            EPeipeiState.Attack => this.attack,
            EPeipeiState.Die => this.die,
            EPeipeiState.GetHurt => this.getHurt,
            EPeipeiState.Idle => this.idle,
            _ => this.idle
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
