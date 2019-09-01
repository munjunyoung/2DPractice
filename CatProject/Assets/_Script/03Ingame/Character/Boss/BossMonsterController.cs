using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOSS_TYPE { Person }
public enum BOSS_ANIMATION_STATE { Idle, Walk, Jump, Fall, Attack, Skill ,TakeDamage, Die}
public class BossMonsterController : MonoBehaviour
{
    public    BOSS_TYPE         bType;
    protected BossData          bData   = new BossData();

    public    DungeonRoom       ownRoom = null;
    protected Transform         TargetOB;
    
    private   BOSS_ANIMATION_STATE _CurrentAnimState;
    protected BOSS_ANIMATION_STATE CurrentAnimState
    {
        get { return _CurrentAnimState; }
        set { _CurrentAnimState = value;}
    }

    
    protected SpriteRenderer        sR;
    protected Rigidbody2D           rb2D;
    protected BoxCollider2D         col;
    protected Color                 normalColor;
    protected Color                 frenzyColor;
    protected Animator              anim;
    protected BT_BossPersonAI       aiSc;
    protected MonsterAttackEffectSc attackEffect;
    [SerializeField]
    protected MonsterHPSliderSc hpSlider;

    protected float             currentMoveSpeed;
    protected float             currentAttackSpeed;
    protected float             flashCount;

    private   int               instanceHP;
    protected int               CurrentHP
    {
        get { return instanceHP; }
        set
        {
            instanceHP = value;
            if (instanceHP >= bData.HP)
            {
                instanceHP = bData.HP;
            }
            else if (instanceHP < 0)
            {
                instanceHP = 0;
                isAlive = false;
            }

            if (hpSlider != null)
                hpSlider.SetHPValue(instanceHP);
        }
    }

    public    bool              isAlive                     = true;
    protected bool              isPause                     = false;
    protected bool              isAleadyFindingTarget       = false;
    protected bool              isCloseTarget               = false;
    protected bool              isStartingAttack            = false;
    protected bool              isLookatTarget              = false;
    protected bool              isReadyAttack               = true;
    protected bool              isRunningTakeDamageFlash    = false;
    protected bool              isFrenzyState               = false;


    //BossData
    public virtual void Init()
    {
        TargetOB = GameObject.FindWithTag("Player").transform;

        sR = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        aiSc = GetComponent<BT_BossPersonAI>();
        //능력 데이터 초기화
        CSVDataReader.instance.SetData(bData, bType.ToString());
        //체력, 체력바 초기화
        CurrentHP = bData.HP;
        hpSlider.SetSliderStartValue(bData.HP, CurrentHP);

        attackEffect = transform.Find("AttackEffect").GetComponent<MonsterAttackEffectSc>();
        attackEffect.SetActiveOff();
        attackEffect.damage = 200;// bData.attackDamage;

        normalColor = sR.color;
        frenzyColor = Color.red;
    }

    protected virtual void LateUpdate()
    {
        SetAnimationState(); 
    }

    #region BT FUNC
    public virtual void IdleAction() { }
    public virtual bool DetectTarget() { return true; }
    
    public virtual void ChaseAction() { }
    public virtual bool CheckCloseTarget() { return true; }
    public virtual bool LookAtTarget() { return true; }
    public virtual bool CheckPossibleAttack() { return true; }
    public virtual void StartAttack() { }
    
    public virtual bool CheckPossibleSkill() { return true; }
    public virtual void SkillAction() { }

    public virtual bool isDie() { return !isAlive; }
    public virtual void DeadAction() { }
    #endregion

