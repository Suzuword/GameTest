using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECrazyCatState {Patrol,Dash,GetHurt,Die, }
public class CrazyCatState : MonoBehaviour
{
    NormalCatState catState;
    public GameObject[] patorlPos;
    public IState currentState;
    public Transform Player;

    IState patrol;
   public IState dash;
    IState getHurt;
    IState die;

    private void Start()
    {
        patrol = GetComponent<CrazyCatPatrol>();
        dash= GetComponent<CrazyCatDash>();
        die = GetComponent<CrazyCatDie>();
        getHurt= GetComponent<CrazyCatGetHurt>();
        this.TransState(ECrazyCatState.Dash);
    }
    public void TransState(ECrazyCatState state)
    {
        IState newState = state switch
        {
            ECrazyCatState.Patrol => this.patrol,
            ECrazyCatState.Dash => this.dash,
            ECrazyCatState.Die => this.die,
            ECrazyCatState.GetHurt => this.getHurt,
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
