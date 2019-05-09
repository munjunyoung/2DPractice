using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Skill_Type { Buff, Immediate}

public class Skill : MonoBehaviour
{
    protected Player playerSc;

    public float durationTime = 0;
    public float coolTime = 0;
    public int consumeValue = 0;

    protected bool skillPossibleCheck = false;
    /// <summary>
    /// NOTE : 변수 초기화 (매번 스킬을 사용할때마다 호출해야 하므로 생성할떄 한번에 초기화하고 계속 사용하도록 함 GETCOMPONENT를 줄이기위함)
    /// TODO : 추후에 스킬값또한 액셀로 처리하게 된다면 START부분을 간소화 할 가능성이 높다
    /// </summary>
    /// <param name="p"></param>
    protected virtual void Start()
    {
        playerSc = GetComponent<Player>();
    }
    /// <summary>
    /// NOTE : 캐릭터 실행
    /// </summary>
    public virtual bool Execute()
    {
        var skillpossiblecheck = CheckSkillPossible();
        if (skillpossiblecheck)
        {
            StartCoroutine(ExecuteSkillCoroutine(durationTime));
            playerSc.item -= consumeValue;
        }

        return skillpossiblecheck;
    }

    /// <summary>
    /// NOTE : 스킬이 실행 제외처리
    /// </summary>
    protected virtual bool CheckSkillPossible()
    {
        if (playerSc.isRunningSkillCooltime)
            return false;
        //플레이어 아이템값이 적을 경우
        if (playerSc.item < consumeValue)
        {
            Debug.Log("아이템이 부족합니다.");
            return false;
        }
        return true;
    }
    /// <summary>
    /// NOTE : 지속버프 즉시시전 모두 코루틴을 이용하여 일정 시간을 두게함으로써 override
    ///      : 즉시 시전하는 경우에도 현재 바로 실행되었는지 체크하기위해 return 함으로 사용
    /// </summary>
    /// <param name="_durationtime"></param>
    /// <returns></returns>
    protected virtual IEnumerator ExecuteSkillCoroutine(float _durationtime)
    {
        yield return new WaitForSeconds(_durationtime);
    }
}

public class SkillAttackUP : Skill
{
    int originalDamage = 30;
    int plusDamage = 30;
    Color originalColor = Color.white;
    Color changeColor = Color.red;
    Vector2 originalScale = Vector2.one;
    Vector2 changeScale = Vector2.one*2f;
    GameObject effectModel = null;
    SpriteRenderer spriteOfEffectModel;
    AttackEffectSc attackScOfEffectSc;

    protected override void Start()
    {
        base.Start();
        effectModel = playerSc.attackEffectModel;
        spriteOfEffectModel = effectModel.GetComponent<SpriteRenderer>();
        attackScOfEffectSc = effectModel.GetComponent<AttackEffectSc>();

        originalDamage = attackScOfEffectSc.damage;
        originalColor = spriteOfEffectModel.color;
        originalScale = effectModel.transform.localScale;

        durationTime = 5f;
        coolTime = 5f;
        consumeValue = 5;
    }
    
    protected override IEnumerator ExecuteSkillCoroutine(float _durationTime)
    {
        effectModel.transform.localScale = changeScale;
        spriteOfEffectModel.color = changeColor;
        attackScOfEffectSc.damage += plusDamage;
        yield return StartCoroutine(base.ExecuteSkillCoroutine(_durationTime));
        effectModel.transform.localScale = originalScale;
        spriteOfEffectModel.color = originalColor;
        attackScOfEffectSc.damage = originalDamage;
    }
}

public class SkillSpeedUP : Skill
{
    private int speedUpAmount = 5;
    private float originalSpeed;

    protected override void Start()
    {
        base.Start();
        originalSpeed = playerSc.pDATA.maxSpeedValue;
        
        durationTime = 5f;
        coolTime = 10f;
        consumeValue = 5;
    }
    
    protected override IEnumerator ExecuteSkillCoroutine(float _durationtime)
    {
        playerSc.pDATA.maxSpeedValue += speedUpAmount;
        yield return StartCoroutine(ExecuteSkillCoroutine(_durationtime));
        playerSc.pDATA.maxSpeedValue = originalSpeed;
    }
}

public class SkillRecoveryHP : Skill
{
    private int hpUpAmount = 50;
    private float originalHp;

    protected override void Start()
    {
        base.Start();
        originalHp = playerSc.pDATA.maxHP;

        durationTime = 0f;
        coolTime = 10f;
        consumeValue = 5;
    }
    
    protected override bool CheckSkillPossible()
    {
        if (!base.CheckSkillPossible())
            return false;
        if (playerSc.CurrentHP.Equals(playerSc.pDATA.maxHP))
        {
            Debug.Log("이미 체력이 가득 차 있습니다.");
            return false;
        }
        return true;
    }

    protected override IEnumerator ExecuteSkillCoroutine(float _durationtime)
    {
        playerSc.CurrentHP += hpUpAmount;
        yield return StartCoroutine(base.ExecuteSkillCoroutine(_durationtime));
    }
}