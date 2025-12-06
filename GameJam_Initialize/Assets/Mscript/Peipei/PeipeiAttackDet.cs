using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeipeiAttackDet : MonoBehaviour
{
    public PeipeiState peipeiState;

    private void Start()
    {
        peipeiState=GetComponentInParent<PeipeiState>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            peipeiState.canAttack = true;
        }
        if(peipeiState.currentState==peipeiState.chase)
        { peipeiState.TransState(EPeipeiState.Attack); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            peipeiState.canAttack = false;
        }
    }
}
