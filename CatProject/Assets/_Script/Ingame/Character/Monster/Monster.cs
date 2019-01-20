using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ORDER_STATE {Idle, Patroll, Trace }
public class Monster : MonoBehaviour
{
    /// <summary>
    /// CHARACTER STATE 
    /// NOTE : 애니매이션 파라미터 상태 설정 (속성은 animation 속도 설정)
    /// WALK : 이동 중일 경우 이동 하는 속도에 맞춰서 발걸음을 느려지게 하기 위함
    /// ATTACK : 공격 속도 설정
    /// TODO : 위 2개 말고는 아직은 필요성을 느끼지 못함
    private ANIMATION_STATE InstanceState;
    private ANIMATION_STATE CurrentAnimState
    {
        get { return InstanceState; }
        set
        {
            InstanceState = value;
            switch (InstanceState)
            {
                case ANIMATION_STATE.Walk:
                    anim.speed = Mathf.Abs(currentMoveSpeed * 0.1f);
                    break;
                case ANIMATION_STATE.Attack:
                    anim.speed = mDATA.attackAnimSpeed;
                    break;
                default:
                    anim.speed = 1;
                    break;
            }
        }
    }

    /// <summary>
    /// NOTE : PATROLL SPEED, TRACESPEED 속성 설정
    /// </summary>
    
    private ORDER_STATE InstanceOrderState;
    public ORDER_STATE orderState
    {
        get { return InstanceOrderState; }
        set
        {
            InstanceOrderState = value;
            switch (InstanceOrderState)
            {
                case ORDER_STATE.Patroll:
                    currentMoveSpeed = mDATA.patrollSpeed;
                    break;
                case ORDER_STATE.Trace:
                    currentMoveSpeed = mDATA.traceSpeed;
                    break;
            }
        }

    }

    [Header("MONSTER DATA SET"), SerializeField]
    private MonsterData mDATA;

    private Rigidbody2D rb2D;
    private Animator anim;

    [HideInInspector]
    public bool isAlive = false;
    private bool isGrounded;

    private float currentMoveSpeed;
    private LayerMask tileLayer;

    [HideInInspector]
    public Transform targetOb = null;

    private void Awake()
    { 
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        tileLayer = LayerMask.GetMask("Tile");
        orderState = Random.Range(0,100)>99? ORDER_STATE.Idle : ORDER_STATE.Patroll;   
    }

    private void FixedUpdate()
    {
        switch (orderState)
        {
            case ORDER_STATE.Idle:
                currentMoveSpeed = 0;
                break;
            case ORDER_STATE.Patroll:
                Patroll();
                break;
            case ORDER_STATE.Trace:
                Trace();
                break;
        }
    }

    private void LateUpdate()
    {
        SetAnimationState();
    }
    
    /// <summary>
    /// NOTE : 구조물에서 길이 없거나 벽에 부딪혔을경우 방향 순회 (속도는 maxspeed의 절반)
    /// TODO : 벽에 부딪혔을 경우에는 점프를 하도록 구현 여지
    /// </summary>
    private void Patroll()
    {
        if (!isGrounded)
            return;

        RaycastHit2D wallCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -0.5f, 0), transform.right, 1.5f, tileLayer);
        if (wallCheckInfo.collider != null)
        {
            if (wallCheckInfo.collider.CompareTag("Ground") || wallCheckInfo.collider.CompareTag("Floor"))
                transform.localEulerAngles = transform.localEulerAngles.y.Equals(0f) ? new Vector3(0, 180, 0) : Vector3.zero;
        }

        RaycastHit2D nullCheckInfo = Physics2D.Raycast(transform.position, transform.right + new Vector3(0,-1f,0), 1.5f, tileLayer);
        if(nullCheckInfo.collider==null)
            transform.localEulerAngles = transform.localEulerAngles.y.Equals(0f) ? new Vector3(0, 180, 0) : Vector3.zero;

        rb2D.velocity = new Vector2(transform.right.x * currentMoveSpeed, rb2D.velocity.y);
    }

    /// <summary>
    /// NOTE : 추적 벽에 부딪힐 경우 점프
    /// </summary>
    private void Trace()
    {
        RaycastHit2D wallCheckInfo = Physics2D.Raycast(transform.position + new Vector3(0, -0.5f, 0), transform.right, 1.5f, tileLayer);
        if (wallCheckInfo.collider != null)
        {
            if (wallCheckInfo.collider.CompareTag("Ground") || wallCheckInfo.collider.CompareTag("Floor"))
                Jump();
        }

        transform.localEulerAngles = transform.position.x > targetOb.position.x ? new Vector3(0, 180, 0) : Vector3.zero;
        rb2D.velocity = new Vector2(transform.right.x * currentMoveSpeed, rb2D.velocity.y);

        Debug.Log(wallCheckInfo.collider);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            if (((int)rb2D.velocity.y).Equals(0))
                rb2D.AddForce(Vector2.up * mDATA.jumpPower, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// 캐릭터 상태 설정
    /// NOTE : STATE 우선 순위
    /// 1. ATTACK 2. JUMP 3. FALL 4. MOVE
    /// </summary>
    private void SetAnimationState()
    {
        //캐릭터 상태 설정(애니매이션 상태 설정)
        if ((int)rb2D.velocity.y > 0)
            CurrentAnimState = ANIMATION_STATE.Jump;
        else if ((int)rb2D.velocity.y < 0)
            CurrentAnimState = ANIMATION_STATE.Fall;
        else
            CurrentAnimState = (int)(currentMoveSpeed * 10) == 0 ? ANIMATION_STATE.Idle : ANIMATION_STATE.Walk;


        anim.SetFloat("StateFloat", (int)CurrentAnimState);
    }

    /// <summary>
    /// NOTE : FloorCheck
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 contactnormalSum = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            contactnormalSum += collision.contacts[i].normal;

        if (contactnormalSum.y > 0)
            isGrounded = true;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
