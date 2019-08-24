using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOSS_TYPE { Person }
public enum BOSS_SEQUENCE { Normal , Phase1, Phase2, Dead }
public enum BOSS_ANIMATION_STATE { Idle, Walk, Jump, Fall, Attack, Skill ,TakeDamage, Die}
public abstract class BossMonsterController : MonoBehaviour
{
    public DungeonRoom ownRoom = null;

    private BOSS_ANIMATION_STATE _CurrentAnimState;
    protected BOSS_ANIMATION_STATE CurrentAnimState
    {
        get { return _CurrentAnimState; }
        set
        {
            _CurrentAnimState = value;
        }
    }

    protected BOSS_TYPE bType;
    protected float currentMoveSpeed;
    //BossData

    public abstract void IdleAction();
    public abstract void PatrolAction();

    public abstract void ChaseAction();
    public abstract void StartAttack();
    public abstract void StopAttack();
    public abstract bool CheckCloseTarget();
    public abstract void SkillAction();

    public abstract void DeadProcess();
    public abstract void isDie();

    public abstract bool CheckDetectTarget();
}
