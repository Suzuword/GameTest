using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public Animator animator;
    [SerializeField] Transform player;
    Rigidbody2D rb;

    protected virtual void Start()
    {
        health=maxHealth;
        animator = GetComponent<Animator>();
        rb= GetComponent<Rigidbody2D>();
    }
 
    public virtual void GetHurt(Attack attacker)
    {
        
        health=health-attacker.Damage;
        if (health > 0)
        {//播放受击动画
         //击退效果
            Vector2 backDir = this.transform.position - attacker.gameObject.transform.parent.transform.position;
            if (backDir.magnitude > 0.01f)
                backDir = backDir.normalized;
            rb.AddForce(backDir * attacker.knockBackForce, ForceMode2D.Impulse);
        }
        if (health <= 0) { 
        //播放死亡动画
        
        }
    }

}
