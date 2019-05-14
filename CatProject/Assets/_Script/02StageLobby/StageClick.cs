using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageClick : InputButton
{
    [SerializeField]
    private Sprite openImage,closeImage;
    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        GetComponent<Image>().sprite = openImage;
        StageSelectSceneManager.instance.ShowSelectStageAlarmPanel(1);
    }

    /// <summary>
    /// NOTE : Door 이미지 전환
    /// </summary>
    public void CloseDoorImage()
    {
        if(closeImage!=null)
            GetComponent<Image>().sprite = closeImage;
    }
}
