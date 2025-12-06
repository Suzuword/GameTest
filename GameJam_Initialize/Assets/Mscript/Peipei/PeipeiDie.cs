using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeipeiDie : MonoBehaviour,IState
{
    Animator animator;
    PeipeiState state;
    SpriteRenderer[] srs;
    private void Start()
    {
        animator = GetComponent<Animator>();
        state=GetComponent<PeipeiState>();
        srs = GetComponentsInChildren<SpriteRenderer>();
    }
    public void OnEnter()
    {
        animator.Play("PeiPeiDead");
    }

    public void OnExit()
    {
      
    }

    public void OnKeep()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.99f)
        {
           
            StartCoroutine(SlowlyDestroyObject());
        }
    }
    IEnumerator SlowlyDestroyObject()
    {
        for (float i = 0.05f, total = 1f; 0 < total; total -= i)
        {
            for (int p = 0; p < srs.Length; p++)
            {
                srs[p].color = new Color(srs[p].color.r, srs[p].color.g, srs[p].color.b, total);
            }
            yield return new WaitForSeconds(i / 2);
        }
        Destroy(this.gameObject);
    }


}
