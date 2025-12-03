using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyCatDie : MonoBehaviour,IState
{
    public void OnEnter()
    {
        Invoke("destory", 2f);
    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {
        
    }
    void destory()
    {
       Destroy(gameObject);
    }
    
}
