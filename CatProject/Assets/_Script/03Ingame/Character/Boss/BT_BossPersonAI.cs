﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_BossPersonAI : BT_Base
{
    //최상위 
    private Sequence            aiRoot              = new Sequence();
    //선택
    private Sequence            seqNormal           = new Sequence();
    private Selector            selector            = new Selector();

    private Sequence            seqchaseAttack           = new Sequence();
    private Sequence            seqSkill           = new Sequence();
    private Sequence            seqDead             = new Sequence();
    //Normal
    private Idle                idle                = new Idle();
    private DetectTarget        detectTarget        = new DetectTarget();
    //Phase1
    private ChaseTarget         chaseTarget         = new ChaseTarget();
    private CheckCloseTarget    checkCloseTarget    = new CheckCloseTarget();
    private LookAtTarget        lookatTarget        = new LookAtTarget();
    private StartAttack         startAttack         = new StartAttack();
    //Phase2
    private CheckPossibleSkill  checkPossibleSkill  = new CheckPossibleSkill();
    private SkillAction         skillAction         = new SkillAction();
    //Dead
    private DeadProcess         deadProcess         = new DeadProcess();
    private StopAttack          stopAttack          = new StopAttack();
    private IsDie               isDie               = new IsDie();

    private IEnumerator behaviorProcess;
    private BossMonsterController bossController;

    public override void Init()
    {
        bossController = GetComponent<BossMonsterController>();
        bossController.Init();

        aiRoot.AddChild(selector);
        aiRoot.AddChild(seqNormal);

        selector.AddChild(seqchaseAttack);
        selector.AddChild(seqSkill);
        selector.AddChild(seqDead);

        seqNormal.AddChild(idle);
        seqNormal.AddChild(detectTarget);

        idle.Controller = bossController;
        detectTarget.Controller = bossController;

        chaseTarget.Controller = bossController;
        checkCloseTarget.Controller = bossController;
        lookatTarget.Controller = bossController;
        startAttack.Controller = bossController;

        checkPossibleSkill.Controller = bossController;
        skillAction.Controller = bossController;

        deadProcess.Controller = bossController;
        stopAttack.Controller = bossController;
        isDie.Controller = bossController;


        seqchaseAttack.AddChild(chaseTarget);
        seqchaseAttack.AddChild(checkCloseTarget);
        seqchaseAttack.AddChild(lookatTarget);
        seqchaseAttack.AddChild(startAttack);
        
        seqSkill.AddChild(checkPossibleSkill);
        seqSkill.AddChild(skillAction);

        seqDead.AddChild(isDie);
        seqDead.AddChild(deadProcess);
        seqDead.AddChild(stopAttack);

        behaviorProcess = BehaviorProcess();
    }

    private void Start()
    {
        Init();
        StartBT();
    }
    public override void StartBT()
    {
        StartCoroutine(behaviorProcess);
    }

    public override void StopBT()
    {
        StopCoroutine(behaviorProcess);
    }

    public override IEnumerator BehaviorProcess()
    {
        while(!aiRoot.Invoke())
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Exit Process");
    }
}
