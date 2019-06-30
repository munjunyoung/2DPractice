using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatnipSc : ItemSc
{
    private void Start()
    {
        catnipamount = 5;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStop)
            return;
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().CatnipItemNumber += catnipamount;
            gameObject.SetActive(false);
        }
    }
}
