using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantCatHealth :CharacterHealth
{
    GiantCatState state;
   protected override void Start()
    {
        base.Start();
        state =GetComponent<GiantCatState>();
    }

    // Update is called once per frame
    public override void GetHurt(Attack attacker) {
        if (state.currentState != state.die)
        {
            base.GetHurt(attacker);
            if (health > 0)
            {
                state.TransState(EGiantCatState.GetHurt);

            }
            if (health <= 0)
            {
                state.TransState(EGiantCatState.Die);

            }
        }

    }
}
