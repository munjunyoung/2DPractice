using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageClick : InputButton
{
    
    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
       
        StageSelectSceneManager.instance.ShowSelectStageAlarmPanel(1);

    }


    protected override void ButtonClickUp() { return; }

    public void SetOriginalColor()
    {
        currentColor.a = 1f;
        modelObjectImage.color = currentColor;
    }
}
