using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatAttackArea : MonoBehaviour
{
    public GameObject AttackArea;



    private void Update()
    {
        if( Input.GetMouseButtonDown(0))
        { CreAttackArea(); }
    }
    void CreAttackArea()
    {
       AttackArea.SetActive(true);

        Invoke("DestroyArea", 0.2f);

    }
    void DestroyArea()
    { AttackArea.SetActive(false); }
}
