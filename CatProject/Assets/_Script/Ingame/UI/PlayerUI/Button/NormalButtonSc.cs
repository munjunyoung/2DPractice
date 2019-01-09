using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NormalButtonSc : PlayerButtonManual
{
    private void Update()
    {
        ButtonOn();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        playerSc.AttackButtonOn = true;
        //playerSc.Attack();
    }
    
    public override void ButtonOn()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //playerSc.Attack();
            playerSc.AttackButtonOn = true;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            playerSc.AttackButtonOn = false;
        }
    }

}
