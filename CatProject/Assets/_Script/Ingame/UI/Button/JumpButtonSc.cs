using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButtonSc : InputButtonManager
{
    private void Update()
    {
        JumpButton();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        playerSc.jumpButtonOn = true;
        playerSc.Jump();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        playerSc.jumpButtonOn = false;
    }

    public void JumpButton()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerSc.jumpButtonOn = true;
            playerSc.Jump();
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            playerSc.jumpButtonOn = false;
        }
                
    }
}
