using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FairyGUI.RTLSupport;

public class PeipeiHealth : CharacterHealth
{
    public PeipeiState peipeiState;

    protected override void Start()
    {
        base.Start();
        peipeiState=GetComponent<PeipeiState>();
    }
    public override void GetHurt(Attack attacker)
    {
        base.GetHurt(attacker);
        if (health > 0)
        { peipeiState.TransState(EPeipeiState.GetHurt); }
        if (health <= 0)
        { peipeiState.TransState(EPeipeiState.Die); }
    }
}
