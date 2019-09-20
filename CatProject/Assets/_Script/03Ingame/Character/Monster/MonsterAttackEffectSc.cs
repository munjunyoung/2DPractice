using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackEffectSc : MonoBehaviour
{
    [HideInInspector]
    public Monster monsterSc;
    [HideInInspector]
    public int damage = 0;

    private SpriteRenderer sR;
    private float posX = 0;
    private float posY = 0;
    private void Start()
    {
        monsterSc = transform.parent.GetComponent<Monster>();
        monsterSc.attackEffectOb = this;
        sR = GetComponent<SpriteRenderer>();
        damage = monsterSc.mDATA.attackDamage;
        posX = transform.localPosition.x;
        posY = transform.localPosition.y;
    }
    
    public void SetActiveOn()
    {
        transform.localPosition = monsterSc.sR.flipX ? new Vector2(-posX, posY) : new Vector2(posX, posY);
        sR.flipX = monsterSc.sR.flipX;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        StartCoroutine(SetActiveOffByCount(0.5f));
    }

    /// <summary>
    /// NOTE : ActiveOff;
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnumerator SetActiveOffByCount(float count)
    {
        yield return new WaitForSeconds(count);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<Player>().TakeDamage(damage, transform.position);
    }
}
