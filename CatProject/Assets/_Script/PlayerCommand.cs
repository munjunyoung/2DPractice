using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TEST 실패
/// </summary>
enum BTN_ACTION { Move = 0, Jump, Attack }
public class PlayerCommand : MonoBehaviour
{
    private Player playerSc;
    private Command moveC, jumpC, attackC;
    public bool moveButtonON, jumpButtonON, attackButtonON;
    // Start is called before the first frame update
    void Start()
    {
        playerSc = GameObject.FindWithTag("Player").GetComponent<Player>();
        SetCommand();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// NOTE : COMMAND 설저
    /// </summary>
    private void SetCommand()
    {
        moveC = new CommandMove();
        jumpC = new CommandJump();
        attackC = new CommandAttack();
    }
}

public class Command
{
    public virtual void Execute(Player _player) { }
    public virtual void Execute(Player _player, Vector2 input) { }
    public virtual void Execute(Player _player, bool buttonOn) { }
}

public class CommandMove : Command
{
    public override void Execute(Player _player, Vector2 input)
    {
        //_player.Move();
    }
}

public class CommandJump : Command
{
    public override void Execute(Player _player, bool buttonOn)
    {
        //_player.Jump(buttonOn);
    }
}

public class CommandAttack : Command
{
    public override void Execute(Player _player)
    {
        //_player.Attack();
    }
}
