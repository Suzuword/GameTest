using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyCatHealth : CharacterHealth
{
   public CrazyCatState state;
  public override void GetHurt(Attack attacker)
    {
        
        if (state.currentState != state.die)
        {
            base.GetHurt(attacker);
            if (health > 0)
            {
                state.TransState(ECrazyCatState.GetHurt);
               
            }
            if (health <= 0)
            {
                state.TransState(ECrazyCatState.Die);
               
            }
        }
    }
}
