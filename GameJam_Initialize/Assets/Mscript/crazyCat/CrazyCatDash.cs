using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyCatDash : MonoBehaviour,IState
{
    public Transform player;
    Rigidbody2D rigidbody2;
    int dashDirX;
    public float dashSpeed;
    public FloorPhycicsCheck floorPhycics;
    public void OnEnter()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
       if(player.transform.position.x>this.transform.position.x)
        { dashDirX = 1; 
        }
       if(player.transform.position.x<this.transform.position.x)
            { dashDirX=-1; }
       floorPhycics = GetComponent<FloorPhycicsCheck>();
    }

    public void OnExit()
    {
       
    }

    public void OnKeep()
    {
        if(dashDirX==-1&&floorPhycics.isleftground==false)
        { dashDirX = 1;  }
        if (dashDirX == 1 && floorPhycics.isrightground == false)
        { dashDirX = -1; }
        this.rigidbody2.velocity=new Vector2(dashSpeed*dashDirX, rigidbody2.velocity.y);
    }

 
}
