using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOSS_TYPE { Person }
public enum BOSS_SEQUENCE { Normal , Phase1, Phase2, Dead }
public enum BOSS_ANIMATION_STATE { Idle, Walk, Jump, Fall, Attack, Skill ,TakeDamage, Die}
public class BossMonster : MonoBehaviour
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
    //BossData

    public void IdleAction() { }
    public void PatrolAction() { }
    public void ChaseAction() { }
    public void AttackAction() { }
    public void SkillAction() { }
    
    protected bool CheckDetectTarget() { return false; }
    protected bool CheckCloseTarget() { return false; }

}
