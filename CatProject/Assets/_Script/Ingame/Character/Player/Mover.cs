using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2((move) * 30, rb.velocity.y);
    }

    private void OnCollisionStay2D(Collision2D coll)
    {
        Vector2 normalSum = Vector2.zero;
        foreach (var p in coll.contacts)
            normalSum += p.normal;

        normalSum.Normalize();
        if (normalSum.y > 0)
            isFalling = false;
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        isFalling = true;
    }

    void Update()
    {
        if (!isFalling && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, 25);
        }
    }
}
