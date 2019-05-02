using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ORDER_STATE { Idle, Patroll, Trace, Attack }
public class Monster : MonoBehaviour
{
    //자신이 존재하는 방
    public DungeonRoom ownRoom = null;
    delegate void AlarmHandler();
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private ANIMATION_STATE InstanceState;
    private ANIMATION_STATE CurrentAnimState
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
    private ORDER_STATE InstanceOrderState;
    public ORDER_STATE OrderState
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

    public MONSTER_TYPE mType;
    public MonsterData mDATA = new MonsterData();

    private Rigidbody2D rb2D;
    private Animator anim;
    private SpriteRenderer sR;
    
    //Raycast    
    private int raycastLayerMask;
    [HideInInspector]
    public Transform targetOb = null;
    //HP
    private int instanceHP;
    public int CurrentHP
    {
        get { return instanceHP; }
        set
        {
            instanceHP = value;
            if (instanceHP >= mDATA.maxHP)
                instanceHP = mDATA.maxHP;
            else if (instanceHP < 0)
                instanceHP = 0;
        }
    }
    //Move
    private float currentMoveSpeed;
    //Attack
    private bool isRunningAttackCoroutine = false;
    private bool attackCooltimeState = false;
    private bool attackOn = false;
    private bool isFrontTarget = false;
    //KnockBack
    private bool isRunningKnockbackCoroutine = false;
    private bool isKnockbackState = false;
    //Stop

    [HideInInspector]
    public bool isAlive;
    private bool isGrounded = false;
    public bool isStopped;

    //HP UI
    [SerializeField]
    private MonsterHPSliderSc hpSliderUI;
    //몬스터가 보스로 등장할 경우 여기서 변경
    public bool isBoss = false;

