using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantCatAttackDetec : MonoBehaviour
{
    GiantCatState catState;
    void Start()
    {
        catState=GetComponentInParent<GiantCatState>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        { 
            catState.canAttack = true;
            catState.TransState(EGiantCatState.Attack); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            catState.canAttack = false;
           
        }
    }
}
