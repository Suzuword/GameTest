using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatChase : MonoBehaviour, IState
{
    public Rigidbody2D rb;
    public CatState state;
    private Vector2 vector2;
    int dirX;
    public float chaseSpeed;
    void IState.OnEnter()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    void IState.OnExit()
    {
        
    }

    void IState.OnKeep()
    {
       if(transform.position.x>state.Player.position.x)
        { vector2 = new Vector2(-1, 0); }
        if (transform.position.x < state.Player.position.x)
        { vector2 = new Vector2(1, 0); }

        rb.velocity = vector2 * chaseSpeed;

        if(Vector2.Distance(transform.position,state.Player.position)>5f)
        { state.TransState(NormalCatState.HaWhenFailedChase); }
    }

 
}
