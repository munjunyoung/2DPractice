using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButtonSc : PlayerButtonManual
{
    private void Update()
    {
        ButtonOn();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        playerSc.JumpButtonOn = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        playerSc.JumpButtonOn = false;
    }


    public override void ButtonOn()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerSc.JumpButtonOn = true;
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            playerSc.JumpButtonOn = false;
        }
    }
}
