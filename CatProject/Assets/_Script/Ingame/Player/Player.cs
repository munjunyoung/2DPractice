using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    enum MoveState { Idle, Run, Jump, Fall }

    [SerializeField]
    [Header("JOYSTICK")]
    private JoyStickScript joystickSc;
    //물리 관련
    private Rigidbody2D rb2D;
    [Header("MOVE OPTION")]

    //스틱 dircetion x 값 * speedValue
    [SerializeField]
    [Range(10f, 20f)]
    private float speedValue;
    [SerializeField]
    [Range(0.1f, 3f)]
    private float accelerationValue, decelerationValue;
    [SerializeField]
    [Range(10, 50)]
    private float jumpPower;

    [HideInInspector]
    public bool isGrounded;

    [HideInInspector]
    //조이스틱, keydata 외부 참조 벡터
    public Vector3 dirValueVector;
    //실제 Move에서 참조하는 벡터 (Vector3를 사용하나 y값은 사용안해서 제외해도 될듯 하다)
    private Vector3 actualMoveDirVector;

    //Animation 관련
    private MoveState PlayerMoveState;
    private Animator anim;


    void Start()
    {
        PlayerMoveState = MoveState.Idle;
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        dirValueVector = joystickSc.DirValue;
    }

    private void FixedUpdate()
    {
        Move(dirValueVector);
    }

    private void LateUpdate()
    {
        //애니매이션 설정
        anim.SetFloat("State", (int)PlayerMoveState);
    }


    private float beforedirX = 0;
    /// <summary>
    /// 플레이어 캐릭터 이동 
    /// </summary>
    private void Move(Vector3 stickDir)
    {
        var maxspeed = stickDir.x * speedValue;

        actualMoveDirVector.x += (stickDir.x * accelerationValue);

        if (stickDir.x > 0)
        {
            if (beforedirX <= stickDir.x)
            {
                if (actualMoveDirVector.x >= maxspeed)
                    actualMoveDirVector.x = maxspeed;
            }
        }
        else if (stickDir.x < 0)
        {
            if (beforedirX >= stickDir.x)
            {
                if (actualMoveDirVector.x <= maxspeed)
                    actualMoveDirVector.x = maxspeed;
            }
        }
        else
        {
            actualMoveDirVector = Vector3.Slerp(actualMoveDirVector, Vector3.zero, Time.deltaTime * decelerationValue);
        }

        transform.position += actualMoveDirVector * Time.deltaTime;
        beforedirX = stickDir.x;
        //캐릭터의 방향 설정
        if (!stickDir.Equals(Vector3.zero))
            transform.localScale = stickDir.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        //캐릭터 상태 설정(애니매이션 상태 설정)
        if (rb2D.velocity.y > 0)
            PlayerMoveState = MoveState.Jump;
        else if (rb2D.velocity.y < 0)
            PlayerMoveState = MoveState.Fall;
        else
        {
            PlayerMoveState = stickDir.Equals(Vector2.zero) ? MoveState.Idle : MoveState.Run;
            // 애니매이션 속도 설정
            anim.speed = stickDir.Equals(Vector2.zero) ? 1 : Mathf.Abs(actualMoveDirVector.x*0.1f);
        }
    }

    /// <summary>
    /// 
    /// 점프 IsGrounded는 캐릭터 오브젝트의 자식오브젝트를 통하여 설정
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            if (PlayerMoveState != MoveState.Jump)
                rb2D.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// 공격
    /// </summary>
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

  
}



/*
 private byte dirBlendValue;

if (Input.GetKeyDown(KeyCode.W))
    dirKey[(int)Key.W] = true;
else if (Input.GetKeyUp(KeyCode.W))
    dirKey[(int)Key.W] = false;

if (Input.GetKeyDown(KeyCode.S))
    dirKey[(int)Key.S] = true;
else if (Input.GetKeyUp(KeyCode.S))
    dirKey[(int)Key.S] = false;

if (Input.GetKeyDown(KeyCode.A))
    dirKey[(int)Key.A] = true;
else if (Input.GetKeyUp(KeyCode.A))
    dirKey[(int)Key.A] = false;

if (Input.GetKeyDown(KeyCode.D))
    dirKey[(int)Key.D] = true;
else if (Input.GetKeyUp(KeyCode.D))
    dirKey[(int)Key.D] = false;

var w = dirKey[(int)Key.W] ? 1 : 0;
var s = dirKey[(int)Key.S] ? -1 : 0;
var a = dirKey[(int)Key.A] ? -1 : 0;
var d = dirKey[(int)Key.D] ? 1 : 0;
*/

