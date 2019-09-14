using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterPathFinding : MonoBehaviour
{
    
    
}
public class PathNode
{
    public Vector2Int pos;
    public int distG;
    public int distH; //거리
    public int distF;
    public int depth;
    public PathNode parentNode;
    
    public PathNode(Vector2Int _pos)
    {
        pos = _pos;
        distG = 0;
        distH = 0;
        distF = 0;
        depth = 0;
        parentNode = null;
    }

    public void CalcDist(PathNode dest, int cdepth)
    {
        int tmpHx = dest.pos.x - pos.x;
        int tmpHy = dest.pos.y - pos.y;
        distH = (tmpHx * tmpHx) + (tmpHy * tmpHy);

        int tmpGx = parentNode.pos.x - pos.x;
        int tmpGy = parentNode.pos.y - pos.y;
        int tmpG = (tmpGx * tmpGx) + (tmpGy * tmpGy);
        distG = tmpG + parentNode.distG;

        depth = cdepth;
        distF = distG + distH;
    }
}

class PathFinding
{
    private List<PathNode> openNodeList = new List<PathNode>();
    private List<PathNode> closeNodeList = new List<PathNode>();
    public Tilemap map = new Tilemap();

    public PathFinding(Tilemap _map)
    {
        map = _map;
    }
    /// <summary>
    /// NOTE : 길찾기
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <param name="path"></param>
    /// <param name="navi"></param>
    /// <returns></returns>
    public bool FindPath(PathNode startNode, PathNode endNode, ref List<PathNode> path)
    {
        openNodeList.Clear();
        closeNodeList.Clear();
        //시작 노드 생성 및 openNode 리스트 추가 
        PathNode tmpNode = startNode;
        openNodeList.Add(tmpNode);
        //깊이 설정
        int iDepth = 0;
        tmpNode.depth = iDepth;
        
        //이웃노드 리스트 생성
        List<PathNode> neighborNodes = new List<PathNode>();
        int count = 0;
        while (count<300)
        {
            count++;
            //열린노드에 데이터가 없을 경우 break
            if (openNodeList.Count == 0)
                break;
            
            tmpNode = openNodeList[0];//열린노드의 가장 처음 항목을 하나 가져오고 리스트에서 제거
            openNodeList.RemoveAt(0);
            
            //해당 노드가 목적지와 같으면 node의 parentnode를 추적하여 path를 역순으로 채우고 함수 리턴
            if (endNode.pos.Equals(tmpNode.pos))
            {
                while (tmpNode != null)
                {
                    path.Add(tmpNode);
                    tmpNode = tmpNode.parentNode;
                }
                return true;
            }
            //아닐경우 다시 진행
            //목표점이 아닌경우 닫힌노드리스트에 삽입
            closeNodeList.Add(tmpNode);

            ++iDepth; //탐색깊이 증가
            neighborNodes.Clear();
            
            GetNeighborNode(tmpNode, ref neighborNodes); //이웃노드를 가져옴

            for (int i = 0; i < neighborNodes.Count; i++)
            {
                //닫힌노드에 있는것이면 무시
                if (CheckFromCloseNode(neighborNodes[i]))
                    continue;

                neighborNodes[i].parentNode = tmpNode;
                neighborNodes[i].CalcDist(endNode, iDepth);
                InsertOpenNode(neighborNodes[i]);
            }

            SortOpenNode();
        }
        openNodeList.Clear();
        closeNodeList.Clear();
        return true;
    }

    /// <summary>
    /// NOTE : 중복 노드 삽입 되지 않도록 처리, 이미 중복된 노드들을 비용을 체크하여 변경)
    /// </summary>
    /// <param name="tmpnode"></param>
    private void InsertOpenNode(PathNode tmpnode)
    {
        for( int i =0;i<openNodeList.Count; i++)
        {
            if(openNodeList[i].pos.Equals(tmpnode.pos))
            {
                //리스트에 있는 노드가 고비용일 경우 tmpnode로 변경
                
                openNodeList[i] = CompareNodeG(openNodeList[i], tmpnode) ? openNodeList[i] : tmpnode;
                return;
            }
        }
        openNodeList.Add(tmpnode);
    }

    /// <summary>
    /// NOTE : OPEN NODE SORT ( 버블 소트 )
    /// </summary>
    private void SortOpenNode()
    {
        //노드가 2개이하면 RETURN
        if (openNodeList.Count < 2)
            return;
        bool bcontinue = true;

        while(bcontinue)
        {
            bcontinue = false;
            for (int i = 0; i < openNodeList.Count - 1; i++)
            {
                if(!CompareNodeF(openNodeList[i], openNodeList[i+1]))
                {
                    PathNode tmpnode = openNodeList[i];
                    openNodeList[i] = openNodeList[i + 1];
                    openNodeList[i + 1] = tmpnode;
                    bcontinue = true;
                }
            }
        }
    }

