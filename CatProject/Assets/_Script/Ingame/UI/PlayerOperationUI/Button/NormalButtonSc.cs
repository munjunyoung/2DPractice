using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalButtonSc : PlayerButtonManual
{
    protected override void SetKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            ButtonClickDown();

        if (Input.GetKeyUp(KeyCode.LeftControl))
            ButtonClickUp();
    }

    protected override void ButtonClickDown()
    {
        if (playerSc.allStop)
            return;
        base.ButtonClickDown();
        playerSc.AttackButtonOn = true;
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
        playerSc.AttackButtonOn = false;
    }
}
