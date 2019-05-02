using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectSc : MonoBehaviour
{
    private Player playerSc;
    [HideInInspector]
    public int damage;
    private void Start()
    {
        playerSc = gameObject.transform.parent.GetComponent<Player>();
        damage = playerSc.pDATA.attackDamage;
    }
}
