using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player playerSc;
    protected bool SkillOn = false;
    protected float SkillDuration = 10f;
    protected virtual void Awake()
    {
        playerSc = GetComponent<Player>();
    }
    public virtual void Execute() { }
}

public class SkillAttackUpgrade : Skill
{
    int originalDamage = 30;
    int plusDamage = 30;
    Color originalColor = Color.white;
    Color changeColor = Color.red;
    Vector2 originalScale = Vector2.one;
    Vector2 changeScale = Vector2.one*2f;
    GameObject effectModel = null;
    SpriteRenderer spriteOfEffectModel;
    AttackEffectSc attackScOfEffectModel;

    /// <summary>
    /// NOTE : 변수 초기화 (매번 스킬을 사용할때마다 호출해야 하므로 생성할떄 한번에 초기화하고 계속 사용하도록 함 GETCOMPONENT를 줄이기위함)
    /// </summary>
    /// <param name="p"></param>
    protected override void Awake()
    {
        base.Awake();
        effectModel = playerSc.attackEffectModel;
        spriteOfEffectModel = effectModel.GetComponent<SpriteRenderer>();
        attackScOfEffectModel = effectModel.GetComponent<AttackEffectSc>();

        originalDamage = attackScOfEffectModel.damage;
        originalColor = spriteOfEffectModel.color;
        originalScale = effectModel.transform.localScale;
    }
    
    public override void Execute()
    {
        if (!SkillOn)
            StartCoroutine(SkillExecute());
    }

    IEnumerator SkillExecute()
    {
        SkillOn = true;
        effectModel.transform.localScale = changeScale;
        spriteOfEffectModel.color = changeColor;
        attackScOfEffectModel.damage += plusDamage;
        yield return new WaitForSeconds(SkillDuration);
        effectModel.transform.localScale = originalScale;
        spriteOfEffectModel.color = originalColor;
        attackScOfEffectModel.damage = originalDamage;
        SkillOn = false;
    }
}
