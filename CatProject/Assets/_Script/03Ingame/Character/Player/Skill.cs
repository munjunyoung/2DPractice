using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Skill_Type { Buff, Immediate}

public class Skill : MonoBehaviour
{
    protected Player playerSc;
    protected SpriteRenderer playerSpriteRenderer;
    protected Color playerColor;
    protected GameObject SkillEffectModel;
    public float durationTime = 0;
    public float coolTime = 0;
    public int consumeCatnipValue = 0;

    protected bool skillPossibleCheck = false;
    /// <summary>
    /// NOTE : 변수 초기화 (매번 스킬을 사용할때마다 호출해야 하므로 생성할떄 한번에 초기화하고 계속 사용하도록 함 GETCOMPONENT를 줄이기위함)
    /// TODO : 추후에 스킬값또한 액셀로 처리하게 된다면 START부분을 간소화 할 가능성이 높다
    /// </summary>
    /// <param name="p"></param>
    protected virtual void Start()
    {
        playerSc = GetComponent<Player>();
        playerSpriteRenderer = playerSc.GetComponent<SpriteRenderer>();
        playerColor = playerSpriteRenderer.color;
        
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
            playerSc.CatnipItemNumber -= consumeCatnipValue;
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
        if (playerSc.CatnipItemNumber < consumeCatnipValue)
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
        CharacterEffectOn();
        yield return new WaitForSeconds(_durationtime);
        CharacterEffectOff();
    }

    /// <summary>
    /// NOTE : 스킬 사용시 캐릭터 이펙트 관련 (스킬마다 다른 캐릭터 이펙트 부여)
    /// </summary>
    protected virtual void CharacterEffectOn()
    {
        SkillEffectModel.SetActive(true);
    }
    
    protected virtual void CharacterEffectOff()
    {
        SkillEffectModel.SetActive(false);
        playerSpriteRenderer.color = playerColor;
    }
}


public class SkillAttackUP : Skill
{
    int originalDamage = 30;
    int plusDamage = 30;
    Color originalColor = Color.white;
    Color changeColor = Color.red;

    Vector2 originalScale = Vector2.one;
    Vector2 changeScale = Vector2.one*1.5f;

    GameObject attackEffectModel = null;
    SpriteRenderer spriteOfEffectModel;
    AttackEffectSc attackScOfEffectSc;

    protected override void Start()
    {
        base.Start();
        SkillEffectModel = Instantiate(LoadDataManager.instance.SkillEffectPrefabDic["AttackUpEffect"], playerSc.transform);
        SkillEffectModel.transform.localPosition = new Vector2(0, -1);
        SkillEffectModel.SetActive(false);
        attackEffectModel = playerSc.attackEffectModel;
        spriteOfEffectModel = attackEffectModel.GetComponent<SpriteRenderer>();
        attackScOfEffectSc = attackEffectModel.GetComponent<AttackEffectSc>();
        

        originalDamage = attackScOfEffectSc.damage;
        originalColor = spriteOfEffectModel.color;
        originalScale = attackEffectModel.transform.localScale;

        durationTime = 5f;
        coolTime = 5f;
        consumeCatnipValue = 5;
    }
    
    protected override IEnumerator ExecuteSkillCoroutine(float _durationTime)
    {
        attackEffectModel.transform.localScale = changeScale;
        spriteOfEffectModel.color = changeColor;
        attackScOfEffectSc.damage += plusDamage;
        
        yield return StartCoroutine(base.ExecuteSkillCoroutine(_durationTime));
        attackEffectModel.transform.localScale = originalScale;
        spriteOfEffectModel.color = originalColor;
        attackScOfEffectSc.damage = originalDamage;
    }
    protected override void CharacterEffectOn()
    {
        base.CharacterEffectOn();
        playerSpriteRenderer.color = Color.red;
    }
}

public class SkillSpeedUP : Skill
{
    private int speedUpAmount = 5;
    private float originalSpeed;

    protected override void Start()
    {
        base.Start();
        SkillEffectModel = Instantiate(LoadDataManager.instance.SkillEffectPrefabDic["SpeedUpEffect"], playerSc.transform);
        SkillEffectModel.transform.localPosition = new Vector2(0, 0);
        SkillEffectModel.SetActive(false);
        originalSpeed = playerSc.pDATA.maxSpeedValue;
        
        durationTime = 5f;
        coolTime = 10f;
        consumeCatnipValue = 5;
    }
    
    protected override IEnumerator ExecuteSkillCoroutine(float _durationtime)
    {
        playerSc.pDATA.maxSpeedValue += speedUpAmount;
        yield return StartCoroutine(base.ExecuteSkillCoroutine(_durationtime));
        playerSc.pDATA.maxSpeedValue = originalSpeed;
    }
    protected override void CharacterEffectOn()
    {
        base.CharacterEffectOn();
        playerSpriteRenderer.color = Color.blue;
    }
}

public class SkillHealing : Skill
{
    private int hpUpAmount = 50;
    private float originalHp;

    protected override void Start()
    {
        base.Start();
        SkillEffectModel = Instantiate(LoadDataManager.instance.SkillEffectPrefabDic["HealingEffect"], playerSc.transform);
        SkillEffectModel.transform.localPosition = new Vector2(0, 0);
        SkillEffectModel.SetActive(false);
        originalHp = playerSc.pDATA.maxHP;

        durationTime = 1f;
        coolTime = 10f;
        consumeCatnipValue = 5;
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
    protected override void CharacterEffectOn()
    {
        base.CharacterEffectOn();
        playerSpriteRenderer.color = Color.green;
    }

    
}
