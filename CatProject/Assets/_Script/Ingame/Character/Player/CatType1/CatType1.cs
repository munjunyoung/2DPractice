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
        mySkill = gameObject.AddComponent<SkillAttackUP>();
    }

    private void Start()
    {
        CSVDataReader.instance.SetData(pDATA, PLAYER_TYPE.Cat1.ToString());
        
        CurrentHP = pDATA.maxHP;
        CurrentTP = pDATA.maxTP;
    }
}


