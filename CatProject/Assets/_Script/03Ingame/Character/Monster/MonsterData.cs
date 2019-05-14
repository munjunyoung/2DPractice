using System.Collections;
using System.Collections.Generic;

public class MonsterData
{
    //HEALTH
    public int maxHP;
    //MOVE
    public float patrollSpeed;
    public float traceSpeed;
    //JUMP
    public float jumpPower;
    //ATTACK
    public int bodyAttacktDamage;
    public float attackRange;
    public int attackDamage;
    //캐릭터와 몸이 겹쳤을때 데미지
    public float attackCoolTime;
    public float attackAnimSpeed = 1;
    //KNOCKBACK
    public float knockBackPower = 5;
    public float knockbackTime = 0.5f;
}
