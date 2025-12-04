using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : CharacterHealth
{
    // Start is called before the first frame update
    BossCatState catState;

    protected override void Start()
    {
        base.Start();
        catState = GetComponent<BossCatState>();
    }
    public override void GetHurt(Attack attacker)
    {
        health = health - attacker.Damage;
        if (health <= 0)
        {
            //²¥·ÅËÀÍö¶¯»­
            catState.TransState(EBossCatState.Die);
            
        }
    }
}
