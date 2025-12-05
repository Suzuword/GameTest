using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatDie : MonoBehaviour,IState
{
    Animator animator;
    CatState state;
    private float destroyTime = 1f;
    SpriteRenderer[] srs;
    Collider2D collider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<CatState>();
        srs = GetComponentsInChildren<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }
    public void OnEnter()
    {

        animator.Play("smallMaoDieDead");
    }

    public void OnExit()
    {

    }

    public void OnKeep()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.99f)
        {
            //Invoke("DestroyObject", 1f); 
            //使用协程
            StartCoroutine(SlowlyDestroyObject());
        }
    }
    void DestroyObject()
    {
        Destroy(gameObject);
    }

    IEnumerator SlowlyDestroyObject()
    {
        for(float i = 0.05f,total = 1f; 0 < total; total -= i)
        {
            for (int p = 0; p < srs.Length; p++)
            {
                srs[p].color = new Color(srs[p].color.r, srs[p].color.g, srs[p].color.b, total);
            }
            yield return new WaitForSeconds(i/2);
        }
        Destroy(this.gameObject);
    }
}