    /// <summary>
    /// NOTE : n1이 n2보다 저비용일 경우 true ( 거리가 까갑거나 탐색깊이가 적음)
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    private bool CompareNodeF(PathNode n1, PathNode n2)
    {
        if (n1.distF < n2.distF)
            return true;
        if (n1.distF> n2.distF)
            return false;
        if (n1.depth <= n2.depth)
            return true;

        return false;
    }

    /// <summary>
    /// NOTE : 2개의 노드의 현재시작점에서의 비용 체크
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    private bool CompareNodeG(PathNode n1, PathNode n2)
    {
        if (n1.distG < n2.distG)
            return true;
        if (n1.distG > n2.distG)
            return false;
        if (n1.depth <= n2.depth)
            return true;
        return false;
    }

    /// <summary>
    /// NOTE : 닫힌 노드 리스트를 순회하여 해당 노드와 같은 노드체크
    /// </summary>
    /// <param name="_tmpnode"></param>
    /// <returns></returns>
    private bool CheckFromCloseNode(PathNode _tmpnode)
    {
        foreach(var cn in closeNodeList)
        {
            if (cn.pos.Equals(_tmpnode.pos))
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// NOTE : 이웃 검색
    /// </summary>
    /// <param name="_tmpnode"></param>
    /// <param name="nodelist"></param>
    public void GetNeighborNode(PathNode _tmpnode, ref List<PathNode> nodelist)
    {
        ////-1,0 1,0 0,-1, 0,1 -> 0,0 2,0 0,2 2,2
        //Vector2Int[] dist = new Vector2Int[4] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };


        //foreach (var v2 in dist)
        //{
        //    Vector2Int tmppos = _tmpnode.pos + v2;

        //    var tmptile = map.GetTile(new Vector3Int(tmppos.x, tmppos.y, 0));
        //    if (tmptile != null)
        //    {
        //        if (tmptile.name.Equals("RuleTile_Terrain"))
        //            continue;
        //    }
        //    //이동가능한 지점이면 목록에 추가하기
        //    nodelist.Add(new Node((Vector2Int)tmppos));
        //}
        //우측 벽이 있을 경우 벽을 기준으로 상하 불가  2,1
        //위쪽 벽이 있을 경우 벽을 기준으로 좌우 불가  1,2
        //좌측 벽이 있을 경우 벽을 기준으로 상하 불가  0,1
        //하단 벽이 있을 경우 벽을 기준으로 좌우 불가  1,0
        

        //벽이 존재할경우 벽을기준으로 해당 포지션을 이웃노드에서 제외하기 위함
        List<Vector2Int> blockPosList = new List<Vector2Int>();
        //원점을 기준으로 0,1 좌우 , 2,3 상하 
        Vector2Int[] wallcheckPos =
              { new Vector2Int(_tmpnode.pos.x + 1 , _tmpnode.pos.y),
                new Vector2Int(_tmpnode.pos.x - 1, _tmpnode.pos.y),
                new Vector2Int(_tmpnode.pos.x , _tmpnode.pos.y +1),
                new Vector2Int(_tmpnode.pos.x , _tmpnode.pos.y -1)};

        for (int i = 0; i < wallcheckPos.Length; i++)
        {
            var tmptile = map.GetTile((Vector3Int)wallcheckPos[i]);
            if (tmptile == null)
                continue;
            else
            {
                if (tmptile.name.Equals("RuleTile_Terrain"))
                {
                    blockPosList.Add(wallcheckPos[i]);
                    if (i==0||i==1)
                    {
                        blockPosList.Add(wallcheckPos[i] + new Vector2Int(0, 1));
                        blockPosList.Add(wallcheckPos[i] + new Vector2Int(0, -1));
                    }
                    else
                    {
                        blockPosList.Add(wallcheckPos[i] + new Vector2Int(1, 0));
                        blockPosList.Add(wallcheckPos[i] + new Vector2Int(-1, 0));
                    }
                }
            }
        }

        //이웃노드 순회
        int[] distx = new int[3] { -1, 0, 1 };
        int[] disty = new int[3] { -1, 0, 1 };

        for (int y = 0; y < 3; ++y)
        {
            for (int x = 0; x < 3; ++x)
            {
                Vector2Int tmppos = new Vector2Int(distx[x] + _tmpnode.pos.x, disty[y] + _tmpnode.pos.y);
                
                //중앙 위치는 필요X
                if (CheckFromCloseNode(new PathNode(tmppos)))
                    continue;

                if (blockPosList.Contains(tmppos))
                    continue;

                //이동불가 지역 필요 X
                var tmptile = map.GetTile((Vector3Int)tmppos);
                if (tmptile != null)
                {
                    if (tmptile.name.Equals("RuleTile_Terrain"))
                        continue;
                }
                //이동가능한 지점이면 목록에 추가하기
                nodelist.Add(new PathNode(tmppos));
            }
        }
    }
}




