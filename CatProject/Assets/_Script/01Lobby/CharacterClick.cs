using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterClick : InputButton
{
    public PLAYER_TYPE pType;
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
