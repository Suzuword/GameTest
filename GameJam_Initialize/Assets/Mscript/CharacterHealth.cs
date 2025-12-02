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

    protected void Start()
    {
        health=maxHealth;
        animator = GetComponent<Animator>();
        rb= GetComponent<Rigidbody2D>();
    }
 
    public virtual void GetHurt(Attack attacker)
    {
        //播放受击动画
        health=health-attacker.Damage;
        //击退效果
        Vector2 backDir = this.transform.position - attacker.transform.position;
        if (backDir.magnitude > 0.01f)
            backDir = backDir.normalized;
        rb.AddForce(backDir*attacker.knockBackForce,ForceMode2D.Impulse);
    }

}
