using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{

}

/// <summary>
/// NOTE : ActionNode 추상클래스
/// </summary>
public abstract class ActionNode
{
    public abstract bool Invoke();
}

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

public class Idle : ActionNode
{
    public Monster monsterController
    {
        set { _monsterController = value; }
    }
    private Monster _monsterController;
    public override bool Invoke()
    {
        _monsterController.IdleAction();
        return true;
    }
}

public class Patrol : ActionNode
{
    public Monster monsterController
    {
        set { _monsterController = value; }
    }
    private Monster _monsterController;
    public override bool Invoke()
    {
        _monsterController.IdleAction();
        return true;
    }
}

public class ChaseTarget : ActionNode
{
    public Monster monsterController
    {
        set { _monsterController = value; }
    }
    private Monster _monsterController;

    public override bool Invoke()
    {
        _monsterController.ChaseAction();
        return true;
    }
}

