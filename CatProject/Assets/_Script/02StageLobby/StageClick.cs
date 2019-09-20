using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageClick : InputButton
{
    public int level;
    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        Debug.Log(level);
        StageSelectSceneManager.instance.ShowSelectStageAlarmPanel(level);
    }


    protected override void ButtonClickUp() { return; }

    public void SetOriginalColor()
    {
        currentColor.a = 1f;
        modelObjectImage.color = currentColor;
    }
}
