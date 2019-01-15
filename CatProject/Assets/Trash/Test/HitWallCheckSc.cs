using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWallCheckSc : MonoBehaviour
{
    [HideInInspector]
    public bool isHitWall = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter : " + collision.tag);
        if (collision.CompareTag("Ground"))
            isHitWall = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit : " + collision.tag);
        if (collision.CompareTag("Floor"))
            isHitWall = false;
    }
}
