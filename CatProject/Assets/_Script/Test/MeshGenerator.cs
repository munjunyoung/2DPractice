using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] _map, float _squareSize)
    {
        squareGrid = new SquareGrid(_map, _squareSize);
    }

    private void OnDrawGizmos()
    {
        if (squareGrid != null)
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector2.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector2.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector2.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector2.one * .4f);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreTop.position, Vector2.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreRight.position, Vector2.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreBottom.position, Vector2.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreLeft.position, Vector2.one * .15f);
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            //맵의 가로 세로 크기
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNode = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector2 pos = new Vector2(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapWidth / 2 + y * squareSize + squareSize / 2);
                    controlNode[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];

            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNode[x, y + 1], controlNode[x + 1, y + 1], controlNode[x + 1, y], controlNode[x, y]);
                }
            }
        }
    }
    /// <summary>
    /// 사각형의 노드들을 설정 
    /// </summary>
    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;

        public Square(ControlNode _topleft, ControlNode _topright, ControlNode _bottomright, ControlNode _bottomleft)
        {
            topLeft = _topleft;
            topRight = _topright;
            bottomRight = _bottomright;
            bottomLeft = _bottomleft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;
        }
    }

    /// <summary>
    /// 기본 노드 클래스 
    /// </summary>
    public class Node
    {
        public Vector2 position;
        public int vertexIndex = -1;

        public Node(Vector2 _pos)
        {
            position = _pos;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector2 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector2.up * squareSize / 2f);
            right = new Node(position + Vector2.right * squareSize / 2f);

        }
    }
}
