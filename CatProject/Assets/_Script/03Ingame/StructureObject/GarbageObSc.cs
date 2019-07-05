using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageObSc : StructureObject
{
    int hp = 0;
    public ItemSc item;

    protected override void Awake()
    {
        item = Instantiate(LoadDataManager.instance.itemPrefabDic[Item_TYPE.BigCatnip.ToString()], new Vector3(transform.position.x,transform.position.y+1f, 0), Quaternion.identity);
        item.gameObject.SetActive(false);
        base.Awake();
        StateOn = true;
        hp = spriteArray.Length-1;
    }

    public override void TakeThis()
    {
        if (!StateOn)
            return;
        hp -= 1;
        ownSpRenderer.sprite = spriteArray[hp];
        
        if(hp<=0)
        {
            StateOn = false;
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().simulated = false;

            EffectOb.SetActive(true);

            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 25f, ForceMode2D.Impulse);
        }
        
        //..애니매이션 실행
        //..기본 스프라이트 이미지 변경
    }
}
