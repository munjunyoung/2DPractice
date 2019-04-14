using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DestructibleStructureSc : MonoBehaviour
{
    private SpriteRenderer spRenderer;
    public Sprite[] spriteArray;
    private int hp = 0;

    private void Awake()
    {
        hp = spriteArray.Length;
        spRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// NOTE : 공격받음
    /// </summary>
    private void TakeDamage()
    {
        hp -= 1;
        if(hp>0)
            spRenderer.sprite = spriteArray[hp-1];
        else
            gameObject.SetActive(false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackEffect"))
            TakeDamage();
    }
}
