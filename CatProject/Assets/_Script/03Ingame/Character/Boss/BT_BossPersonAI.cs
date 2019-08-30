using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_BossPersonAI : BT_Base
{
    //최상위 
    private Sequence            aiRoot              = new Sequence();
    //선택
    private Sequence            seqNormal           = new Sequence();
    private Selector            selector            = new Selector();

    private Sequence            seqchaseAttack      = new Sequence();
    private Sequence            seqSkill            = new Sequence();
    private Sequence            seqDead             = new Sequence();
    //Normal
    private Idle                idle                = new Idle();
    private DetectTarget        detectTarget        = new DetectTarget();
    //Phase1
    private ChaseTarget         chaseTarget         = new ChaseTarget();
    private CheckCloseTarget    checkCloseTarget    = new CheckCloseTarget();
    private CheckPossibleAttack checkPossibleAttack = new CheckPossibleAttack();
    private StartAttack         startAttack         = new StartAttack();
    //Phase2
    private CheckPossibleSkill  checkPossibleSkill  = new CheckPossibleSkill();
    private SkillAction         skillAction         = new SkillAction();
    //Dead
    private IsDie               isDie               = new IsDie();
    private DeadAction          deadAction         = new DeadAction();

    private IEnumerator behaviorProcess;
    private BossMonsterController bossController;

    public override void Init()
    {
        bossController = GetComponent<BossMonsterController>();
        bossController.Init();
    
        idle.Controller = bossController;
        detectTarget.Controller = bossController;

        chaseTarget.Controller = bossController;
        checkCloseTarget.Controller = bossController;
        checkPossibleAttack.Controller = bossController;
        startAttack.Controller = bossController;

        checkPossibleSkill.Controller = bossController;
        skillAction.Controller = bossController;

        deadAction.Controller = bossController;
        isDie.Controller = bossController;

        //Node 추가 
        aiRoot.AddChild(seqNormal);

        seqNormal.AddChild(idle);
        seqNormal.AddChild(detectTarget);
        
        aiRoot.AddChild(selector);
            
        selector.AddChild(seqchaseAttack);
        selector.AddChild(seqSkill);
        selector.AddChild(seqDead);

        seqchaseAttack.AddChild(chaseTarget);
        seqchaseAttack.AddChild(checkCloseTarget);
        seqchaseAttack.AddChild(checkPossibleAttack);
        seqchaseAttack.AddChild(startAttack);
        
        seqSkill.AddChild(checkPossibleSkill);
        seqSkill.AddChild(skillAction);

        seqDead.AddChild(isDie);
        seqDead.AddChild(deadAction);

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
