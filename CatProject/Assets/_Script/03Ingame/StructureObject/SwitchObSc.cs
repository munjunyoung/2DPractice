using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchObSc : MonoBehaviour
{
    public DungeonRoom ownRoom = null;
    public bool SwitchOn = false;
    public Sprite[] switchImage;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            GetComponent<SpriteRenderer>().sprite = switchImage[1];
            SwitchOn = true;
            ownRoom.CheckLockRoom();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            GetComponent<SpriteRenderer>().sprite = switchImage[0];
            SwitchOn = false;
            ownRoom.CheckLockRoom();
        }
        
    }
}


