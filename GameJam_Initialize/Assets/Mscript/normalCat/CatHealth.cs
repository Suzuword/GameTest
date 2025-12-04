using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CatHealth : CharacterHealth
{
    [SerializeField] CatState catState;
   
  public override void GetHurt(Attack attacker)
    {
        base.GetHurt(attacker);
       if(health>0)
        { catState.TransState(NormalCatState.GetHurt); }
       if(health<=0)
        { catState.TransState(NormalCatState.Die); }
    }


}
