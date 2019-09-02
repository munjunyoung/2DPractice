using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotionSc : ItemSc
{
    private Rigidbody2D rb2d;
    private int hpAmount;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hpAmount = 20;
        rb2d = GetComponent<Rigidbody2D>();
    }

    public override void PlayWhenPick(Player playersc)
    {
        base.PlayWhenPick(playersc);
        playersc.CurrentHP += hpAmount;
        StartCoroutine(SetActiveOff(1f));
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
