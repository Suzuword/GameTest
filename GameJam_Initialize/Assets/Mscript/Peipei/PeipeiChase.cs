using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;

public class PeipeiChase : MonoBehaviour,IState
{
    Animator animator;
    private Vector2 vector2;
    PeipeiState state;
    Rigidbody2D rb;
    public float chaseSpeed;
   void Start()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<PeipeiState>();
        rb=GetComponent<Rigidbody2D>();
    }
    public void OnEnter()
    {
        animator.Play("PeiPeiWalk");
    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {

        if (transform.position.x > state.Player.position.x)
        {
            vector2 = new Vector2(-1, 0);
            this.gameObject.transform.localScale = new Vector2(-1, 1);
        }
        if (transform.position.x < state.Player.position.x)
        {
            vector2 = new Vector2(1, 0);
            this.gameObject.transform.localScale = new Vector2(1, 1);
        }

        rb.velocity = vector2 * chaseSpeed;
        if(state.canAttack)
        { state.TransState(EPeipeiState.Attack); }
    }

    // Start is called before the first frame update
    

   
}
