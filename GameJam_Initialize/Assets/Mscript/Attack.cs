using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float Damage;
    public float knockBackForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<CharacterHealth>()?.GetHurt(this);
    }
   
}
