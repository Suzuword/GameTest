using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float Damage;
    public float knockBackForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            
        collision.GetComponent<CharacterHealth>().GetHurt(this);
           
       }
    }
  void attackStart()
    {
        this.gameObject.SetActive(true);
    }
    void attackEnd()
    {
        this.gameObject.SetActive(false);
    }
}
