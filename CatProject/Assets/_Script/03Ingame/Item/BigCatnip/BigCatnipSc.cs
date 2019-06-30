using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCatnipSc : ItemSc
{
    private void Start()
    {
        catnipamount = 20;    
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isStop)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.GetComponent<Player>().CatnipItemNumber += catnipamount;
            gameObject.SetActive(false);
        }
    }
}
