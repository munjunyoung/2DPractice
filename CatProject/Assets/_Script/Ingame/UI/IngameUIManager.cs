using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUIManager : MonoBehaviour {
    
    [Header("Player Object")]
    public Player playerSc;
    //UI 스크립트
    [Header("JoyStick Object")]
    public JoyStickScript joyStickSc;
    
    // Use this for initialization
    void Start () {
		
	}

    private void Update()
    {
        MovePlayer(joyStickSc.DirValue);
    }

    /// <summary>
    /// 조이스틱을 이용한 플레이어 이동벡터 초기화
    /// </summary>
    /// <param name="dir"></param>
    private void MovePlayer(Vector2 dir)
    {
        playerSc.directionVector = dir;
    }

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
