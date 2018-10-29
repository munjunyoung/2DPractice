using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public JoyStickScript joyStick;
    public Player player;

    private void FixedUpdate()
    {
        
    }
}

abstract class Command : MonoBehaviour
{
    protected abstract void Execute(GameObject actor);    
}

public class MoveCommand : MonoBehaviour
{
    GameObject ob;
    Vector2 dir;

    public MoveCommand(GameObject o)
    {
        ob = o;
        dir = Vector2.zero;
    }
    
    public void Move(Vector2 dir, float speed)
    {
        dir = dir.normalized;
        ob.transform.transform.Translate(dir * speed * Time.deltaTime);
    }
}

