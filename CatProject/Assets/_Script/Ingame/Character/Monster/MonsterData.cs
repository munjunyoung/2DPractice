using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
    [Header("HEALTH OPTION")]
    public int MaxHealth;

    /// </summary>
    [Header("MOVE OPTION")]
    [Range(1f, 10f)]
    public float patrollSpeed;
    [Range(1f, 100f)]
    public float traceSpeed;

    /// </summary>
    [Header("JUMP OPTION")]
    [Range(1, 30)]
    public float jumpPower;

    [Header("ATTACK OPTION")]
    [SerializeField]
    public GameObject attackEffectModel;
    [Range(1, 5)]
    public float attackRange;
    [Range(1f, 10f)]
    public int attackDamage;
    [Range(0f, 5f)]
    public float attackCoolTime;
    [Range(0f, 5f)]
    public float attackAnimSpeed;
}
