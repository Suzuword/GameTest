using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mplayer : MonoBehaviour
{

    public Rigidbody2D rb;
    public float speed;
    private Vector2 newVelocity;
    private void Update()
    {
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
    }
}