    /// <summary>
    /// NOTE : 광폭화
    /// </summary>
    protected virtual void FrenzyAction() { }
    /// <summary>
    /// NOTE : 애니매이션 StateFloat 값 설정
    /// </summary>
    protected virtual void SetAnimationState()
    {
        if (isPause)
            return;
        if (!isAlive)
            return;

        if ((int)rb2D.velocity.y != 0)
            CurrentAnimState = BOSS_ANIMATION_STATE.Jump;
        else
            CurrentAnimState = (int)(rb2D.velocity.x) == 0 ? BOSS_ANIMATION_STATE.Idle : BOSS_ANIMATION_STATE.Walk;

        if (isStartingAttack)
            CurrentAnimState = BOSS_ANIMATION_STATE.Attack;
        anim.SetFloat("StateFloat", (int)CurrentAnimState);
    }
    
    /// <summary>
    /// NOTE : 체력 설정 및 캐릭터 점멸 애니매이션
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (!isAlive)
            return;

        //공격을 당했을 경우 (멀리서 스킬로 공격 하여 어그로를 무시하는 경우 방지)
        isAleadyFindingTarget = true;

        CurrentHP -= damage;
        //hpSlider.SetHPValue(CurrentHP);

        if (!isRunningTakeDamageFlash)
            StartCoroutine(TakeDamageFlashProcess());
        else
            flashCount = 0;
    }

    /// <summary>
    /// NOTE : 스프라이트 이미지 a 점멸
    /// </summary>
    /// <param name="flashCount"></param>
    /// <returns></returns>
    protected virtual IEnumerator TakeDamageFlashProcess()
    {
        isRunningTakeDamageFlash = true;
        flashCount = 0;
        Color tmpcolor = normalColor;
        while (flashCount < 1f)
        {
            flashCount += 0.1f;
            tmpcolor.a = tmpcolor.a.Equals(1f) ? 0.2f : 1f;
            sR.color = tmpcolor;
            yield return new WaitForSeconds(0.1f);
        }
        sR.color = normalColor;
        isRunningTakeDamageFlash = false;
    }

    /// <summary>
    /// NOTE : 공격 애니매이션 클립 이벤트 정의 함수 : attackEffect SetOn
    /// </summary>
    public void SetActiveAttackEffect()
    {
        attackEffect.gameObject.SetActive(true);
    }
    /// <summary>
    /// NOTE : Animation ADD EVENT FUNCTION
    /// NOTE : ATTACK함수에서 RAYCAST를 통하여 앞에 플레이어가 멀어졌을 경우 다시 TRACE 상태로 변경 
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator AttackCoolTimeProcess()
    {
        isReadyAttack = false;
        isStartingAttack = false;
        yield return new WaitForSeconds(bData.attackCooltime);
        isReadyAttack = true;
    }
    
    /// <summary>
    /// NOTE : 파라미터 시간만큼 일시정지 코루틴 실행
    /// </summary>
    /// <param name="_stopcount"></param>
    public void PauseCharacter(float _stopcount)
    {
        StartCoroutine(PauseProcess(_stopcount));
    }

    /// <summary>
    /// NOTE : 파라미터 시간 만큼 일시 정지
    /// </summary>
    /// <param name="_stopcount"></param>
    /// <returns></returns>
    IEnumerator PauseProcess(float _stopcount)
    {
        isPause = true;
        aiSc.StopBT();
        yield return new WaitForSeconds(_stopcount);
        isPause = false;
        aiSc.StartBT();
    }

    /// <summary>
    /// NOTE : 애니매이션 상태 변경 및 RoomCheck 
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DeadProcess()
    {
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(2f);
        ownRoom.CheckLockRoom();
        gameObject.SetActive(false);
    }
    /// <summary>
    /// NOTE : 설정한 Collider 플레이어 근접 체크 확인
    /// </summary>
    /// <param name="collision"></param>
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isCloseTarget = true;
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isCloseTarget = false;
    }

    /// <summary>
    /// NOTE : 플레이어와 부딪혔을 경우 Damage 처리
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (isAlive)
                collision.transform.GetComponent<Player>().TakeDamage(bData.bodyAttackDamage, transform);
        }
    }

    
}
