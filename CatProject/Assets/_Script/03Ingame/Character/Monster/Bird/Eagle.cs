using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : FlyingMonster
{
 
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        mType = MONSTER_TYPE.Eagle;
    }
}
