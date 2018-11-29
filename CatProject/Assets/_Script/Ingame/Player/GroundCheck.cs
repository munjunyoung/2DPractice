using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
             player.isGrounded  = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            player.isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag=="Ground")
        {
            player.isGrounded = false;
        }
    }
}
