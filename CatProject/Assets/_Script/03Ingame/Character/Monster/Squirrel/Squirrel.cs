using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squirrel : RemoteRangeMonster
{
    protected override void Awake()
    {
        base.Awake();
        mType = MONSTER_TYPE.Squirrel;
    }
}
