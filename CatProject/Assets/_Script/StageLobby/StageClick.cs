using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClick : InputButton
{
    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        StageSelectSceneManager.instance.ShowSelectStageAlarmPanel(1);
    }
}
