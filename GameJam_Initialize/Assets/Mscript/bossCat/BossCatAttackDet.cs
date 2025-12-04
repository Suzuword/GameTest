using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCatAttackDet : MonoBehaviour
{
    public BossCatState State;
    private void Start()
    {
        State=GetComponentInParent<BossCatState>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            State.canAttack = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
       
    
        if(collision.CompareTag("Player"))
        {
            State.canAttack = false;
        }
    
    }
}
