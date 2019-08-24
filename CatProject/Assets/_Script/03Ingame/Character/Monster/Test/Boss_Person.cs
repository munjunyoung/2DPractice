using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Person : BossMonsterController
{
    private void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    public override void IdleAction()
    {
        currentMoveSpeed = 0;
    }

    public override void PatrolAction()
    {
        throw new System.NotImplementedException();
    }

    public override void ChaseAction()
    {
        throw new System.NotImplementedException();
    }

    public override void StartAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void StopAttack()
    {
        throw new System.NotImplementedException();
    }

    public override bool CheckCloseTarget()
    {
        throw new System.NotImplementedException();
    }

    public override void SkillAction()
    {
        throw new System.NotImplementedException();
    }

    public override void DeadProcess()
    {
        throw new System.NotImplementedException();
    }

    public override void isDie()
    {
        throw new System.NotImplementedException();
    }

    public override bool CheckDetectTarget()
    {
        throw new System.NotImplementedException();
    }
}
