using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchObSc : StructureObject
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            sR.sprite = spriteArray[1];
            StateOn = true;
            ownRoom.CheckLockRoom();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            sR.sprite = spriteArray[0];
            StateOn = false;
            ownRoom.CheckLockRoom();
        }
        
    }
}


