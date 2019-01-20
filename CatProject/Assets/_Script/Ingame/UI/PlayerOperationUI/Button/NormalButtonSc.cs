using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalButtonSc : PlayerActionButton
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
        base.ButtonClickDown();
        playerSc.attackButtonPress = true;
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
        playerSc.attackButtonPress = false;
    }
}
