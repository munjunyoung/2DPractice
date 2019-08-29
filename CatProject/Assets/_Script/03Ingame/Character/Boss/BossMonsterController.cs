using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOSS_TYPE { Person }
public enum BOSS_SEQUENCE { Normal ,Phase1, Phase2, Dead }
public enum BOSS_ANIMATION_STATE { Idle, Walk, Jump, Fall, Attack, Skill ,TakeDamage, Die}
public class BossMonsterController : MonoBehaviour
{
    protected BOSS_TYPE         bType;
    protected BossData          bData   = new BossData();

    public    DungeonRoom       ownRoom = null;
    protected Transform         TargetOB;
    
    private   BOSS_ANIMATION_STATE _CurrentAnimState;
    protected BOSS_ANIMATION_STATE CurrentAnimState
    {
        get { return _CurrentAnimState; }
        set { _CurrentAnimState = value; }
    }

    
    protected SpriteRenderer    sR;
    protected Rigidbody2D       rb2D;
    protected BoxCollider2D     col;
    protected Animator          anim;

    protected float             currentMoveSpeed;
    protected float             flashCount;

    protected bool              isAlive                     = true;
    protected bool              isAleadyFindingTarget       = false;
    protected bool              isCloseTarget               = false;
    protected bool              isStartingAttack            = false;
    protected bool              isLookatTarget              = false;
    protected bool              isReadyAttack               = true;
    protected bool              isRunningTakeDamageFlash    = false;
    

    //BossData
    public virtual void Init()
    {
        TargetOB = GameObject.FindWithTag("Player").transform;

        sR = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        CSVDataReader.instance.SetData(bData, bType.ToString());
    }

    protected virtual void LateUpdate()
    {
        SetAnimationState(); 
    }
    #region BT FUNC
    public virtual void IdleAction() { }
    /// <summary>
    /// NOTE : 플레이어 발견시 True 리턴
    /// </summary>
    /// <returns></returns>
    public virtual bool DetectTarget() { return true; }
    
    public virtual void ChaseAction() { }
    public virtual bool CheckCloseTarget() { return true; }
    public virtual bool LookAtTarget() { return true; }
    public virtual bool CheckPossibleAttack() { return true; }
    public virtual void StartAttack() { }
    
    public virtual bool CheckPossibleSkill() { return true; }
    public virtual void SkillAction() { }

    public virtual bool isDie() { return false; }
    public virtual void StopAttack() { }
    public virtual void DeadProcess() { }
    #endregion
    
    /// <summary>
    /// NOTE : 애니매이션 StateFloat 값 설정
    /// </summary>
    protected virtual void SetAnimationState()
    {
        if ((int)rb2D.velocity.y != 0)
            CurrentAnimState = BOSS_ANIMATION_STATE.Jump;
        else
            CurrentAnimState = (int)(currentMoveSpeed * 10) == 0 ? BOSS_ANIMATION_STATE.Idle : BOSS_ANIMATION_STATE.Walk;

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
    protected IEnumerator TakeDamageFlashProcess()
    {
        isRunningTakeDamageFlash = true;
        flashCount = 0;
        Color tmpcolor = sR.color;
        while (flashCount < 1f)
        {
            flashCount += 0.1f;
            tmpcolor.a = tmpcolor.a.Equals(1f) ? 0.2f : 1f;
            sR.color = tmpcolor;
            yield return new WaitForSeconds(0.1f);
        }
        sR.color = Color.white;
        isRunningTakeDamageFlash = false;
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
}
