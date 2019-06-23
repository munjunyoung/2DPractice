using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchObSc : MonoBehaviour
{
    public DungeonRoom ownRoom = null;
    public bool SwitchOn = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            SwitchOn = true;
            ownRoom.CheckLockRoom();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            SwitchOn = false;
            ownRoom.CheckLockRoom();
        }
        
    }
}
