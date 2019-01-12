using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
/// <summary>
/// NOTE : CAT TYPE1 캐릭터 
/// </summary>
public class CatType1 : PlayerInfo
{


    private float attackCountTimer = 0;
    private bool attackPossible = true;

    protected override void FixedUpdate()
    {     
        if (allStop)
            return;

        base.FixedUpdate();
            Jump();
    }

    private void LateUpdate()
    {
        SetCharacterState();
        SetAnimation();
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
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InGameManager.GetInstance().UnLockRoom();
        }
    }
    #endregion
}


