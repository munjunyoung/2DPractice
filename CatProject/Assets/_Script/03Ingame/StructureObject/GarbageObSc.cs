using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageObSc : Rb2dStructureSc
{
    public ItemSc item;

    protected override void Awake()
    {
        base.Awake();
        item = Instantiate(LoadDataManager.instance.itemPrefabDic[Item_TYPE.BigCatnip.ToString()], new Vector3(transform.position.x, transform.position.y + 2f, 0), Quaternion.identity);
        item.gameObject.SetActive(false);
    }

    //public override void TakeThis()
    //{
    //    base.TakeThis();


      
        
    //    //..애니매이션 실행
    //    //..기본 스프라이트 이미지 변경
    //}

    protected override void SetWhenHPzero()
    {
        //base.SetWhenHPzero();
        if (hp > 0)
            sR.sprite = spriteArray[hp];
        else
        {
            StateOn = false;
            //GetComponent<BoxCollider2D>().isTrigger = true;
            //GetComponent<Rigidbody2D>().simulated = false;

            EffectOb.SetActive(true);

            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 25f, ForceMode2D.Impulse);
        }
    }
}
