using UnityEngine;
using System;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    enum MoveState { Idle, Run, Jump, Fall }

    [Range(0, 10f)]
    public float speed;
    [HideInInspector]
    public Vector3 directionVector = Vector3.zero;
    private Rigidbody2D rb2D;
    [Range(10,50)]
    public float jumpPower;

    //Animation 관련
    private MoveState PlayerMoveState;
    private Animator anim;

    [HideInInspector]
    public bool isGrounded;
    
    void Start()
    {
        PlayerMoveState = MoveState.Idle;
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //InputKeyboard();
        //Attack();
        //if (Input.GetKeyDown(KeyCode.Space))
        //    Jump();
    }
    
    private void FixedUpdate()
    {
        Move(directionVector);
    }
 
    private void LateUpdate()
    {
        //애니매이션 설정
        anim.SetFloat("State", (int)PlayerMoveState);
    }

    /// <summary>
    /// 플레이어 캐릭터 이동 
    /// </summary>
    private void Move(Vector3 dir)
    {
        //대각선 일정수치 
        dir = dir.normalized;
        transform.position += dir * speed * Time.deltaTime;
        
        //이동을 멈춘후에도 캐릭터의 방향을 유지시키기 위한 설정
        if (!dir.Equals(Vector3.zero))
            transform.localScale = dir.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        //캐릭터 애니매이션 상태 설정
        if (rb2D.velocity.y > 0)
            PlayerMoveState = MoveState.Jump;
        else if (rb2D.velocity.y < 0)
            PlayerMoveState = MoveState.Fall;
        else
            PlayerMoveState = directionVector.Equals(Vector2.zero) ? MoveState.Idle : MoveState.Run;
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb2D.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }
    
    /// <summary>
    /// 공격
    /// </summary>
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }
    
    #region input Keyboard set
    /// <summary>
    /// 키보드 키 입력 관련 함수
    /// </summary>
    private void InputKeyboard()
    {
        //이렇게 되면 성능차이가 심하려나 ? 매프레임마다 초기화 됨
        var w = Input.GetKey(KeyCode.W) ? 1 : 0;
        var s = Input.GetKey(KeyCode.S) ? -1 : 0;
        var a = Input.GetKey(KeyCode.A) ? -1 : 0;
        var d = Input.GetKey(KeyCode.D) ? 1 : 0;
        
        directionVector = new Vector2(a + d, w + s);
    }
    
    private void InputAttack()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Attack();
    }
    #endregion
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

