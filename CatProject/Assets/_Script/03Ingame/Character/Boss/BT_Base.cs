using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BT_Base : MonoBehaviour
{
    public abstract void Init();
    public abstract void StartBT();
    public abstract void StopBT();
    public abstract IEnumerator BehaviorProcess();
}
/// <summary>
/// NOTE : ActionNode 추상클래스
/// </summary>
public abstract class ActionNode
{
    public abstract bool Invoke();
}

/// <summary>
/// NOTE : Node stack 
/// </summary>
public class CompositeActionNode :ActionNode
{
    private List<ActionNode> childrens = new List<ActionNode>();

    public override bool Invoke()
    {
        throw new System.NotImplementedException();
    }

    public void AddChild(ActionNode _Node)
    {
        childrens.Add(_Node);
    }

    public List<ActionNode> GetChildrens()
    {
        return childrens;
    }
}

/// <summary>
/// NOTE : 노드들중 하나라도 True면 true를 리턴
/// </summary>
public class Selector : CompositeActionNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (node.Invoke())
                return true;
        }
        return false;
    }
}

/// <summary>
/// NOTE : 모든 자식들이 True여야 True를 리턴
/// </summary>
public class Sequence : CompositeActionNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (!node.Invoke())
                return false;
        }
        return true;
    }
}

#region Normal
public class Idle : ActionNode
{
    public BossMonsterController Controller
    {
        set {  _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.IdleAction();
        return true;
    }
}

public class DetectTarget : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        return _controller.DetectTarget();
    }
}

#endregion

#region chaseAttack
/// <summary>
/// NOTE : 타겟에게 이동
/// </summary>
public class ChaseTarget : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.ChaseAction();
        return true;
    }
}

/// <summary>
/// NOTE : 지정한 거리에 타겟 존재 확인
/// </summary>
public class CheckCloseTarget : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        if (_controller.CheckCloseTarget())
            return true;
        return false;
    }
}

/// <summary>
/// NOTE : 공격 가능한 상태인지 체크
/// </summary>
public class CheckPossibleAttack : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        if (_controller.CheckPossibleAttack())
            return true;
        return false;
    }
}

/// <summary>
/// NOTE : 공격 시작
/// </summary>
public class StartAttack : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.StartAttack();
        return false;
    }
}

#endregion

#region skill

/// <summary>
/// NOTE : 스킬 사용이 가능한지 체크
/// </summary>
public class CheckPossibleSkill : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        if (_controller.CheckPossibleSkill()) 
            return true;
        return false;
    }
}

/// <summary>
/// NOTE : 스킬 ACTION 
/// </summary>
public class SkillAction : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.SkillAction();
        return false;
    }
}


#endregion

#region Dead
/// <summary>
/// NOTE : 죽음
/// </summary>
public class DeadProcess :ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.DeadProcess();
        return true;
    }
}

public class StopAttack : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.StopAttack();
        return true;
    }
}

public class IsDie : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        return _controller.isDie();
    }
}
#endregion
