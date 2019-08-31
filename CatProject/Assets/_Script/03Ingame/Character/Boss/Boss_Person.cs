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
        if (isAleadyFindingTarget)
            return;

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }

    public override bool DetectTarget()
    {
        if (isAleadyFindingTarget)
            return true;
        
        var overlapCollider = Physics2D.OverlapCircle(transform.position, 7f);
        if (overlapCollider.CompareTag("Player"))
            isAleadyFindingTarget = true;
        return isAleadyFindingTarget;
    }
    #endregion

    #region ChaseAttack
    public override void ChaseAction()
    {
        //공격중일때 이동하지 않게 하기 위함
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;

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
        if (isFrenzyState)
            return false;

        if (CurrentHP <= (bData.HP * 0.3f))
        {
            isFrenzyState = true;
            return true;
        }
        return false;
    }

    public override void SkillAction()
    {
        FrenzyAction();
    }
    #endregion

    #region Dead

    public override bool isDie()
    {
       return base.isDie();
    }
    
    public override void DeadAction()
    {
        StartCoroutine(DeadProcess());
    }
    #endregion

    protected bool isFrenzyState = false;
    protected void FrenzyAction()
    {
        sR.color = Color.red;
        //속도
        anim.SetFloat("MoveSpeed", bData.normalSpeed);
        //공속
        anim.SetFloat("AttackSpeed", bData.attackSpeed);
        //공격 쿨타임
        bData.attackCooltime = bData.attackCooltime * 0.5f;
        //공격력
        bData.attackDamage = bData.attackDamage + 20;
        //공격 범위
        attackEffect.transform.localScale = Vector2.one * 1.5f;
        
    }
   
}
