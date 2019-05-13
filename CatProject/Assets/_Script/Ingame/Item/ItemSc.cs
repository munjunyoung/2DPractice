using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSc : MonoBehaviour
{
    bool isStop = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStop)
            return;
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().CatnipItemNumber += 5;
            gameObject.SetActive(false);
        }
    }

    public void StopAction(float _stopcount)
    {
        StartCoroutine(StopCoroutine(_stopcount));
    }

    private IEnumerator StopCoroutine(float _stopcount)
    {
        isStop = true;
        yield return new WaitForSeconds(_stopcount);
        isStop = false;
    }

}
