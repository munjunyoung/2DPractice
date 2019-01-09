using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButtonSc : PlayerButtonManual
{
    protected override void SetKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ButtonClickDown();

        if (Input.GetKeyUp(KeyCode.Space))
            ButtonClickUp();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        targetModel.JumpButtonOn = true;
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
        targetModel.JumpButtonOn = false;
    }
}
