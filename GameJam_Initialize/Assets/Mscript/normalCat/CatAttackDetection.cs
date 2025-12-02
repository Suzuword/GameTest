using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttackDetection : MonoBehaviour
{
    CatState state;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("player"))
        { state.TransState(NormalCatState.Attack); }
    }
}
