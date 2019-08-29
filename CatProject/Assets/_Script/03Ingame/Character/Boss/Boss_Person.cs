﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Person : BossMonsterController
{

    public override void Init()
    {
        base.Init();
        bType = BOSS_TYPE.Person;
    }

    #region Normal
    public override void IdleAction()
    {
        if (isAleadyFindingTarget)
            return;

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }

    public override bool DetectTarget()
    {
        if (isAleadyFindingTarget)
            return true;

        var overlapCollider = Physics2D.OverlapCircle(transform.position, 5f);
        if (overlapCollider.CompareTag("Player"))
            isAleadyFindingTarget = true;
        return isAleadyFindingTarget;
    }
    #endregion

    #region ChaseAttack
    public override void ChaseAction()
    {
        currentMoveSpeed = bData.normalSpeed;
        var dirX = Mathf.Sign(TargetOB.position.x - transform.position.x);
        sR.flipX = dirX >= 0 ? true : false;

        if(!isCloseTarget)
            rb2D.velocity = new Vector2(dirX * currentMoveSpeed, rb2D.velocity.y);
    }
    
    public override bool CheckCloseTarget()
    { 
        return isCloseTarget;
    }
    
    public override bool CheckPossibleAttack()
    {
        return isReadyAttack;
    }

    public override void StartAttack()
    {
        rb2D.velocity = Vector2.zero;
        isStartingAttack = true;
        //..AttackStart
    }
    #endregion

    #region Skill
    public override bool CheckPossibleSkill()
    {
        return false;
    }

    public override void SkillAction()
    {

    }
    #endregion

    #region Dead

    public override bool isDie()
    {
        return false;
    }

    public override void StopAttack()
    {

    }

    public override void DeadProcess()
    {

    }



    #endregion

   
}
