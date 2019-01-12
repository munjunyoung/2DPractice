using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTER_STATE { Idle = 0, Walk, Jump, Fall, Attack }

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
    /// NOTE : 애니매이션 속도 설정
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private CHARACTER_STATE InstanceState;
    protected CHARACTER_STATE CurrentPlayerState
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
    protected float currentSpeed;

    /// <summary>
    /// Require Component
    /// </summary>
    protected Rigidbody2D rb2D;
    protected Animator anim;

    [HideInInspector]
    public bool isGrounded, isAlive = false;
    
    [HideInInspector]
    public float currentMoveInputValue;
    protected float prevMoveInputValue = 0;

    private bool isRunningJumpCoroutine = false;
    private bool isRunningAttackCoroutine = false;

    [HideInInspector]
    public bool JumpButtonOn, AttackButtonOn, allStop = false;

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
    //JUMP - 해당 함수 추가
    //ATTACK - 이펙트 추가시
    //SET CAHRACTERSTATE 
    //SET ANIMATION

    /// <summary>
    /// NOTE : 플레이어 캐릭터 이동 함수
    /// NOTE : 최대 속도 - 키 입력값 * maxSpeedValue (프레임당 키입력 값은 (0~1))
    /// FIXME : rb2D.velocity.y 플레이어가 떨어지면서 땅에 안착했을때 0이 되지않고 -5.0938같은 값으로 처리될 때가 있어서 임시 방편으로 (int)형으로 캐스팅 수정
    /// </summary>
    protected virtual void Move(float xinputvalue)
    {
        float maxspeed = xinputvalue * maxSpeedValue;
        currentSpeed += (xinputvalue * accelerationValue);


        //우측방향 가속
        if (xinputvalue > 0)
        {
            //이전 프레임 값과 비교하여 증감
            if (prevMoveInputValue <= xinputvalue && currentSpeed >= maxspeed)
                currentSpeed = maxspeed;
        }
        //좌측방향 가속
        else if (xinputvalue < 0)
        {
            if (prevMoveInputValue >= xinputvalue && currentSpeed <= maxspeed)
                currentSpeed = maxspeed;
        }
        //감속
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * decelerationValue);
        }
        transform.position += new Vector3(currentSpeed, 0, 0) * Time.deltaTime;
        prevMoveInputValue = xinputvalue;

        //캐릭터의 방향 설정
        if (!xinputvalue.Equals(0f))
            transform.localEulerAngles = xinputvalue > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }

    /// <summary>
    /// NOTE : 코루틴 함수가 현재 실행중인지 체크한 후 실행
    /// NOTE : IsGrounded는 캐릭터 오브젝트의 자식오브젝트의 트리거함수로 설정됨
    /// </summary>
    protected virtual void Jump()
    {
        if(isGrounded && isRunningJumpCoroutine)
            StartCoroutine(JumpCoroutine());
    }

    /// <summary>
    /// NOTE : JUMP()함수 실행되는 Coroutine
    /// TODO : 버튼이나 해당 키가 눌려있음을 체크하여 JumpCount만큼 실행, 다른 방법으로도 구현할 가능성이 있음
    /// </summary>
    protected virtual IEnumerator JumpCoroutine()
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
}