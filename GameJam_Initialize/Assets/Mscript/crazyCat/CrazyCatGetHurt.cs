using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyCatGetHurt : MonoBehaviour,IState
{
    CrazyCatState state;
    public void OnEnter()
    {
        state = GetComponent<CrazyCatState>();
        Invoke("ToChase", 1f);
    }

    public void OnExit()
    {
      
    }

    public void OnKeep()
    {
       
    }
    private void ToChase()
    { state.TransState(ECrazyCatState.Dash); }
   
}
