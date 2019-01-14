using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ANIMATION_STATE { Idle = 0, Walk, Jump, Fall, Attack, Die }
/// <summary>
/// NOTE : 플레이어캐릭터 공격 점프 이동
/// </summary>
[RequireComponent(typeof(Rigidbody2D),typeof(Animator))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private ANIMATION_STATE InstanceState;
    private ANIMATION_STATE CurrentPlayerState
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
    public bool isAlive = false;
    [HideInInspector]
    public bool JumpOn, AttackOn, StopOn = false;

    private bool isRunningJumpCoroutine = false;
    private bool isRunningAttackCoroutine = false;
    private bool isRunningStopCoroutine = false;

    private FloorCheckSc floorCheckPrefab;
    private bool IsGrounded{ get { return floorCheckPrefab.isGrounded; } }
    private bool isHitWall = false;

    //가속 감속 처리하기 위함
    [HideInInspector]
    public float currentMoveInputValue;
    
    protected float prevMoveInputValue = 0;
    private float currentMoveSpeed;

    private bool attackPossibleOn = true;

    protected void Awake()
    {
        floorCheckPrefab = transform.GetComponentInChildren<FloorCheckSc>();
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected void Update()
    {
        HitWallCheck();
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
        SetCharacterState();
        SetAnimation();
    }

    private void HitWallCheck()
    {
        RaycastHit2D hitwallinfo = Physics2D.Raycast(transform.position + (transform.right*1.3f) + (Vector3.down*-0.5f), transform.right, 0.2f);
        
        if (hitwallinfo.collider == null)
            isHitWall = false;
        else
        {
            if (hitwallinfo.collider.CompareTag("Ground") || hitwallinfo.collider.CompareTag("Floor"))
                isHitWall = true;
            else
                isHitWall = false;
        }
    }


    /// <summary>
    /// NOTE : 플레이어 캐릭터 이동 함수
    /// NOTE : 최대 속도 - 키 입력값 * maxSpeedValue (프레임당 키입력 값은 (0~1))
    /// FIXME : rb2D.velocity.y 플레이어가 떨어지면서 땅에 안착했을때 0이 되지않고 -5.0938같은 값으로 처리될 때가 있어서 임시 방편으로 (int)형으로 캐스팅 수정
    /// </summary>
    protected void Move()
    {
        float maxspeed = currentMoveInputValue * pDATA.maxSpeedValue;
        currentMoveSpeed += (currentMoveInputValue * pDATA.accelerationValue);
        
        //우측방향 가속
        if (currentMoveInputValue > 0)
        {
            //이전 프레임 값과 비교하여 증감
            if (prevMoveInputValue <= currentMoveInputValue && currentMoveSpeed >= maxspeed)
                currentMoveSpeed = maxspeed;
        }
        //좌측방향 가속
        else if (currentMoveInputValue < 0)
        {
            if (prevMoveInputValue >= currentMoveInputValue && currentMoveSpeed <= maxspeed)
                currentMoveSpeed = maxspeed;
        }
        //감속
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, 0, Time.deltaTime * pDATA.decelerationValue);
        }
        if (isHitWall)
            currentMoveSpeed = 0;

        Vector3 dirvector = new Vector3(currentMoveSpeed, 0, 0);
        transform.position += dirvector * Time.deltaTime;
        prevMoveInputValue = currentMoveInputValue;
        
        //캐릭터의 방향 설정
        if (!currentMoveInputValue.Equals(0f))
            transform.localEulerAngles = currentMoveInputValue > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }

    /// <summary>
    /// NOTE : 코루틴 함수가 현재 실행중인지 체크한 후 실행
    /// NOTE : IsGrounded는 캐릭터 오브젝트의 자식오브젝트의 트리거함수로 설정됨
    /// </summary>
    protected void Jump()
    {
        if (JumpOn)
        {
            if (IsGrounded)
            {
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
    /// NOTE : STATE
    /// 1. ATTACK 2. JUMP 3. FALL 4. MOVE
    /// </summary>
    protected void SetCharacterState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentPlayerState = ANIMATION_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentPlayerState = ANIMATION_STATE.Fall;
        else
            CurrentPlayerState = (int)(currentMoveSpeed * 10) == 0 ? ANIMATION_STATE.Idle : ANIMATION_STATE.Walk;

        if (AttackOn && attackPossibleOn)
            CurrentPlayerState = ANIMATION_STATE.Attack;
    }

    /// <summary>
    /// NOTE : 애니매이션 설정, enum 상태를 그대로 대입 하여 사용
    /// </summary>
    protected void SetAnimation()
    {
        anim.SetFloat("StateFloat", (int)CurrentPlayerState);
    }
}

