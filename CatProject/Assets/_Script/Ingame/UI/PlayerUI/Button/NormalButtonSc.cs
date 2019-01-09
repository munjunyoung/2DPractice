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
            ButtonClickOn();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        targetModel.AttackButtonOn = true;
    }

    protected override void ButtonClickOn()
    {
        base.ButtonClickOn();
        targetModel.AttackButtonOn = false;
    }
}
