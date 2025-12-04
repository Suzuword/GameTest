using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaDetection : MonoBehaviour
{
    
    public CatState state;
    public Transform target;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (state.currentState!=state.chase)
            { state.TransState(NormalCatState.Ha);
                target = collision.transform;
                Debug.Log("ha!"); }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            if (state.currentState==state.ha)
            state.TransState(NormalCatState.Patorl);
        }
    }
}
