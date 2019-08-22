using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackEffect : MonoBehaviour
{
    private Monster parentOb;
    private int damage;
    private float count;

    private CircleCollider2D col;
    private SpriteRenderer spr;

    public GameObject BombEffect;
    

    private void Start()
    {
        parentOb = GetComponentInParent<Monster>();
        col = GetComponent<CircleCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        damage = parentOb.mDATA.attackDamage;
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        spr.enabled = true;
        col.isTrigger = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
            collision.transform.GetComponent<Player>().TakeDamage(damage, transform);

        InvisibleObject();
    }

    private void InvisibleObject()
    {
        spr.enabled = false;
        col.isTrigger = true;
        BombEffect.SetActive(true);
        StartCoroutine(SetActiveOFF(1f));
    }
    

    IEnumerator SetActiveOFF(float count)
    {
        yield return new WaitForSeconds(count);
        gameObject.SetActive(false);
    }
}
