using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlyingMonster : Monster
{
    //PathFinding
    private PathFinding testPathfinding;
    private List<PathNode> tracePath = new List<PathNode>();
    private LineRenderer testline;
    private Vector3[] points;
    private bool resetPathfinding = false;
    private int pointCount = 1;
    private bool isRunningPathfindingCoroutine = false;
    private Vector2Int prevTargetPos = Vector2Int.zero;
    private bool TargetOn = false;
    
    //Up Down
    private float deltaValue = 1f;
    private float ySpeed = 2f;
    private float yValue = 0f;
    protected override void Start()
    {
        base.Start();
        testPathfinding = new PathFinding(ownRoom.roomModel.GetComponent<Tilemap>());
        //testline = GameObject.Find("TestTraceLine").GetComponent<LineRenderer>();
    }

    /// <summary>
    /// NOTE : 상승 하강 Mathf.SIn 사용
    /// </summary>
    public override void PatrolAction()
    {
        base.PatrolAction();
        yValue = deltaValue * Mathf.Sin(Time.time * ySpeed);
        rb2D.velocity = new Vector2(base.characterDir.x * currentMoveSpeed,yValue );
    }

    /// <summary>
    /// NOTE : A* 알고리즘을 통한 path 포인트를 이용하여 이동
    /// </summary>
    public override void ChaseAction()
    {
        base.ChaseAction();
        
        //pathfinding이 리셋되었을 경우 다시 카운트0
        if (resetPathfinding)
        {
            resetPathfinding = false;
            pointCount = points.Length > 1 ? points.Length - 2 : 0;
        }
        if (Vector2.Distance(transform.position, points[pointCount]) < 0.05f)
        {
            if (pointCount <= 1)
            {
                pointCount = 1;
                rb2D.velocity = Vector2.zero;
                return;
            }
            else
                pointCount--;
        }
        
        characterDir = points[pointCount] - transform.position ;
        characterDir = characterDir.normalized;
        sR.flipX = characterDir.x >= 0 ? false : true;
        rb2D.velocity = characterDir * currentMoveSpeed;
        
    }

    /// <summary>
    /// NOTE : PATHFINDING 실행
    /// </summary>
    /// <param name="target"></param>
    protected override void AggroON(Transform target)
    {
        base.AggroON(target);
        rb2D.velocity = Vector2.zero;
        if (!isRunningPathfindingCoroutine)
            StartCoroutine(PathFindCoroutine());
    }

    public override void AttackAction()
    {
        base.AttackAction();
        rb2D.velocity = Vector2Int.zero;
    }
    /// <summary>
    /// NOTE : 중력 SCALE값 변경
    /// </summary>
    protected override void Die()
    {
        rb2D.gravityScale = 1;
        base.Die();
    }


    /// <summary>
    /// NOTE : Set PathFinding 
    /// </summary>
    private void PathFindingFunc(Vector2Int targetpos)
    {
        //Test
        tracePath.Clear();
        PathNode stnode = new PathNode(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        PathNode endnode = new PathNode(targetpos);
        testPathfinding.FindPath(stnode, endnode, ref tracePath);
            
        points = new Vector3[tracePath.Count];
       

        for (int i = 0; i < tracePath.Count; i++)
            points[i] = new Vector3(tracePath[i].pos.x + 0.5f, tracePath[i].pos.y + 0.5f, 0);

       
        resetPathfinding = true;
    }

    /// <summary>
    /// PathFinding 반복
    /// </summary>
    /// <returns></returns>
    IEnumerator PathFindCoroutine()
    {
        isRunningPathfindingCoroutine = true;
        while (isAlive)
        {
            Vector2Int  targetpos = new Vector2Int((int)targetOb.position.x, (int)targetOb.position.y);
            if (!targetpos.Equals(prevTargetPos))
            {
                PathFindingFunc(targetpos);
                prevTargetPos = targetpos;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    protected override void SetAnimationState()
    {
        CurrentAnimState = rb2D.velocity == Vector2.zero ? ANIMATION_STATE.Idle : ANIMATION_STATE.Walk;
        base.SetAnimationState();
    }


}
