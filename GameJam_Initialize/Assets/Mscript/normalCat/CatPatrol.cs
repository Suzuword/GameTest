using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatPatrol : MonoBehaviour, IState
{
    CatState catState;
    Transform currentTarget;
    int currentIndex;
    private void Start()
    {
        catState=GetComponent<CatState>();
        currentTarget = catState.patorlPos[0].transform;
    }
    public void OnEnter()
    {
       //²¥·ÅÑ²Âß¶¯»­

    }

    public void OnExit()
    {
        
    }

    public void OnKeep()
    {
        
        transform.position = Vector2.MoveTowards(transform.position,currentTarget.transform.position,1f*Time.deltaTime);
       if (Vector2.Distance(currentTarget.transform.position, transform.position) < 0.1f)
        { AddPatrolIndex(); }
    }
    public void AddPatrolIndex()
    {
        currentIndex++; 
        if(currentIndex>=catState.patorlPos.Length)
        { currentIndex = 0; }
        currentTarget = catState.patorlPos[currentIndex].transform;
       
    }
}
