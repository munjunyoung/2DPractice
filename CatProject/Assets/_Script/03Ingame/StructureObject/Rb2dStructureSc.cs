using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rb2dStructureSc : StructureObject
{
    protected int hp = 0;
    protected Rigidbody2D rb2d;

    protected override void Awake()
    {
        base.Awake();
        rb2d = GetComponent<Rigidbody2D>();
        StateOn = true;
        hp = spriteArray.Length;
    }

    public virtual void TakeThis(Vector3 pos)
    {
        KnockBack(pos);
        if (!StateOn)
            return;
        hp -= 1;
        if (hp > 0)
            sR.sprite = spriteArray[hp-1];
        else
            SetWhenHPzero();
    }

    protected virtual void SetWhenHPzero() { }

    protected virtual void KnockBack(Vector3 hitpos)
    {
        var dirvector = transform.position - hitpos;
        dirvector = dirvector.normalized;
        rb2d.AddForce(dirvector*100f, ForceMode2D.Impulse);
    }
    
}
