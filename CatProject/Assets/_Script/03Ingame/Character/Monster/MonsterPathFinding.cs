using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterPathFinding : MonoBehaviour
{
    
    
}
public class Node
{
    public Vector2Int pos;
    public int distH; //거리
    public int depth;
    public Node parentNode = null;
    
    public Node(Vector2Int _pos)
    {
        pos = _pos;
    }

    public void CalcDist(Node dest, int cdepth)
    {
        int tmpx = dest.pos.x - pos.x;
        int tmpy = dest.pos.y - pos.y;
        distH = (tmpx * tmpx) + (tmpy * tmpy);
        depth = cdepth;
    }
}

class PathFinding
{
    private List<Node> openNodeList = new List<Node>();
    private List<Node> closeNodeList = new List<Node>();
    
    /// <summary>
    /// NOTE : 길찾기
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <param name="path"></param>
    /// <param name="navi"></param>
    /// <returns></returns>
    public bool FindPath(Node startNode, Node endNode, ref List<Node> path, MapData navi)
    {
        openNodeList.Clear();
        closeNodeList.Clear();

        Node tmpNode = startNode;
        //시작노드 추가 
        openNodeList.Add(tmpNode);

        int iDepth = 0;
        tmpNode.depth = iDepth;

        List<Node> neighborNodes = new List<Node>();


        while (true)
        {
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
            
            
            navi.GetNeighborNode(tmpNode, ref neighborNodes); //이웃노드를 가져옴

            for (int i = 0; i < neighborNodes.Count; i++)
            {
                //닫힌노드에 있는것이면 무시
                if (CheckFromCloseNode(neighborNodes[i]))
                    continue;

                neighborNodes[i].CalcDist(endNode, iDepth);
                neighborNodes[i].parentNode = tmpNode;
                InsertOpenNode(neighborNodes[i]);
            }

            SortOpenNode();
        }

        return true;
    }

    /// <summary>
    /// NOTE : 중복 노드 삽입 되지 않도록 처리
    /// </summary>
    /// <param name="tmpnode"></param>
    private void InsertOpenNode(Node tmpnode)
    {
        for( int i =0;i<openNodeList.Count; i++)
        {
            if(openNodeList[i].pos.Equals(tmpnode.pos))
            {
                closeNodeList.Add(openNodeList[i]);
                openNodeList[i] = tmpnode;
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
        Node tmpnode;

        bool bcontinue = true;

        while(bcontinue)
        {
            bcontinue = false;
            for (int i = 0; i < openNodeList.Count - 1; i++)
            {
                if(!CompareNode(openNodeList[i], openNodeList[i+1]))
                {
                    tmpnode = openNodeList[i];
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
    private bool CompareNode(Node n1, Node n2)
    {
        if (n1.distH < n2.distH)
            return true;
        if (n1.distH > n2.distH)
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
    private bool CheckFromCloseNode(Node _tmpnode)
    {
        foreach(var cn in closeNodeList)
        {
            if (cn.pos.Equals(_tmpnode.pos))
                return true;
        }
        return false;
    }


    public void PrintNode()
    {
        foreach(var ns in openNodeList)
        {
            Debug.Log("Pos : " + ns.pos);
        }
    }
}


public class MapData
{
    public Tilemap map = new Tilemap();

    public MapData(Tilemap _map)
    {
        map = _map;
    }

    /// <summary>
    /// NOTE : 이웃 검색
    /// </summary>
    /// <param name="_tmpnode"></param>
    /// <param name="nodelist"></param>
    public void GetNeighborNode(Node _tmpnode, ref List<Node> nodelist)
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

        int[] distx = new int[3] { -1, 0, 1 };
        int[] disty = new int[3] { -1, 0, 1 };

        for (int y = 0; y < 3; ++y)
        {
            for (int x = 0; x < 3; ++x)
            {

                Vector3Int tmppos = new Vector3Int(distx[x] + _tmpnode.pos.x, disty[y] + _tmpnode.pos.y, 0);

                //중앙 위치는 필요X
                if (_tmpnode.pos.x == tmppos.x && _tmpnode.pos.y == tmppos.y)
                    continue;

                //이동불가 지역 필요 X
                var tmptile = map.GetTile(tmppos);
                if (tmptile != null)
                {
                    if (tmptile.name.Equals("RuleTile_Terrain"))
                        continue;
                }
                //이동가능한 지점이면 목록에 추가하기
                nodelist.Add(new Node((Vector2Int)tmppos));
            }
        }
    }
}




