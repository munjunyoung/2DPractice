using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// NOTE : CAT TYPE1 캐릭터 
/// </summary>
public class CatType1 : Player
{
    
    protected override void Awake()
    {
        base.Awake();
        CSVDataReader.instance.SetData(pDATA, PLAYER_TYPE.Cat1.ToString());
        mySkill = gameObject.AddComponent<SkillAttackUP>();
    }

    protected override void Start()
    {
        CurrentHP = pDATA.maxHP;
        CurrentTP = pDATA.maxTP;
    }
}


