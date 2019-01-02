using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
    enum MoveState { Idle, Walk, Jump, Fall }

    //물리 관련
    private Rigidbody2D rb2D;
    [Header("MOVE OPTION")]
    [SerializeField, Range(10f, 100f)]
    private float speedValue;
    [SerializeField, Range(0.1f, 3f)]
    private float accelerationValue, decelerationValue;
    [HideInInspector]
    public Vector2 dirvalue;

    [Header("JUMP OPTION")]
    [SerializeField, Range(1, 30)]
    private float jumpPower;
    //코루틴 반복시 addforce 카운트 횟수
    [SerializeField, Range(1, 10)]
    private float jumpMaxCount;
    //코루틴 대기 속도 설정
    [SerializeField, Range(0.001f, 0.1f)]
    private float jumpSpeed;

    [HideInInspector]
    public bool jumpButtonOn = false;
    //[HideInInspector]
    public bool isGrounded;
    public bool leftCheck;

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

    private void FixedUpdate()
    {
        Move(dirvalue);
    }

    private void LateUpdate()
    {
        //애니매이션 설정
        anim.SetFloat("StateBlend", (int)PlayerMoveState);
    }


    private float beforedirX = 0;
    /// <summary>
    /// 플레이어 캐릭터 이동 
    /// </summary>
    public void Move(Vector3 stickDir)
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

        if (leftCheck)
            actualMoveDirVector = Vector3.zero;

        transform.Translate(actualMoveDirVector * Time.deltaTime);
        //rb2D.position += actualMoveDirVector * Time.deltaTime;
        //if(!leftCheck)
        //    rb2D.velocity = new Vector2(actualMoveDirVector.x, rb2D.velocity.y);
        //Debug.Log(rb2D.velocity);
        //rb2D.MovePosition(transform.position + actualMoveDirVector * Time.deltaTime);
        //transform.position += actualMoveDirVector * Time.deltaTime;
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
            if (isGrounded)
            {
                PlayerMoveState = (int)(actualMoveDirVector.x * 10) == 0 ? MoveState.Idle : MoveState.Walk;
                // 애니매이션 속도 설정
                anim.speed = stickDir.Equals(Vector2.zero) ? 1 : Mathf.Abs(actualMoveDirVector.x * 0.1f);
            }
        }
    }

    /// <summary>
    /// 점프 IsGrounded는 캐릭터 오브젝트의 자식오브젝트를 통하여 설정
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
            StartCoroutine(JumpCoroutine());
    }

    /// <summary>
    /// jump Coroutine
    /// </summary>
    /// <param name="onButton"></param>
    /// <returns></returns>
    IEnumerator JumpCoroutine()
    {
        var jumpCount = 0;
        while (jumpButtonOn && jumpCount <= jumpMaxCount)
        {
            jumpCount++;
            rb2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            yield return new WaitForSeconds(jumpSpeed);
        }
        jumpButtonOn = false;
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

