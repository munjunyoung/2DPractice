using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWallCheckSc : MonoBehaviour
{
    [HideInInspector]
    public bool isHitWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            isHitWall = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            isHitWall = true;
        
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
            isHitWall = false;
    }
}
