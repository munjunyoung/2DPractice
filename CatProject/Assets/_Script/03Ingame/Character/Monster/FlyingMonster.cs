using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlyingMonster : Monster
{
    //PathFinding
    private PathFinding testPathfinding;
    private List<Node> tracePath = new List<Node>();
    private LineRenderer testline;
    private Vector3[] points;
    private bool resetPathfinding = false;
    private int pointCount = 1;
    private bool isRunningPathfindingCoroutine = false;
    private Vector2Int prevTargetPos = Vector2Int.zero;
    
    //Up Down
    private float deltaValue = 1f;
    private float ySpeed = 2f;
    private float yValue = 0f;
    protected override void Start()
    {
        base.Start();
        testPathfinding = new PathFinding(ownRoom.roomModel.GetComponent<Tilemap>());
        testline = GameObject.Find("TestTraceLine").GetComponent<LineRenderer>();
    }

    /// <summary>
    /// NOTE : 상승 하강 Mathf.SIn 사용
    /// </summary>
    protected override void Patrol()
    {
        base.Patrol();
        yValue = deltaValue * Mathf.Sin(Time.time * ySpeed);
        rb2D.velocity = new Vector2(base.characterDir.x * currentMoveSpeed,yValue );
    }

    protected override void Chase()
    {
        base.Chase();
        
        //pathfinding이 리셋되었을 경우 다시 카운트0
        if(resetPathfinding)
        {
            resetPathfinding = false;
            pointCount = points.Length - 2;
        }
        if (Vector2.Distance(transform.position, points[pointCount]) < 0.05f)
            pointCount = pointCount <= 0 ? 0 : pointCount - 1;

        characterDir = points[pointCount] - transform.position ;
        characterDir = characterDir.normalized;
        sR.flipX = characterDir.x >= 0 ? false : true;
        rb2D.velocity = characterDir * currentMoveSpeed;
        
        Debug.Log("c1 : " + transform.position + " w1 : " + points[pointCount] + " dir : " + characterDir);
    }

    /// <summary>
    /// NOTE : PATHFINDING 실행
    /// </summary>
    /// <param name="target"></param>
    protected override void ChaseON(Transform target)
    {
        base.ChaseON(target);
        rb2D.velocity = Vector2.zero;
        if (!isRunningPathfindingCoroutine)
            StartCoroutine(PathFindCoroutine());
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
        Node stnode = new Node(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        Node endnode = new Node(targetpos);
        testPathfinding.FindPath(stnode, endnode, ref tracePath);
            
        points = new Vector3[tracePath.Count];
        testline.positionCount = tracePath.Count;

        for (int i = 0; i < tracePath.Count; i++)
            points[i] = new Vector3(tracePath[i].pos.x + 0.5f, tracePath[i].pos.y + 0.5f, 0);

        testline.SetPositions(points);
        resetPathfinding = true;
    }

    /// <summary>
    /// PathFinding 반복
    /// </summary>
    /// <returns></returns>
    IEnumerator PathFindCoroutine()
    {
        isRunningPathfindingCoroutine = true;
        while (OrderState.Equals(ORDER_STATE.Chase)||OrderState.Equals(ORDER_STATE.Attack))
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

}
