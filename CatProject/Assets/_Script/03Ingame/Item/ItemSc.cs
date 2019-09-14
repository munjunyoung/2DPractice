using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSc : MonoBehaviour
{
    protected bool isStop = false;
    protected SpriteRenderer itemSpriteRenderer; 
    protected BoxCollider2D itemCol;
    public GameObject pickupEffect;

    protected virtual void Start()
    {
        itemSpriteRenderer = GetComponent<SpriteRenderer>();
        itemCol = GetComponent<BoxCollider2D>();
        pickupEffect.SetActive(false);
    }

    public virtual void PlayWhenPick(Player playersc)
    {
        if (isStop)
            return;
    }

    public void StopAction(float _stopcount)
    {
        StartCoroutine(StopCoroutine(_stopcount));
    }

    protected IEnumerator StopCoroutine(float _stopcount)
    {
        isStop = true;
        yield return new WaitForSeconds(_stopcount);
        isStop = false;
    }
}
