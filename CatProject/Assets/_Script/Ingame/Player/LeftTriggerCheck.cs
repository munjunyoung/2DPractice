using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftTriggerCheck : MonoBehaviour
{
    public bool leftCheck;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            leftCheck = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            leftCheck = false;
        }
    }

}
