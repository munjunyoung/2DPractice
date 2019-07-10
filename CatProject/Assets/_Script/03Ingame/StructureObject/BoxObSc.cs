using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObSc : Rb2dStructureSc
{
    private Vector2 startpos;

    protected override void Awake()
    {
        base.Awake();
        startpos = transform.localPosition;
    }

    ///// <summary>
    ///// NOTE : 공격받음
    ///// </summary>
    //public override void TakeThis()
    //{
    //    if (!StateOn)
    //        return;
    //    hp -= 1;

    //    if (hp > 0)
    //        ownSpRenderer.sprite = spriteArray[hp - 1];
    //    else
    //        StartCoroutine(DestroyObjectByCount(2f));
    //}

    protected override void SetWhenHPzero()
    {
        //base.SetWhenHPzero();
        StartCoroutine(DestroyObjectByCount(2f));
    }
    /// <summary>
    /// NOTE : 2초후 실행
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnumerator DestroyObjectByCount(float count)
    {
        StateOn = false;
        rb2d.velocity = Vector2.zero;
        ownSpRenderer.sprite = null;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<BoxCollider2D>().isTrigger = true;
        ownSpRenderer.sprite = null;
        yield return new WaitForSeconds(count);
        transform.localPosition = startpos;
        transform.localRotation = Quaternion.identity;
        hp = spriteArray.Length;
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<BoxCollider2D>().isTrigger = false;

        ownSpRenderer.sprite = spriteArray[hp - 1];
        StateOn = true;
    }
    
}
