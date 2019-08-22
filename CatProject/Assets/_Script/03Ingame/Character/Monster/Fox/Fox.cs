using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : CloseRangeMonster
{
    protected override void Awake()
    {
        base.Awake();
        mType = MONSTER_TYPE.Fox;
    }
}
