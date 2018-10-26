using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    enum MoveState { Idle, Walk, Run }
    enum Key { W = 0, A, S, D }

    //키코드 설정 변경시
    public List<KeyCode> KeyCodeList = new List<KeyCode>();
    public bool[] keyState = new bool[20];

    //이동 관련
    public float speed = 2;
    private Vector2 dir = Vector2.zero;

    //Animation 관련
    private int dirBlendValue = 0;
    private MoveState PlayerMoveState;
    private Animator anim;
    private float stateAnimParam;

    // Use this for initialization
    void Start()
    {
        PlayerMoveState = MoveState.Idle;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        TakeInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// 이동 관련 함수
    /// </summary>
    private void Move()
    {
        var w = keyState[(int)Key.W] ? 1 : 0;
        var s = keyState[(int)Key.S] ? -1 : 0;
        var a = keyState[(int)Key.A] ? -1 : 0;
        var d = keyState[(int)Key.D] ? 1 : 0;

        Vector2 dir = new Vector2(a + d, w + s);

        dir = dir.normalized;
        transform.Translate(dir * speed * Time.deltaTime);


        //방향 설정
        anim.SetFloat("Side", dir.x);
        anim.SetFloat("Front", dir.y);

        anim.SetFloat("BlendDir",0);

        // Animation Setting State Float param  0 : idle 1 : walk
        if (dir.Equals(Vector2.zero))
            PlayerMoveState = MoveState.Idle;
        else
            PlayerMoveState = MoveState.Walk;

        Debug.Log((int)PlayerMoveState);
        anim.SetInteger("State", (int)PlayerMoveState);
    }

    /// <summary>
    /// 키 입력 관련 함수
    /// </summary>
    private void TakeInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            keyState[(int)Key.W] = true;
        else if (Input.GetKeyUp(KeyCode.W))
            keyState[(int)Key.W] = false;

        if (Input.GetKeyDown(KeyCode.S))
            keyState[(int)Key.S] = true;
        else if (Input.GetKeyUp(KeyCode.S))
            keyState[(int)Key.S] = false;

        if (Input.GetKeyDown(KeyCode.A))
            keyState[(int)Key.A] = true;
        else if (Input.GetKeyUp(KeyCode.A))
            keyState[(int)Key.A] = false;

        if (Input.GetKeyDown(KeyCode.D))
            keyState[(int)Key.D] = true;
        else if (Input.GetKeyUp(KeyCode.D))
            keyState[(int)Key.D] = false;
    }
}
