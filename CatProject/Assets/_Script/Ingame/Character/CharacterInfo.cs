using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTER_STATE { Idle = 0, Walk, Jump, Fall, Attack}

/// <summary>
/// NOTE : 재활용성이 높은 타입들
/// TODO : 엑셀과 연동하기 위하여 데이터 조정값들은 따로 관리해야함
/// TODO : 함수또한 분리 해야함
/// </summary>
[RequireComponent(typeof(Rigidbody2D),typeof(Animator))]
public class CharacterInfo : MonoBehaviour
{
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private CHARACTER_STATE InstanceState;
    protected CHARACTER_STATE CurrentCharcterState
    {
        get{ return InstanceState; }
        set
        {
            InstanceState = value;
            switch(InstanceState)
            {
                case CHARACTER_STATE.Walk:
                    anim.speed = Mathf.Abs(currentSpeed * 0.1f);
                    break;
                case CHARACTER_STATE.Attack:
                    anim.speed = attackAnimSpeed;
                    break;
                default:
                    anim.speed = 1;
                    break;
            }
        }
    }
    
    protected Rigidbody2D rb2D;
    protected Animator anim;
    
    [HideInInspector]
    public bool isGrounded, isAlive = false;
    [HideInInspector]
    public bool JumpOn, AttackOn, StopOn = false;
    
    protected float currentSpeed;
    
    protected bool attackPossible = true;

    protected bool isRunningStopCoroutine = false;
    protected bool isRunningJumpCoroutine = false;
    protected bool isRunningAttackCoroutine = false;
    
    #region EXCEL DATA
    [Header("HEALTH OPTION")]
    protected int MaxHealth;

    /// <summary>
    /// Move :  플레이어 캐릭터 이동 함수 
    /// maxSpeedValue
    /// NOTE : 최대 속도 값 설정 (키 입력값 * maxSpeedValue (프레임당 키입력 값은 (0~1)))
    /// accelerationValue
    /// NOTE : 최대 속도 가속 값 (실제 속도 += 키입력값 * accelerationValue)
    /// decelerationValue
    /// NOTE : 정지 상태 감속 값 (Lerp함수 Time.deltaTime * decelerationValue)
    /// </summary>
    [Header("MOVE OPTION")]
    [SerializeField, Range(10f, 100f)]
    protected float maxSpeedValue;
    [SerializeField, Range(0.1f, 3f)]
    protected float accelerationValue, decelerationValue;

    /// <summary>
    /// JUMP : 버튼이나 해당 키가 눌려있음을 체크하여 JumpCount만큼 Addforce(impulse) 반복 실행
    /// JumpMaxCount 
    /// NOTE : 최대 addforce를 실행할 카운트 설정
    /// JumpPowerPerCount
    /// NOTE : addforce에 실제로 들어갈 jumpPower값 설정
    /// addForceFrameIntervalSpeed 
    /// NOTE : addforce를 함수 실행 프레임을 몇초 간격으로 둘것인지 설정
    /// </summary>
    [Header("JUMP OPTION")]
    [SerializeField, Range(1, 10)]
    protected int jumpMaxCount;
    [SerializeField, Range(1, 30)]
    protected float jumpPowerPerCount;
    [SerializeField, Range(0.001f, 0.1f)]
    protected float addForceFrameIntervalTime;

    /// <summary>
    /// ATTACK 관련 함수 
    /// attackEffectModel
    /// NOTE : 해당 이펙트 오브젝트 저장
    /// attackRange
    /// NOTE : 이펙트 Scale을 설정하여 처리
    /// attackDamage
    /// NOTE : 공격 데미지 양
    /// attackAnimSpeed
    /// NOTE : 애니매이션의 속도 이펙트 생성 시간 및 끝나는 시간조절
    /// attackCoolTime
    /// NOTE : 공격 후 대기시간
    /// </summary>
    [Header("ATTACK OPTION")]
    [SerializeField]
    protected GameObject attackEffectModel;
    [SerializeField, Range(1, 5)]
    protected float attackRange;
    [SerializeField, Range(1f, 10f)]
    protected int attackDamage;
    [SerializeField, Range(0.1f, 5f)]
    protected float attackAnimSpeed;
    [SerializeField, Range(0f, 5f)]
    protected float attackCoolTime;
    #endregion
    
    protected virtual void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        if (StopOn)
            return;
    }

    private void LateUpdate()
    {
        SetCharacterState();
        SetAnimation();
    }
    
    /// <summary>
    /// NOTE : STOP코루틴 함수 실행
    /// NOTE : 외부 함수에서 실행하는 경우가 있어서 public 선언
    /// </summary>
    /// <param name="stoptime"></param>
    public void StopCharacter(float stoptime)
    {
        JumpOn = false;
        AttackOn = false;
        currentSpeed = 0;
        StartCoroutine(StopCoroutine(stoptime));
    }

    /// <summary>
    /// NOTE : 파라미터 시간 만큼 TIME STOP
    /// </summary>
    /// <param name="stoptime"></param>
    /// <returns></returns>
    private IEnumerator StopCoroutine(float stoptime)
    {
        isRunningStopCoroutine = true;
        StopOn = true;
        yield return new WaitForSeconds(stoptime);
        StopOn = false;
        isRunningStopCoroutine = false;
    }

    /// <summary>
    /// 캐릭터 상태 설정
    /// NOTE : STATE
    /// 1. ATTACK 2. JUMP 3. FALL 4. MOVE
    /// </summary>
    protected virtual void SetCharacterState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentCharcterState = CHARACTER_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentCharcterState = CHARACTER_STATE.Fall;
        else
            CurrentCharcterState = (int)(currentSpeed * 10) == 0 ? CHARACTER_STATE.Idle : CHARACTER_STATE.Walk;

        if (AttackOn && attackPossible)
            CurrentCharcterState = CHARACTER_STATE.Attack;
    }

    /// <summary>
    /// NOTE : 애니매이션 설정, enum 상태를 그대로 대입 하여 사용
    /// </summary>
    protected virtual void SetAnimation()
    {
        anim.SetFloat("StateFloat", (int)CurrentCharcterState);
    }

}