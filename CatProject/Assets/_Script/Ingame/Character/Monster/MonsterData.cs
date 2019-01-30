using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
    [Header("HEALTH OPTION")]
    public int maxHP;
    
    [Header("MOVE OPTION")]
    [Range(1f, 10f)]
    public float patrollSpeed;
    [Range(1f, 100f)]
    public float traceSpeed;
    
    [Header("JUMP OPTION")]
    [Range(1f, 30f)]
    public float jumpPower;

    [Header("ATTACK OPTION")]
    [Range(1, 50)]
    public int bodyAttacktDamage;
    [SerializeField]
    public GameObject attackEffectModel;
    [Range(1f, 5f)]
    public float attackRange;
    [Range(1, 50)]
    public int attackDamage;
    //캐릭터와 몸이 겹쳤을때 데미지
    [Range(0f, 5f)]
    public float attackCoolTime;
    [Range(0f, 5f)]
    public float attackAnimSpeed;

    [Header("KNOCKBACK OPTION")]
    [Range(1, 20)]
    public float knockBackPower;
    [Range(0f, 10f)]
    public float knockbackTime;
}
