using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : CharacterHealth
{

    public override void GetHurt(Attack attacker)
    {
       
        health = health - attacker.Damage;
    }
}
