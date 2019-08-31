using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill
{
    public BossMonsterController bossController;
    protected Color characterColor;
    protected Color ChangeColor;
    protected virtual void Start()
    {

    }
}

public class FrenzySkillBoss : BossSkill
{
    
    protected override void Start()
    {
        base.Start();
        characterColor = Color.white;
        characterColor = Color.red;
        //bossController.
    }

    
}
