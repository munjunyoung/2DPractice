using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteRangeMonster : WalkingMonster
{
    public MonsterRemoteAttackEffect[] attackEffectObject;
    private int effectpoolingCount = 0;
    
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
}
