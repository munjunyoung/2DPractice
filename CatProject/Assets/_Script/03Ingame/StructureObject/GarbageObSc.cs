using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageObSc : StructureObject
{
    int hp = 0;
    protected override void Awake()
    {
        base.Awake();
        StateOn = true;
        hp = spriteArray.Length;
    }

    private void TakeOpen()
    {
        if (!StateOn)
            return;
        hp -= 1;

        if (hp > 0)
        {
            ownSpRenderer.sprite = spriteArray[hp - 1];
        }
        else
        {
            StateOn = false;
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().simulated = false;
        }

     
        //..애니매이션 실행
        //..기본 스프라이트 이미지 변경
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackEffect"))
            TakeOpen();
    }
}
