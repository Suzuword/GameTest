using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Eshanx { min,middle,max}
public class Shanxian : MonoBehaviour
{
    public float cool;
    public float coolTime;
    public bool isCool;

    public Vector2 Destination;
    public Vector2 Offset ;
    public Vector2 RectSize;
    public float CheckRidus;
    bool canFlash;
    public Eshanx eshanx;
   
    
    [SerializeField] LayerMask ground;


    private void Start()
    {
        Destination = new Vector2(5, 0);
        transState(Eshanx.max);
    }

    private void Update()
    {
        if(isCool&&coolTime>0)
        { coolTime-=Time.deltaTime; }
        if(coolTime<=0)
        { coolTime = cool;isCool=false;}
       if( Input.GetKeyDown(KeyCode.F)&&!isCool)
            { Flash(); }
        canFlash = !Physics2D.OverlapBox((Vector2)this.gameObject.transform.position + Offset, RectSize, 0f, ground);
    }

    private void Flash()
    { if(canFlash)
      this.gameObject.transform.position = (Vector2)this.gameObject.transform.position + Destination;
      isCool=true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere((Vector2)this.gameObject.transform.position + Offset, CheckRidus);
        Gizmos.DrawSphere((Vector2)this.gameObject.transform.position + Destination, CheckRidus);
        Gizmos.DrawWireCube((Vector2)this.gameObject.transform.position + Offset, RectSize);
    }
    public void transState(Eshanx target)
    { eshanx = target;
    if(target==Eshanx.max)
        { cool = 2; }
    if (target==Eshanx.middle)
        { cool = 3; }
    if(target == Eshanx.min)
        { cool = 1;
        //减少体力代码
        }
    
    }
}
