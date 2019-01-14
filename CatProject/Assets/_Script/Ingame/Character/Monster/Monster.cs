using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private ANIMATION_STATE InstanceState;
    private ANIMATION_STATE CurrentState
    {
        get { return InstanceState; }
        set
        {
            InstanceState = value;
            switch (InstanceState)
            {
                case ANIMATION_STATE.Walk:
                    anim.speed = Mathf.Abs(currentMoveSpeed * 0.1f);
                    break;
                case ANIMATION_STATE.Attack:
                    anim.speed = mDATA.attackAnimSpeed;
                    break;
                default:
                    anim.speed = 1;
                    break;
            }
        }
    }

    /// <summary>
    /// NOTE : PATROLL SPEED, TRACESPEED 속성 설정
    /// </summary>
    enum ORDER_STATE { Patroll, Trace }
    private ORDER_STATE InstanceOrderState;
    private ORDER_STATE orderState
    {
        get { return InstanceOrderState; }
        set
        {
            InstanceOrderState = value;
            switch (InstanceOrderState)
            {
                case ORDER_STATE.Patroll:
                    currentMoveSpeed = mDATA.patrollSpeed;
                    break;
                case ORDER_STATE.Trace:
                    currentMoveSpeed = mDATA.traceSpeed;
                    break;
            }
        }

    }

    [Header("MONSTER DATA SET"), SerializeField]
    private MonsterData mDATA;

    private Rigidbody2D rb2D;
    private Animator anim;
    
    private float currentMoveSpeed;

    [Space(20), SerializeField]
    private Transform groundDetectionPrefab;

    protected void FixedUpdate()
    {
        switch (orderState)
        {
            case ORDER_STATE.Patroll:
                Patroll();
                break;
            case ORDER_STATE.Trace:

                break;
        }
    }

    /// <summary>
    /// NOTE : 구조물에서 길이 없거나 벽에 부딪혔을경우 방향 순회 (속도는 maxspeed의 절반)
    /// TODO : 벽에 부딪혔을 경우네는 점프를 하도록 구현 여지
    /// </summary>
    protected void Patroll()
    {
        transform.Translate(Vector2.right * currentMoveSpeed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetectionPrefab.position, Vector2.down, 0.1f);

        if (!groundInfo.collider || groundInfo.collider.CompareTag("Ground"))
            transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 180, 0);
    }

    /// <summary>
    /// NOTE : 추적
    /// </summary>
    protected void Trace()
    {

    }
}
