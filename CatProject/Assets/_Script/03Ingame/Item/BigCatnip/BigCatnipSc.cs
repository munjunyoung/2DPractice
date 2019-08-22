using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCatnipSc : ItemSc
{
    private Rigidbody2D rb2d;
    protected int catnipamount;
    protected override void Start()
    {
        base.Start();
        rb2d = GetComponent<Rigidbody2D>();
        catnipamount = 20;    
    }
    public override void PlayWhenPick(Player playersc)
    {
        base.PlayWhenPick(playersc);
        playersc.CatnipItemNumber += catnipamount;
        StartCoroutine(SetActiveOff(0.5f));
    }

    IEnumerator SetActiveOff(float count)
    {
        rb2d.simulated = false;
        itemSpriteRenderer.enabled = false;
        itemCol.enabled = false;

        pickupEffect.SetActive(true);
        yield return new WaitForSeconds(count);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            PlayWhenPick(collision.transform.GetComponent<Player>());
    }
}
