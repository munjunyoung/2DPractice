using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectSc : MonoBehaviour
{
    private Player playerSc;
    [HideInInspector]
    public int damage = 0;

    private void Awake()
    {
        playerSc = gameObject.transform.parent.GetComponent<Player>();
        damage = playerSc.pDATA.attackDamage;

        gameObject.SetActive(false);
    }
}