    private void Awake()
    {
        isAlive = true;

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();

        //Set Layer 8 - tile, 9 - player
        raycastLayerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("Monster"));
        //Size
        if (isBoss)
        {
            //사이즈 변경 및 색깔 변경
            transform.localScale = Vector3.one * 3f;
            sR.color = new Color(1, 0.5f, 0.5f, 1);
        }

    }

    private void Start()
    {
        CSVDataReader.instance.SetData(mDATA, mType.ToString());
        OrderState = Random.Range(0, 100) > 35 ? ORDER_STATE.Idle : ORDER_STATE.Patroll;
        //Set HP
        CurrentHP = mDATA.maxHP;
        hpSliderUI = transform.GetComponentInChildren<MonsterHPSliderSc>();
        hpSliderUI.SetSliderStartValue(mDATA.maxHP, CurrentHP);
        hpSliderUI.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!isAlive)
            return;
        if (isStopped)
            return;
        if (isKnockbackState)
            return;

        switch (OrderState)
        {
            case ORDER_STATE.Idle:
                currentMoveSpeed = 0;
                break;
            case ORDER_STATE.Patroll:
                Patroll();
                break;
            case ORDER_STATE.Trace:
                Trace();
                break;
            case ORDER_STATE.Attack:
                Attack();
                break;
            default:
                Debug.Log("Monster Default State !");
                break;
        }
    }

    private void LateUpdate()
    {
        SetAnimationState();
    }

    /// <summary>
    /// NOTE : 구조물에서 길이 없거나 벽에 부딪혔을경우 방향 순회 (속도는 maxspeed의 절반)
    /// TODO : 벽에 부딪혔을 경우에는 점프를 하도록 구현 여지
    /// </summary>
    private void Patroll()
    {
        if (!isGrounded)
            return;
        //flip을 통한 dir 설정
        Vector2 dir = sR.flipX.Equals(true) ? -Vector2.right : Vector2.right;

        //벽 Raycast
        RaycastHit2D wallCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -((transform.localScale.y - 1) + 0.5f), 0), dir, transform.localScale.x + 0.5f, raycastLayerMask);
        if (wallCheckInfo.collider != null)
        {
            if (wallCheckInfo.collider.CompareTag("Ground") || wallCheckInfo.collider.CompareTag("Floor") || wallCheckInfo.collider.CompareTag("Monster"))
                sR.flipX = sR.flipX.Equals(true) ? false : true;
        }
        //길 끊김 Null Raycast
        RaycastHit2D nullCheckInfo = Physics2D.Raycast(transform.position, dir + new Vector2(0, -transform.localScale.y), transform.localScale.x + 0.5f, raycastLayerMask);
        if (nullCheckInfo.collider == null)
            sR.flipX = sR.flipX.Equals(true) ? false : true;

        rb2D.velocity = new Vector2(dir.x * currentMoveSpeed, rb2D.velocity.y);
    }

    /// <summary>
    /// NOTE : 추적 벽에 부딪힐 경우 점프
    /// </summary>
    private void Trace()
    {
        //flip을 통한 dir 설정
        Vector2 dir = sR.flipX.Equals(true) ? -Vector2.right : Vector2.right;
        //벽 Raycast (아래를 훑어야하긴하는데)
        RaycastHit2D frontCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -((transform.localScale.y - 1) + 0.2f), 0), dir, transform.localScale.x, raycastLayerMask);

        if (frontCheckInfo.collider != null)
        {
            if (frontCheckInfo.collider.CompareTag("Ground") || frontCheckInfo.collider.CompareTag("Floor"))
                Jump();
            else if (frontCheckInfo.collider.CompareTag("Player"))
                OrderState = ORDER_STATE.Attack;
            else if (frontCheckInfo.collider.CompareTag("Monster"))
                dir = Vector2.zero;
        }

        sR.flipX = (int)(transform.position.x) > (int)(targetOb.position.x) ? true : false;

        rb2D.velocity = new Vector2(dir.x * currentMoveSpeed, rb2D.velocity.y);
    }

    /// <summary>
    /// NOTE : JUMP 
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
        {
            if (((int)rb2D.velocity.y).Equals(0))
                rb2D.AddForce(Vector2.up * mDATA.jumpPower, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// NOTE : 공격상태일 경우 RAYCAST를 통해 플레이어가 앞에 존재하는지 체크
    /// </summary>
    private void Attack()
    {
        if (!((int)rb2D.velocity.x).Equals(0))
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        if (!attackCooltimeState)
            attackOn = true;

        RaycastHit2D targetCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -0.5f, 0), transform.right, 1.5f, raycastLayerMask);

        if (targetCheckInfo.collider != null)
            isFrontTarget = targetCheckInfo.collider.CompareTag("Player") ? true : false;
        else
            isFrontTarget = false;
    }

    /// <summary>
    /// NOTE : Animation ADD EVENT FUNCTION
    /// NOTE : ATTACK함수에서 RAYCAST를 통하여 앞에 플레이어가 멀어졌을 경우 다시 TRACE 상태로 변경 
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttackCoroutine()
    {
        isRunningAttackCoroutine = true;
        attackCooltimeState = true;
        if (!isFrontTarget)
        {
            OrderState = ORDER_STATE.Trace;
            attackOn = false;
        }
        yield return new WaitForSeconds(mDATA.attackCoolTime);

        attackCooltimeState = false;
        isRunningAttackCoroutine = false;
    }
    /// <summary>
    /// NOTE : 데미지를 입었을때 처리
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage, Transform targetpos)
    {
        if (!isAlive)
            return;

        CurrentHP -= damage;
        //체력 UI 시작 및 설정
        if (!hpSliderUI.isActiveAndEnabled)
            hpSliderUI.gameObject.SetActive(true);
        hpSliderUI.SetHPValue(CurrentHP);

        anim.SetTrigger("TakeDamage");
        KnockBack(targetpos);
        if (CurrentHP <= 0)
            Die();
    }

    /// <summary>
    /// NOTE : 넉백, 플레이어와 부딪혔을 때나 공격 당했을 때
    /// </summary>
    /// <param name="targetpos"></param>
    private void KnockBack(Transform targetpos)
    {
        if (!isRunningKnockbackCoroutine)
            StartCoroutine(KnockbackCoroutine(targetpos));
    }

    protected void Skill()
    {
        
    }

    /// <summary>
    /// NOTE : 넉백 물리 실행 및 설정 시간 이후 상태 변경 
    /// </summary>
    /// <returns></returns>
    IEnumerator KnockbackCoroutine(Transform targetpos)
    {
        //상태 변경
        isRunningKnockbackCoroutine = true;
        isKnockbackState = true;
        //KncokBack Action
        rb2D.velocity = Vector2.zero;
        float xdir = Mathf.Sign(transform.position.x - targetpos.position.x);
        float ydir = Mathf.Sign(transform.position.y - targetpos.position.y).Equals(1) ? 1f : -1f;
        Vector2 dir = new Vector2(xdir, ydir);
        rb2D.AddForce(dir * mDATA.knockBackPower, ForceMode2D.Impulse);

        //점멸이펙트
        float timer = 0f;
        Color tmpcolor = sR.color;
        while (timer <= mDATA.knockbackTime)
        {
            timer += 0.1f;
            tmpcolor.a = tmpcolor.a.Equals(1f) ? 0.2f : 1f;
            sR.color = tmpcolor;
            yield return new WaitForSeconds(0.1f);
        }
        //상태 리셋
        isKnockbackState = false;
        isRunningKnockbackCoroutine = false;
    }

    /// <summary>
    /// NOTE : 애니매이션 Die실행
    /// </summary>
    private void Die()
    {
        isAlive = false;
        anim.SetTrigger("Die");
        anim.GetComponent<BoxCollider2D>().isTrigger = true;
        StartCoroutine(ActiveOff());
    }
    /// <summary>
    /// NOTE : 해당 시간 후에 삭제
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveOff()
    {
        yield return new WaitForSeconds(2f);
        ownRoom.CheckLockRoom();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// NOTE : 해당시간만큼 몬스터 행동을 정지 시키기 위함
    /// </summary>
    /// <param name="_stopcount"></param>
    public void StopAction(float _stopcount)
    {
        StartCoroutine(StopCoroutine(_stopcount));
    }

    private IEnumerator StopCoroutine(float _stopcount)
    {
        isStopped = true;
        yield return new WaitForSeconds(_stopcount);
        isStopped = false;
    }

    /// <summary>
    /// 캐릭터 상태 설정
    /// NOTE : STATE 우선 순위
    /// 1. ATTACK 2. JUMP 3. FALL 4. MOVE
    /// </summary>
    private void SetAnimationState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentAnimState = ANIMATION_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentAnimState = ANIMATION_STATE.Fall;
        else
            CurrentAnimState = (int)(currentMoveSpeed * 10) == 0 ? ANIMATION_STATE.Idle : ANIMATION_STATE.Walk;

        if (attackOn)
            CurrentAnimState = ANIMATION_STATE.Attack;
        
        anim.SetFloat("StateFloat", (int)CurrentAnimState);
    }

    #region COLLISION
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

        if (collision.collider.CompareTag("Player"))
        {
            if (isAlive)
                KnockBack(collision.transform);
        }

    }

    /// <summary>
    /// NOTE : GROUND CHECK FALSE
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackEffect"))
            TakeDamage(collision.GetComponent<AttackEffectSc>().damage, collision.transform);
    }
}
