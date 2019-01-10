using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class CatType1 : CharacterInfo
{
    //[HideInInspector]
    public bool isGrounded, JumpButtonOn, AttackButtonOn= false;

    [HideInInspector]
    public float currentMoveInputValue;
    private float prevMoveInputValue = 0;

    private bool isRunningJumpCoroutine = false;
    private bool isRunningAttackCoroutine = false;

    private float attackCountTimer = 0;
    private bool attackPossible = true;

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void LateUpdate()
    {
        SetCharacterState();
        SetAnimation();
    }

    /// <summary>
    /// NOTE : 플레이어 캐릭터 이동 함수
    /// NOTE : 최대 속도 - 키 입력값 * maxSpeedValue (프레임당 키입력 값은 (0~1))
    /// FIXME : rb2D.velocity.y 플레이어가 떨어지면서 땅에 안착했을때 0이 되지않고 -5.0938같은 이상한 값으로 처리될 때가 있어서(int)형으로 캐스팅 수정
    /// </summary>
    private void Move()
    {
        float maxspeed = currentMoveInputValue * maxSpeedValue;
        currentSpeed += (currentMoveInputValue * accelerationValue);


        //우측방향 가속
        if (currentMoveInputValue > 0)
        {
            //이전 프레임 값과 비교하여 증감
            if (prevMoveInputValue <= currentMoveInputValue && currentSpeed >= maxspeed)
                currentSpeed = maxspeed;
        }
        //좌측방향 가속
        else if (currentMoveInputValue < 0)
        {
            if (prevMoveInputValue >= currentMoveInputValue && currentSpeed <= maxspeed)
                currentSpeed = maxspeed;
        }
        //감속
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * decelerationValue);
        }
        transform.position += new Vector3(currentSpeed, 0, 0) * Time.deltaTime;
        prevMoveInputValue = currentMoveInputValue;

        //캐릭터의 방향 설정
        if (!currentMoveInputValue.Equals(0f))
            transform.localEulerAngles = currentMoveInputValue > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }
    
    /// <summary>
    /// NOTE : 코루틴 함수가 현재 실행중인지 체크한 후 실행
    /// NOTE : IsGrounded는 캐릭터 오브젝트의 자식오브젝트의 트리거함수로 설정됨
    /// </summary>
    private void Jump()
    {
        if (isGrounded && JumpButtonOn && !isRunningJumpCoroutine)
            StartCoroutine(JumpCoroutine());
    }
    
    /// <summary>
    /// NOTE : JUMP()함수 실행되는 Coroutine
    /// TODO : 버튼이나 해당 키가 눌려있음을 체크하여 JumpCount만큼 실행, 다른 방법으로도 구현할 가능성이 있음
    /// </summary>
    IEnumerator JumpCoroutine()
    {
        isRunningJumpCoroutine = true;
        float jumpCount = 0;
        while (JumpButtonOn && jumpCount <= jumpMaxCount)
        {
            jumpCount++;
            rb2D.AddForce(Vector2.up * jumpPowerPerCount, ForceMode2D.Impulse);
            yield return new WaitForSeconds(addForceFrameIntervalTime);
        }
        isRunningJumpCoroutine = false;
    }

    #region ATTACK ANIMATION ADD EVENT FUNCTION
    /// <summary>
    /// NOTE : ATTACK 상태 실행시 코루틴
    /// NOTE : ATTACK ANIMATION 클립 내부 add Event에서 출력 
    /// NOTE : 공격 실행 후 카운트 실행 설정한ATTACK COOLTIME값 이후 ATTACK Possible true로 변경
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackCoroutine()
    {
        isRunningAttackCoroutine = true;
        attackPossible = false;
        attackCountTimer = 0;
        while (!attackPossible)
        {
            attackCountTimer += Time.deltaTime;
            if (attackCountTimer >= attackCoolTime)
                attackPossible = true;
            yield return null;
        }
        isRunningAttackCoroutine = false;
    }
    
    /// <summary>
    /// NOTE : 자식으로 저장된 attackEffectModel을 on, off하는 형식
    /// NOTE : ATTACK ANIMATION 클립 내부 add Event에서 출력
    /// TODO : 현재 애니매이션 클립내부에서 실행되기 때문에 클립이 변경된다면 함수 선언위치 등 고려가능성이 높음
    /// </summary>
    public void AttackEffectOn()
    {
        attackEffectModel.SetActive(true);
    }
    public void AttackEffectOff()
    {
        attackEffectModel.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 캐릭터 상태 설정
    /// NOTE : STATE
    /// 1. ATTACK 2. JUMP 3. FALL 4. MOVE
    /// </summary>
    private void SetCharacterState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentPlayerState = CHARACTER_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentPlayerState = CHARACTER_STATE.Fall;
        else
            CurrentPlayerState = (int)(currentSpeed * 10) == 0 ? CHARACTER_STATE.Idle : CHARACTER_STATE.Walk;

        if (AttackButtonOn&&attackPossible)
            CurrentPlayerState = CHARACTER_STATE.Attack;

    }

    /// <summary>
    /// NOTE : 애니매이션 설정, enum 상태를 그대로 대입 하여 사용
    /// </summary>
    private void SetAnimation()
    {
        anim.SetFloat("StateFloat", (int)CurrentPlayerState);
    }

    #region 테스트용
    private void Update()
    {
        UnLockRoom();
    }
    private void UnLockRoom()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InGameManager.GetInstance().UnLockRoom();
        }
    }
    #endregion
}


