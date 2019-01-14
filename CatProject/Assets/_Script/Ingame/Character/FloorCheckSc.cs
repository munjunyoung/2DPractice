using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCheckSc : MonoBehaviour
{
    [HideInInspector]
    public bool isGrounded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
            isGrounded = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
            isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
            isGrounded = false;
    }
}
