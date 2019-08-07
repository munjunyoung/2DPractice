using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMonster : Monster
{
    protected bool isGrounded = false;
    
    /// <summary>
    /// NOTE : 구조물에서 길이 없거나 벽에 부딪혔을경우 방향 순회 (속도는 maxspeed의 절반)
    /// TODO : 벽에 부딪혔을 경우에는 점프를 하도록 구현 여지
    /// </summary>
    protected override void Patrol()
    {
        base.Patrol();
        //flip을 통한 dir 설정
        
        //길 끊김 Null Raycast
        RaycastHit2D nullCheckInfo = Physics2D.Raycast(transform.position, base.characterDir + new Vector2(0, -transform.localScale.y), transform.localScale.x + 0.5f);
        if (nullCheckInfo.collider == null)
            sR.flipX = sR.flipX.Equals(true) ? false : true;

        rb2D.velocity = new Vector2(base.characterDir.x * currentMoveSpeed, rb2D.velocity.y);
    }


    /// <summary>
    /// NOTE : 추적 벽에 부딪힐 경우 점프
    /// </summary>
    protected override void Chase()
    {
        base.Chase();
        //flip을 통한 dir 설정
        Vector2 dir = sR.flipX.Equals(true) ? -Vector2.right : Vector2.right;
        //벽 Raycast (아래를 훑어야하긴하는데)
        RaycastHit2D frontCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -((transform.localScale.y - 1) + 0.2f), 0), dir, transform.localScale.x);

        if (frontCheckInfo.collider != null)
        {
            if (frontCheckInfo.collider.CompareTag("Ground") || frontCheckInfo.collider.CompareTag("Floor") || frontCheckInfo.collider.CompareTag("Garbage") || frontCheckInfo.collider.CompareTag("Box"))
                Jump();
        }
        sR.flipX = (int)(transform.position.x) > (int)(targetOb.position.x) ? true : false;

        rb2D.velocity = new Vector2(dir.x * currentMoveSpeed, rb2D.velocity.y);

        ChaseOFF();
    }

    /// <summary>
    /// NOTE : 땅에 정착해 있는지 체크후 점프 (Addforce 사용)
    /// </summary>
    protected override void Jump()
    {
        base.Jump();
        if (isGrounded)
        {
            if (((int)rb2D.velocity.y).Equals(0))
                rb2D.AddForce(Vector2.up * mDATA.jumpPower, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// NOTE : OrderState가 공격상태이면 실행
    /// </summary>
    protected override void Attack()
    {
        base.Attack();
        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }

    protected override void SetAnimationState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentAnimState = ANIMATION_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentAnimState = ANIMATION_STATE.Fall;
        else
            CurrentAnimState = (int)(currentMoveSpeed * 10) == 0 ? ANIMATION_STATE.Idle : ANIMATION_STATE.Walk;
        base.SetAnimationState();
    }

    #region COLLISION
    /// <summary>
    /// NOTE : FloorCheck
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        Vector2 contactnormalSum = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            contactnormalSum += collision.contacts[i].normal;

        if (contactnormalSum.y > 0)
            isGrounded = true;
    }

    /// <summary>
    /// NOTE : GROUND CHECK FALSE
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
    #endregion
}
