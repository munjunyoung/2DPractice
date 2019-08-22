using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatnipSc : ItemSc
{
    protected int catnipamount;
    protected override void Start()
    {
        base.Start();
        catnipamount = 5;
    }

    public override void PlayWhenPick(Player playersc)
    {
        base.PlayWhenPick(playersc);
        playersc.CatnipItemNumber += catnipamount;
        StartCoroutine(SetActiveOff(0.5f));
    }

    IEnumerator SetActiveOff(float count)
    {
        itemSpriteRenderer.enabled = false;
        itemCol.enabled = false;

        pickupEffect.SetActive(true);
        yield return new WaitForSeconds(count);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            PlayWhenPick(collision.transform.GetComponent<Player>());
    }
}
