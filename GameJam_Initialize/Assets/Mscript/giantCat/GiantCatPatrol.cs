using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantCatPatrol : MonoBehaviour,IState
{
    GiantCatState catState;
    Transform currentTarget;
    int currentIndex;
    public float speed;
    private void Start()
    {
        catState = GetComponent<GiantCatState>();
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

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime);
        if (Vector2.Distance(currentTarget.transform.position, transform.position) < 0.1f)
        { AddPatrolIndex(); }
    }
    public void AddPatrolIndex()
    {
        currentIndex++;
        if (currentIndex >= catState.patorlPos.Length)
        { currentIndex = 0; }
        currentTarget = catState.patorlPos[currentIndex].transform;

    }

}
