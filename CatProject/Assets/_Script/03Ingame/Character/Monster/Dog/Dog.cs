using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Player
{
    protected override void Awake()
    {
        base.Awake();
        CSVDataReader.instance.SetData(pDATA, PLAYER_TYPE.Dog.ToString());
        mySkill = gameObject.AddComponent<SkillAttackUP>();
    }
}
