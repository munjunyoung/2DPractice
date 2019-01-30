using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    private Player playerSc;
    private int Damage;
    private void Start()
    {
        playerSc = gameObject.transform.parent.GetComponent<Player>();
        Damage = playerSc.pDATA.attackDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            collision.GetComponent<Monster>().TakeDamage(Damage, transform);
    }
}
