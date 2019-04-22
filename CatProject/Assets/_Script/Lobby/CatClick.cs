using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatClick : InputButton
{
    private PLAYER_TYPE pType = PLAYER_TYPE.Cat1;
    

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        LobbyManager.instance.ActiveSelectCompletePanel(pType);
    }
}
