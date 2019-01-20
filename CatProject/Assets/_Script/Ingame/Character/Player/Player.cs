using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ANIMATION_STATE { Idle = 0, Walk, Jump, Fall, Attack, Die }
/// <summary>
/// NOTE : 플레이어캐릭터 공격 점프 이동
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private ANIMATION_STATE InstanceState;
    private ANIMATION_STATE CurrentPlayerAnimState
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
                    anim.speed = pDATA.attackAnimSpeed;
                    break;
                default:
                    anim.speed = 1;
                    break;
            }
        }
    }

    [Header("PLAYER DATA SET"), SerializeField]
    private PlayerData pDATA;

    private Rigidbody2D rb2D;
    private Animator anim;

    [HideInInspector]
    public bool MoveOn, JumpOn, AttackOn, StopOn = false;

    private bool isRunningJumpCoroutine = false;
    private bool isRunningAttackCoroutine = false;
    private bool isRunningStopCoroutine = false;

    private int instanceHealth;
    public int CurrentHealth
    {
        get { return instanceHealth; }
        set
        {
            instanceHealth = value;
            if (instanceHealth >= pDATA.MaxHealth)
                instanceHealth = pDATA.MaxHealth;
            else if (instanceHealth < 0)
                instanceHealth = 0;

        }
    }

    //가속 감속 처리하기 위함
    [HideInInspector]
    public float currentMoveInputValue;
    protected float prevMoveInputValue = 0;
    private float currentMoveSpeed;

    private bool isGrounded = false;

    private bool attackPossibleOn = true;

    protected void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        Debug.Log(Mathf.Sign(-1));
    }

    protected void FixedUpdate()
    {
        if (StopOn)
            return;
        Move();
        Jump();
    }

    private void LateUpdate()
    {
        SetAnimationState();
    }

    /// <summary>
    /// NOTE : 플레이어 캐릭터 이동 함수
    /// NOTE : 최대 속도 -> 키 입력값 * maxSpeedValue (프레임당 키입력 값은 (0~1))
    /// FIXME : rb2D.velocity.y 플레이어가 떨어지면서 땅에 안착했을때 0이 되지않고 -5.0938같은 값으로 처리될 때가 있어서 임시 방편으로 (int)형으로 캐스팅 수정
    /// </summary>
    protected void Move()
    {
        float maxspeed = currentMoveInputValue * pDATA.maxSpeedValue;
        currentMoveSpeed = AccelerationCalc(currentMoveSpeed, maxspeed);
        rb2D.velocity = new Vector2(currentMoveSpeed, rb2D.velocity.y);
        //캐릭터의 방향 설정
        if (!currentMoveInputValue.Equals(0f))
            transform.localEulerAngles = currentMoveInputValue > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }

    /// <summary>
    /// NOTE : 가속도 [ 현재속도 = 현재속도 + 가속값 * 방향 * Time]
    /// NOTE : 가속할떄 가속력과 감속할때 감속력이 다르다
    /// </summary>
    /// <param name="currentspeed"></param>
    /// <param name="maxspeed"></param>
    /// <returns></returns>
    protected float AccelerationCalc(float currentspeed, float maxspeed)
    {
        if (currentspeed.Equals(maxspeed))
            return currentspeed;
        
        float dir = Mathf.Sign(maxspeed - currentspeed);
        float accelrate = maxspeed.Equals(0) ? pDATA.decelerationValue : pDATA.accelerationValue;
        currentspeed += dir * accelrate * Time.deltaTime;
        return (dir.Equals(Mathf.Sign(maxspeed - currentspeed))) ? currentspeed : maxspeed;
    }
    /// <summary>
    /// NOTE : 코루틴 함수가 현재 실행중인지 체크한 후 실행
    /// NOTE : IsGrounded는 캐릭터 오브젝트의 자식오브젝트의 트리거함수로 설정됨
    /// </summary>
    protected void Jump()
    {
        if (JumpOn)
        {
            if (isGrounded)
            {
                if (((int)rb2D.velocity.y).Equals(0))
                    //GroundCheck && isrunningjumpcoroutine의 경우에는 2번 점프 되는경우가 생겨서 묶음)
                    if (!isRunningJumpCoroutine)
                        StartCoroutine(JumpCoroutine());
            }
        }
    }

    /// <summary>
    /// NOTE : JUMP()함수 실행되는 Coroutine
    /// TODO : 버튼이나 해당 키가 눌려있음을 체크하여 JumpCount만큼 실행, 다른 방법으로도 구현할 가능성이 있음
    /// </summary>
    private IEnumerator JumpCoroutine()
    {
        isRunningJumpCoroutine = true;
        float jumpCount = 0;
        while (JumpOn && jumpCount <= pDATA.jumpMaxCount)
        {
            jumpCount++;
            rb2D.AddForce(Vector2.up * pDATA.jumpPowerPerCount, ForceMode2D.Impulse);
            yield return new WaitForSeconds(pDATA.addForceFrameIntervalTime);
        }
        isRunningJumpCoroutine = false;
    }

    #region ATTACK ANIMATION ADD EVENT FUNCTION
    /// <summary>
    /// NOTE : 자식으로 저장된 attackEffectModel을 on, off하는 형식
    /// NOTE : ATTACK ANIMATION 클립 내부 add Event에서 출력
    /// TODO : 현재 애니매이션 클립내부에서 실행되기 때문에 클립이 변경된다면 함수 선언위치 등 고려가능성이 높음
    /// </summary>
    public void AttackEffectOn()
    {
        pDATA.attackEffectModel.SetActive(true);
    }
    public void AttackEffectOff()
    {
        pDATA.attackEffectModel.SetActive(false);
    }
    /// <summary>
    /// NOTE : ATTACK 상태 실행시 코루틴
    /// NOTE : ATTACK ANIMATION 클립 내부 add Event에서 출력 
    /// NOTE : 공격 실행 후 카운트 실행 설정한ATTACK COOLTIME값 이후 ATTACK Possible true로 변경
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttackCoroutine()
    {
        isRunningAttackCoroutine = true;
        attackPossibleOn = false;
        yield return new WaitForSeconds(pDATA.attackCoolTime);
        attackPossibleOn = true;
        isRunningAttackCoroutine = false;
    }
    #endregion

    /// <summary>
    /// NOTE : STOP코루틴 함수 실행
    /// NOTE : 외부 함수에서 실행하는 경우가 있어서 public 선언
    /// </summary>
    /// <param name="stoptime"></param>
    public void StopCharacter(float stoptime)
    {
        JumpOn = false;
        AttackOn = false;
        currentMoveSpeed = 0;
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
    /// NOTE : STATE 우선 순위
    /// 1. ATTACK 2. JUMP 3. FALL 4. MOVE
    /// </summary>
    protected void SetAnimationState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentPlayerAnimState = ANIMATION_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentPlayerAnimState = ANIMATION_STATE.Fall;
        else
            CurrentPlayerAnimState = (int)(currentMoveSpeed * 10) == 0 ? ANIMATION_STATE.Idle : ANIMATION_STATE.Walk;

        if (AttackOn && attackPossibleOn)
            CurrentPlayerAnimState = ANIMATION_STATE.Attack;

        anim.SetFloat("StateFloat", (int)CurrentPlayerAnimState);
    }

    /// <summary>
    /// NOTE : FloorCheck
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 contactnormalSum = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            contactnormalSum += collision.contacts[i].normal;

        if (contactnormalSum.y > 0)
            isGrounded = true;
    }

    /// <summary>
    /// Exit처리가 하나라도되면 IsGrounded 체크
    /// TODO : EXIT되는 contact가 표시되지 않고 collider tag는 체크되나
    /// floor태그에 isgrounded를 false하게되면 옆면에서 부딪힌 floor에서 떨어질떄도 체크가 해제됨
    /// 또한 floor뿐만아니라 다른 오브젝트에서도 점프를 가능하게 해야하기때문에 현재 무언가 위에 존재하고있는지 체크가 가능하다.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}