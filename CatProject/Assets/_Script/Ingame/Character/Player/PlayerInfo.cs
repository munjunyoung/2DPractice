using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOTE : 실제 플레이어캐릭터 정보들
/// </summary>
public class PlayerInfo : CharacterInfo
{

    private bool isRunningStopCoroutine = false;

    protected virtual void FixedUpdate()
    {
        Move(currentMoveInputValue);
        Jump();
    }

    /// <summary>
    /// NOTE : STOP코루틴 함수 실행
    /// </summary>
    /// <param name="stoptime"></param>
    public virtual void StopCharacter(float stoptime)
    {
        JumpButtonOn = false;
        AttackButtonOn = false;
        currentSpeed = 0;
        StartCoroutine(StopCoroutine(stoptime));
    }

    /// <summary>
    /// NOTE : 파라미터 시간 만큼 TIME STOP
    /// </summary>
    /// <param name="stoptime"></param>
    /// <returns></returns>
    protected virtual IEnumerator StopCoroutine(float stoptime)
    {
        isRunningStopCoroutine = true;
        allStop = true;
        yield return new WaitForSeconds(stoptime);
        allStop = false;
        isRunningStopCoroutine = false;
    }
}
