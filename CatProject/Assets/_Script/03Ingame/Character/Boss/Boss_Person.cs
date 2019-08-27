using System.Collections;
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
        rb2D.velocity = Vector2.zero;
    }

    public override bool DetectTarget()
    {
        var overlapCollider = Physics2D.OverlapCircle(transform.position, 5f);
        if (overlapCollider.CompareTag("Player"))
            aleadyFindTarget = true;
        return aleadyFindTarget;
    }
    #endregion

    #region ChaseAttack
    public override void ChaseAction()
    {
        currentMoveSpeed = bData.normalSpeed;
        var dirX = Mathf.Sign(TargetOB.position.x - transform.position.x);
        sR.flipX = dirX >= 0 ? false : true;

        rb2D.velocity = new Vector2(dirX * currentMoveSpeed, rb2D.velocity.y);
    }

    public override bool CheckCloseTarget()
    {
        if (checkCloseTarget)
            return true;
        return false;
    }

    public override bool CheckPossibleAttack()
    {
        return true;
    }

    public override void StartAttack()
    {
        rb2D.velocity = Vector2.zero;
        //..AttackStart
    }
    #endregion

    #region Skill
    public override bool CheckPossibleSkill()
    {
        //..

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            checkCloseTarget = true;
        }
    }
}
