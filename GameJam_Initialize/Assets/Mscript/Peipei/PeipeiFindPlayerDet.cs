using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeipeiFindPlayerDet : MonoBehaviour
{
    public  PeipeiState state;
    private void Start()
    {
        state=GetComponentInParent<PeipeiState>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            state.TransState(EPeipeiState.Chase);
        }
    }
}
