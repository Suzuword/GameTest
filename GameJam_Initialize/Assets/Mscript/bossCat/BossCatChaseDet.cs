using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCatChaseDet : MonoBehaviour
{
   BossCatState state;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(state.currentState==state.idle)
            { state.TransState(EBossCatState.Chase); }
        }
    }
}
