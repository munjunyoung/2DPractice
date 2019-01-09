using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    private PLAYERSTATE currentPlayerState = PLAYERSTATE.Idle;

    [Header("Component Reference")]
    [SerializeField]
    private Rigidbody2D rb2D;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private GameObject attackEffectModel;
    
    [Header("MOVE OPTION")]
    [SerializeField, Range(10f, 100f)]
    private float speedValue;
    [SerializeField, Range(0.1f, 3f)]
    private float accelerationValue, decelerationValue = 0;
    
    //직전 프레임 dirX
    private float beforedirX = 0;
    [HideInInspector]
    public Vector2 dirValueOfJoystick;
    //실제 Move에서 참조하는 벡터 (Vector3를 사용하나 y값은 사용안해서 제외해도 될듯 하다)
    private Vector3 MoveDirValue;

    [Header("JUMP OPTION")]
    [SerializeField, Range(1, 30)]
    private float jumpPower;
    //코루틴 반복시 addforce 카운트 횟수
    [SerializeField, Range(1, 10)]
    private float jumpMaxCount;
    //코루틴 대기 속도 설정
    [SerializeField, Range(0.001f, 0.1f)]
    private float jumpSpeed;
    private bool jumpRunning = false;

    [HideInInspector]
    public bool JumpButtonOn = false;
    [HideInInspector]
    public bool AttackButtonOn = false;
    [HideInInspector]
    public bool isGrounded;
    

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void LateUpdate()
    {
        SetAnimation();
    }


    /// <summary>
    /// NOTE : 플레이어 캐릭터 이동 함수
    /// XXX : rb2D.velocity.y 플레이어가 떨어지면서 땅에 안착했을때 0이 되지않고 -5.0938같은 이상한 값으로 처리될 때가 있어서(int)형으로 캐스팅 수정
    /// </summary>
    private void Move()
    {
        var maxspeed = dirValueOfJoystick.x * speedValue;
        MoveDirValue.x += (dirValueOfJoystick.x * accelerationValue);

        if (dirValueOfJoystick.x > 0)
        {
            if (beforedirX <= dirValueOfJoystick.x)
            {
                if (MoveDirValue.x >= maxspeed)
                    MoveDirValue.x = maxspeed;
            }
        }
        else if (dirValueOfJoystick.x < 0)
        {
            if (beforedirX >= dirValueOfJoystick.x)
            {
                if (MoveDirValue.x <= maxspeed)
                    MoveDirValue.x = maxspeed;
            }
        }
        else
        {
            MoveDirValue = Vector3.Slerp(MoveDirValue, Vector3.zero, Time.deltaTime * decelerationValue);
        }

        transform.position += MoveDirValue * Time.deltaTime;
        beforedirX = dirValueOfJoystick.x;

        //캐릭터의 방향 설정
        if (!dirValueOfJoystick.Equals(Vector3.zero))
            transform.localEulerAngles = dirValueOfJoystick.x > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }

    /// <summary>
    /// NOTE : 점프 IsGrounded는 캐릭터 오브젝트의 자식오브젝트의 트리거함수로 설정됨
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
        {
            if (JumpButtonOn)
            {
                if (!jumpRunning)
                    StartCoroutine(JumpCoroutine());
            }
        }
    }

    /// <summary>
    /// NOTE : JUMP()함수 실행되는 Corutine 
    /// TODO : 버튼이나 해당 키가 눌려있음을 체크하여 JumpCount만큼 실행, 다른 방법으로도 구현할 가능성이 있음
    /// </summary>
    /// <param name="onButton"></param>
    /// <returns></returns>
    IEnumerator JumpCoroutine()
    {
        jumpRunning = true;
        var jumpCount = 0;
        while (JumpButtonOn && jumpCount <= jumpMaxCount)
        {
            jumpCount++;
            rb2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            yield return new WaitForSeconds(jumpSpeed);
        }
        jumpRunning = false;
    }

    private void Attack()
    {
        
    }

    /// <summary>
    /// NOTE : 자식으로 저장된 attackEffectModel을 on, off하는 형식
    /// </summary>
    public void AttackEffectOn()
    {
        attackEffectModel.SetActive(true);
    }
    public void AttackEffectOff()
    {
        attackEffectModel.SetActive(false);
    }
    
    /// <summary>
    /// 애니매이션 설정
    /// </summary>
    private void SetAnimation()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            currentPlayerState = PLAYERSTATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            currentPlayerState = PLAYERSTATE.Fall;
        else
            currentPlayerState = (int)(MoveDirValue.x * 10) == 0 ? PLAYERSTATE.Idle : PLAYERSTATE.Walk;

        if (AttackButtonOn)
            currentPlayerState = PLAYERSTATE.Attack;

        //애니매이션 속도 설정
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            anim.speed = dirValueOfJoystick.Equals(Vector2.zero) ? 1 : Mathf.Abs(MoveDirValue.x * 0.05f);
        else
            anim.speed = 1f;

        anim.SetFloat("StateBlend", (int)currentPlayerState);
    }

}


