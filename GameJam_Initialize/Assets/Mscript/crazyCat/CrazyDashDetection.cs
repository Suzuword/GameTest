using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyDashDetection : MonoBehaviour
{
    [SerializeField]CrazyCatState catState;
    public float dashDuration;
    public float dashTime;
    bool startCount;
    private void Start()
    {
        dashTime = dashDuration;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && catState.currentState != catState.dash)
        { catState.TransState(ECrazyCatState.Dash);
            dashTime = dashDuration;
            startCount=false;
        }
        if (collision.CompareTag("Player") && catState.currentState == catState.dash)
        {
            
            dashTime = dashDuration;
            startCount=false ;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
         if (collision.CompareTag("Player") && catState.currentState == catState.dash)
        { startCount = true; }
    }
    private void Update()
    {
        if (startCount&&dashTime>0) { dashTime -= Time.deltaTime; }
        if (dashTime <= 0) { dashTime = dashDuration;
        catState.TransState(ECrazyCatState.Patrol);
            startCount = false;
        }
    }

}
