using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatClick : InputButton
{
    private PLAYER_TYPE pType = PLAYER_TYPE.Cat1;
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        anim.SetTrigger("Click");
        LobbyManager.instance.ActiveSelectCompletePanel(pType);
    }
}
