using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatChaseDet : MonoBehaviour
{
    public CatState catState;
    private void Start()
    {
        catState=GetComponentInParent<CatState>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&catState.currentState==catState.patrol)
        {
            catState.TransState(NormalCatState.Chase);
        }
    }
}
