using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ORDER_STATE { Idle, Patrol, Chase, Attack, Die}
public class Monster : MonoBehaviour
{
    //자신이 존재하는 방
    public DungeonRoom ownRoom = null;
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private ANIMATION_STATE InstanceState;
    protected ANIMATION_STATE CurrentAnimState
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
                case ORDER_STATE.Patrol:
                    currentMoveSpeed = mDATA.patrollSpeed;
                    break;
                case ORDER_STATE.Chase:
                    currentMoveSpeed = mDATA.traceSpeed;
                    break;
            }
        }

    }
    //Type Data
    public MONSTER_TYPE mType;
    public MonsterData mDATA = new MonsterData();
    //Component
    protected Rigidbody2D rb2D;
    private Animator anim;
    protected SpriteRenderer sR;
    
    //Raycast    
    private int raycastLayerMask;
    [HideInInspector]
    protected Transform targetOb = null;
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
    protected float currentMoveSpeed;
    protected Vector2 characterDir = Vector2.zero;
    //Attack
    private bool isRunningAttackCoroutine = false;
    protected bool attackCooltimeState = false;
    protected bool attackOn = false;
    //KnockBack
    private bool isRunningKnockbackCoroutine = false;
    private bool isKnockbackState = false;
    //Stop

    [HideInInspector]
    public bool isAlive;
    public bool isStopped;

    private float traceOffDistance = 25f;
    private float traceOnDistance = 7f;
    //HP UI
    [SerializeField]
    private MonsterHPSliderSc hpSliderUI;
    //몬스터가 보스로 등장할 경우 여기서 변경
    public bool isBoss = false;
    
    protected virtual void Awake()
    {
        isAlive = true;

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
        
        //Set Layer 8 - tile, 9 - player
        //raycastLayerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("Monster"));
        //Size
        if (isBoss)
        {
            //사이즈 변경 및 색깔 변경
            transform.localScale = Vector3.one * 3f;
            sR.color = new Color(1, 0.5f, 0.5f, 1);
        }
        CSVDataReader.instance.SetData(mDATA, mType.ToString());
    }

    protected virtual void Start()
    {
        targetOb = GameObject.FindWithTag("Player").transform;
        
        OrderState = Random.Range(0, 100) > 100 ? ORDER_STATE.Idle : ORDER_STATE.Patrol;
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
        AggroCheck();
        switch (OrderState)
        {
            case ORDER_STATE.Idle:
                currentMoveSpeed = 0;
                break;
            case ORDER_STATE.Patrol:
                Patrol();
                break;
            case ORDER_STATE.Chase:
                Chase();
                break;
            case ORDER_STATE.Attack:
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
    /// NOTE : 캐릭터 정면으로 발사하는RAY를 통하여 플레이어를 발견하였을 경우 추적
    /// </summary>
    private void AggroCheck()
    {
        if (OrderState.Equals(ORDER_STATE.Chase))
            return;

        Vector2 dir = sR.flipX.Equals(true) ? -Vector2.right : Vector2.right;
        Debug.DrawRay(transform.position, dir *traceOnDistance, Color.green, 0.1f, false);
        RaycastHit2D checkRay = Physics2D.Raycast(transform.position, dir, traceOnDistance);

        if(checkRay.collider!=null)
        {
            if (checkRay.collider.CompareTag("Player"))
                ChaseON(checkRay.collider.transform);
        }
    }

    /// <summary>
    /// NOTE : TARGET 설정 ORDER STATE TRACE 로 변경
    /// </summary>
    /// <param name="target"></param>
    protected virtual void ChaseON(Transform target)
    {
        if (OrderState.Equals(ORDER_STATE.Chase))
            return;
        if(targetOb==null)
            targetOb = target;
        OrderState = ORDER_STATE.Chase;
    }
    
    /// <summary>
    /// NOTE : 일정 거리 이상으로 벌어질 경우 Trace Off
    /// </summary>
    protected void ChaseOFF()
    {
        float dis = Vector2.Distance(transform.position, targetOb.transform.position);
        if(dis>traceOffDistance)
            OrderState = Random.Range(0, 100) > 35 ? ORDER_STATE.Idle : ORDER_STATE.Patrol;
    }

    /// <summary>
    /// NOTE : 경비
    /// </summary>
    protected virtual void Patrol()
    {
        characterDir = sR.flipX.Equals(true) ? -Vector2.right : Vector2.right;
        //벽 Raycast
        RaycastHit2D wallCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -((transform.localScale.y - 1) + 0.5f), 0), characterDir, transform.localScale.x + 0.5f);
        if (wallCheckInfo.collider != null)
        {
            if (wallCheckInfo.collider.CompareTag("Ground") || wallCheckInfo.collider.CompareTag("Floor") || wallCheckInfo.collider.CompareTag("Garbage") || wallCheckInfo.collider.CompareTag("Box"))
                sR.flipX = sR.flipX.Equals(true) ? false : true;
        }
    }

    /// <summary>
    /// NOTE : 추적 
    /// </summary>
    public virtual void Chase(){ }

    /// <summary>
    /// NOTE : JUMP 
    /// </summary>
    protected virtual void Jump() { }

    /// <summary>
    /// NOTE : Attack
    /// </summary>
    protected virtual void Attack()
    {
        attackOn = true;
        currentMoveSpeed = 0;
    }

    public virtual void AttackEffect()
    {

    }
    /// <summary>
    /// NOTE : Animation ADD EVENT FUNCTION
    /// NOTE : ATTACK함수에서 RAYCAST를 통하여 앞에 플레이어가 멀어졌을 경우 다시 TRACE 상태로 변경 
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator AttackCoolTimeCoroutine()
    {
        isRunningAttackCoroutine = true;
        attackCooltimeState = true;
        attackOn = false;
        yield return new WaitForSeconds(mDATA.attackCoolTime);
        attackCooltimeState = false;
        isRunningAttackCoroutine = false;
    }

    /// <summary>
    /// NOTE : 데미지를 입었을때 처리
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage, Transform targetpos ,Vector3 collisionpos)
    {
        if (!isAlive)
            return;
        ChaseON(targetpos);
        CurrentHP -= damage;
        //체력 UI 시작 및 설정
        if (!hpSliderUI.isActiveAndEnabled)
            hpSliderUI.gameObject.SetActive(true);
        hpSliderUI.SetHPValue(CurrentHP);

        anim.SetTrigger("TakeDamage");
        KnockBack(collisionpos);
        if (CurrentHP <= 0)
            Die();
    }

    /// <summary>
    /// NOTE : 넉백, 플레이어와 부딪혔을 때나 공격 당했을 때
    /// </summary>
    /// <param name="targetpos"></param>
    protected void KnockBack(Vector3 collisionpos)
    {
        if (!isRunningKnockbackCoroutine)
            StartCoroutine(KnockbackCoroutine(collisionpos));
    }
    
    /// <summary>
    /// NOTE : 넉백 물리 실행 및 설정 시간 이후 상태 변경 
    /// </summary>
    /// <returns></returns>
    IEnumerator KnockbackCoroutine(Vector3 collisionpos)
    {
        //상태 변경
        isRunningKnockbackCoroutine = true;
        isKnockbackState = true;
        //KncokBack Action
        rb2D.velocity = Vector2.zero;
        float xdir = Mathf.Sign(transform.position.x - collisionpos.x);
        float ydir = Mathf.Sign(transform.position.y - collisionpos.y).Equals(1) ? 1f : -1f;
        Vector2 dir = new Vector2(xdir, ydir);
        //rb2D.AddForce(dir * mDATA.knockBackPower, ForceMode2D.Impulse);

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
    protected virtual void Die()
    {
        isAlive = false;
        anim.SetTrigger("Die");
        OrderState = ORDER_STATE.Die;
        anim.GetComponent<CircleCollider2D>().isTrigger = true;
        StartCoroutine(ActiveOff());
    }
    /// <summary>
    /// NOTE : 해당 시간 후에 삭제
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveOff()
    {
        yield return new WaitForSeconds(2f);

        if (isBoss)
            ownRoom.BossClearCheck();
        else
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
    protected virtual void SetAnimationState()
    {
        if (attackOn&&!attackCooltimeState)
            CurrentAnimState = ANIMATION_STATE.Attack;
        
        anim.SetFloat("StateFloat", (int)CurrentAnimState);
    }

    #region colision
    /// <summary>
    /// NOTE : FloorCheck
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (isAlive)
            {
                collision.transform.GetComponent<Player>().TakeDamage(mDATA.bodyAttacktDamage, transform);
                ChaseON(collision.transform);
                KnockBack(collision.contacts[0].point);
            }
        }
    }

    /// <summary>
    /// Note : Attack 체크
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Attack();
            sR.flipX = transform.position.x - collision.transform.position.x > 0 ? true : false;
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!isAlive)
            return;
        if (collision.CompareTag("Player"))
            ChaseON(targetOb);
    }
    #endregion
}
