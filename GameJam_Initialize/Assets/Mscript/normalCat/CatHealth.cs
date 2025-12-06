using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CatHealth : CharacterHealth
{
    [SerializeField] CatState catState;

    protected override void Start()
    {
        catState=GetComponent<CatState>();
    }
    public override void GetHurt(Attack attacker)
    {
        base.GetHurt(attacker);
       if(health>0)
        { catState.TransState(NormalCatState.GetHurt); }
       if(health<=0)
        { catState.TransState(NormalCatState.Die); }
    }


}
