﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : WalkingMonster
{
    protected override void Awake()
    {
        base.Awake();
        mType = MONSTER_TYPE.Fox;
    }
}
