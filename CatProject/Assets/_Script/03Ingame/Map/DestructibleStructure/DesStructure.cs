using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesStructure : MonoBehaviour
{
    public DungeonRoom ownRoom = null;

    private SpriteRenderer spRenderer;
    public Sprite[] spriteArray;
    private int hp = 0;
    public bool isAlive = false;
    private Vector2 startpos;
    private void Awake()
    {
        hp = spriteArray.Length;
        isAlive = true;
        spRenderer = GetComponent<SpriteRenderer>();
        startpos = transform.localPosition;
    }

    /// <summary>
    /// NOTE : 공격받음
    /// </summary>
    private void TakeDamage()
    {
        if (!isAlive)
            return;
        hp -= 1;
        if (hp > 0)
            spRenderer.sprite = spriteArray[hp - 1];
        else
            StartCoroutine(DestroyObjectByCount(2f));

    }
    
    /// <summary>
    /// NOTE : 2초후 실행
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnumerator DestroyObjectByCount(float count)
    {
        isAlive = false;
        spRenderer.sprite = null;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(count);
        transform.localPosition = startpos;
        transform.localRotation = Quaternion.identity;
        hp = spriteArray.Length;
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<BoxCollider2D>().isTrigger = false;
        
        spRenderer.sprite = spriteArray[hp-1];
        isAlive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackEffect"))
            TakeDamage();
    }
}
