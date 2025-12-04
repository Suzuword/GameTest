using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BossCatChase : MonoBehaviour,IState
{
    private Vector2 vector2;
    private BossCatState state;
    public float chaseSpeed;
    public Rigidbody2D rb;
    public void OnEnter()
    {
        state=GetComponent<BossCatState>();
        rb= GetComponent<Rigidbody2D>();
    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {
        if (transform.position.x > state.Player.position.x)
        { vector2 = new Vector2(-1, 0); }
        if (transform.position.x < state.Player.position.x)
        { vector2 = new Vector2(1, 0); }

        rb.velocity = vector2 * chaseSpeed;
        if(state.canAttack&& state.currentState == state.chase)
        { state.TransState(EBossCatState.Attack); }
    }

   
}
