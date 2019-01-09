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
            ButtonClickOn();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        targetModel.JumpButtonOn = true;
    }

    protected override void ButtonClickOn()
    {
        base.ButtonClickOn();
        targetModel.JumpButtonOn = false;
    }
}
