using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteRangeMonster : WalkingMonster
{
    RaycastHit2D playerCheckRay;
    private Vector3 rayDirToPlayer = Vector2.zero;

    public MonsterAttackEffect[] attackEffectObject;
    private int effectpoolingCount = 0;
    

    /// <summary>
    /// NOTE : RayCast를 통하여 직선상에 캐릭터만 존재하는부분 
    /// </summary>
    protected override void Chase()
    {
        rayDirToPlayer = targetOb.transform.position - transform.position;
        rayDirToPlayer = rayDirToPlayer.normalized;

        playerCheckRay = Physics2D.Raycast(transform.position, rayDirToPlayer, mDATA.attackRange);
        Debug.DrawRay(transform.position, rayDirToPlayer * mDATA.attackRange, Color.red, 0.1f, false);
        if (playerCheckRay.collider != null)
        {
            if (playerCheckRay.collider.CompareTag("Player"))
            {
                Attack();
                return;
            }
        }

        base.Chase();
    }

    /// <summary>
    /// NOTE : ray로 체크하여 몬스터가 존재할경우 쿨타임 실행
    /// </summary>
    /// <returns></returns>
    public override IEnumerator AttackCoolTimeCoroutine()
    {
        if(playerCheckRay.collider!=null)
        {
            if (!playerCheckRay.collider.CompareTag("Player"))
                ChaseON(targetOb);
        }
        else
        {
            ChaseON(targetOb);
        }
        return base.AttackCoolTimeCoroutine();
    }

    /// <summary>
    /// NOTE : 배열에 저장한 이펙트들을 순서대로 사용
    /// </summary>
    public override void AttackEffect()
    {
        base.AttackEffect();
        var distance = Vector3.Distance(targetOb.position, transform.position);
        attackEffectObject[effectpoolingCount].gameObject.SetActive(true);
        Vector2 vel = GetVelocity(transform.position, targetOb.transform.position);
       
        attackEffectObject[effectpoolingCount].GetComponent<Rigidbody2D>().velocity = vel;
        effectpoolingCount = effectpoolingCount >= attackEffectObject.Length-1 ? 0 : effectpoolingCount + 1;
    }

    /// <summary>
    /// NOTE : 포물선 운동 시작점과 끝점을 이용하여 Velocity 값 구하기
    /// 높이가 평균보다 높을 경우
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    Vector3 GetVelocity(Vector3 currentPos, Vector3 targetPos)
    {
        float gravity = Physics2D.gravity.magnitude;
        float angle = 45 * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(targetPos.x, 0, 0);
        Vector3 planarPosition = new Vector3(currentPos.x, 0, 0);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        distance = distance > mDATA.attackRange ? mDATA.attackRange : distance;
        float yOffset = 0;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (targetPos.x > currentPos.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {   
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }


}
