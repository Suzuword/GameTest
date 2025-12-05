using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Attack
{
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            collision.GetComponent<CharacterHealth>().GetHurt(this);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
