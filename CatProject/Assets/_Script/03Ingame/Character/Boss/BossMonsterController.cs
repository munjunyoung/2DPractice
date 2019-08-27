using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOSS_TYPE { Person }
public enum BOSS_SEQUENCE { Normal ,Phase1, Phase2, Dead }
public enum BOSS_ANIMATION_STATE { Idle, Walk, Jump, Fall, Attack, Skill ,TakeDamage, Die}
public class BossMonsterController : MonoBehaviour
{
    protected BOSS_TYPE bType;
    protected BossData bData = new BossData();

    public DungeonRoom ownRoom = null;
    protected Transform TargetOB;


    private BOSS_ANIMATION_STATE _CurrentAnimState;
    protected BOSS_ANIMATION_STATE CurrentAnimState
    {
        get { return _CurrentAnimState; }
        set { _CurrentAnimState = value; }
    }
    
    protected SpriteRenderer    sR;
    protected Rigidbody2D       rb2D;
    protected BoxCollider2D     col;

    protected float             currentMoveSpeed;
    protected bool              checkCloseTarget;
    protected bool              aleadyFindTarget = false;

    //BossData
    public virtual void Init()
    {
        TargetOB = GameObject.FindWithTag("Player").transform;

        sR = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        CSVDataReader.instance.SetData(bData, bType.ToString());
    }
    
    public virtual void IdleAction() { }
    /// <summary>
    /// NOTE : 플레이어 발견시 True 리턴
    /// </summary>
    /// <returns></returns>
    public virtual bool DetectTarget() { return true; }
    
    public virtual void ChaseAction() { }
    public virtual bool CheckCloseTarget() { return true; }
    public virtual bool LookAtTarget() { return true; }
    public virtual bool CheckPossibleAttack() { return true; }
    public virtual void StartAttack() { }
    
    public virtual bool CheckPossibleSkill() { return true; }
    public virtual void SkillAction() { }

    public virtual bool isDie() { return false; }
    public virtual void StopAttack() { }
    public virtual void DeadProcess() { }
}
