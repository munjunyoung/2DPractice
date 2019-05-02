using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : PlayerActionButton
{
    protected override void SetKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ButtonClickDown();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            ButtonClickUp();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        playerSc.OnSkill();
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
    }
}
