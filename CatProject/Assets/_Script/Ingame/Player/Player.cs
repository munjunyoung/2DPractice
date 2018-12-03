using UnityEngine;
using System;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    enum MoveState { Idle, Run, Jump, Fall }

    //물리 관련
    private Rigidbody2D rb2D;
    [Header("MOVE OPTION")]
    [Range(10, 20f)]
    public float maxSpeed;
    [Range(0.1f, 3f)]
    public float accelerationValue;
    [Range(0.1f, 3f)]
    public float decelerationValue;
    [Range(10, 50)]
    public float jumpPower;
    [HideInInspector]
    public bool isGrounded;

    [HideInInspector]
    //조이스틱, keydata 외부 참조 벡터
    public Vector3 stickValueVector; 
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
        //InputKeyboard();
    }
    
    private void FixedUpdate()
    {
        Move(stickValueVector);
    }
 
    private void LateUpdate()
    {
        //애니매이션 설정
        anim.SetFloat("State", (int)PlayerMoveState);
    }

    /// <summary>
    /// 플레이어 캐릭터 이동 
    /// </summary>
    private void Move(Vector3 stickDir)
    {
        //가속, 감속 적용
        if (!stickDir.x.Equals(0))
        {
            actualMoveDirVector.x += stickDir.x * accelerationValue;

            if (Mathf.Abs(actualMoveDirVector.x) > maxSpeed)
                actualMoveDirVector.x = actualMoveDirVector.x > 0 ? maxSpeed : -maxSpeed;
        }
        else
        {
            //actualMoveDirVector.x가 0에 도달 했을 경우 slerp를 도는것보다 조건문을 넘기는게 더 나아보인다.
            if (!actualMoveDirVector.x.Equals(0))
                actualMoveDirVector = Vector3.Slerp(actualMoveDirVector, Vector3.zero, Time.deltaTime * decelerationValue);
        }
        //실제 물리 이동
        
        transform.position += actualMoveDirVector * Time.deltaTime;

        //캐릭터의 방향 설정
        if (!stickDir.Equals(Vector3.zero))
            transform.localScale = stickDir.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        //캐릭터 상태 설정(애니매이션 상태 설정)
        if (rb2D.velocity.y > 0)
            PlayerMoveState = MoveState.Jump;
        else if (rb2D.velocity.y < 0)
            PlayerMoveState = MoveState.Fall;
        else
            PlayerMoveState = stickDir.Equals(Vector2.zero) ? MoveState.Idle : MoveState.Run;
    }

    /// <summary>
    /// 점프 IsGrounded는 캐릭터 오브젝트의 자식오브젝트를 통하여 설정
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            if(PlayerMoveState!=MoveState.Jump)
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
        
        stickValueVector = new Vector2(a + d , 0);

        InputJump();
    }
    
    private void InputJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
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

