using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : CharacterInfo
{
    [HideInInspector]
    public bool JumpButtonOn, AttackButtonOn, allStop = false;

    private bool isRunningStopCoroutine = false;
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
