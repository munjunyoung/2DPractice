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
    private Stack<ActionNode> childrens = new Stack<ActionNode>();

    public override bool Invoke()
    {
        throw new System.NotImplementedException();
    }

    public void AddChild(ActionNode _Node)
    {
        childrens.Push(_Node);
    }

    public Stack<ActionNode> GetChildrens()
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

public class Patrol : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.PatrolAction();
        return true;
    }
}

#endregion

#region Phase1
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
        return true;
    }
}

public class StopAttack :ActionNode
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
#endregion

#region phase2
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
        return true;
    }
}


#endregion

#region Dead
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

public class IsDie : ActionNode
{
    public BossMonsterController Controller
    {
        set { _controller = value; }
    }
    private BossMonsterController _controller;

    public override bool Invoke()
    {
        _controller.isDie();
        return true;
    }
}
#endregion
