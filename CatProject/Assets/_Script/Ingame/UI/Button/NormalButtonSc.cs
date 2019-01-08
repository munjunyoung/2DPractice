using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalButtonSc : InputButtonManager
{
    private void Update()
    {
        NormalButton();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {

        playerSc.Attack();
    }
    

    public void NormalButton()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            playerSc.Attack();
    }

}
