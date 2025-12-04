using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPhycicsCheck : MonoBehaviour
{
    public float CheckRidus;
    public LayerMask ground;
    public Vector2 LeftfootOffset;
    public Vector2 RightfootOffset;

    public bool isleftground;
    public bool isrightground;
    private void Update()
    {
       isleftground= Physics2D.OverlapCircle(this.transform.position+(Vector3)LeftfootOffset, CheckRidus, ground);
       isrightground= Physics2D.OverlapCircle(this.transform.position + (Vector3)RightfootOffset, CheckRidus, ground);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position + (Vector3)LeftfootOffset, CheckRidus);
        Gizmos.DrawSphere(this.transform.position + (Vector3)RightfootOffset, CheckRidus);
    }
}
