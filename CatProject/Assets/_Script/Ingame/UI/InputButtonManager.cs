using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputButtonManager : MonoBehaviour
{
    
    [Header("Player Object")]
    [SerializeField]
    private Player playerSc;
    //UI 스크립트
    [Header("JoyStick Object")]
    [SerializeField]
    private JoyStickScript joyStickSc;
    
    [Header("JUMP BUTTON")]
    [SerializeField]
    private Button jumpButton;

    private void Update()
    {
    }
    
    /// <summary>
    /// Player Jump Button
    /// </summary>
    public void JumpPlayer()
    {
        playerSc.Jump();
    }
    #region 우측하단 UI
    /// <summary>
    /// 일반공격 실행
    /// </summary>
    public void NormalAttack()
    {
        playerSc.Attack();
    }

    /// <summary>
    /// 첫번째 스킬 실행
    /// </summary>
    public void Skill1()
    {
        Debug.Log("Skill1");
    }

    /// <summary>
    /// 두번째 스킬 실행
    /// </summary>
    public void Skill2()
    {
        Debug.Log("Skill2");
    }

    /// <summary>
    /// 세번째 스킬 실행
    /// </summary>
    public void Skill3()
    {
        Debug.Log("Skill3");
    }

    #endregion
}
