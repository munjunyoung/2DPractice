using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator2 : MonoBehaviour {

    private int[,] map;
    [Header("MAP Size")]
    [Range(0, 100)]
    public int height;
    [Range(0, 100)]
    public int width;

    [Header("SPRITE Reference")]

    public List<GameObject> Fence;
    public List<GameObject> Weed;
    public List<GameObject> Road;


    // Use this for initialization
    void Start() {
    }

    void GenerateMap()
    {
        map = new int[width, height];

    }

    /// <summary>
    /// 테두리 설정
    /// </summary>
    void SetEdge()
    {
        // 커브 테두리 LB : 0,0 RB : 20,0 LT : 0,10 RT : 20,10
        // 문위치 십자 CL : 0,5 CR : 20,5 CB : 10,0 CB : 10,10  
        // 수직 fence -> L(0,1~9) R(20,1~9) 수평 fence -> B(1~19,0) T(1~19,10)
        // 스프라이트 이미지 0 : weed 1 
        // Road : 10~ :
        //Fence : LB - 20, RB - 21, LT - 22, RT - 24, Vertical - 25, horizontal - 26

        //테두리 값 설정
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 2;
                else
                    map[x, y] = 0;
            }
        }
    }
    
    void DrawMap()
    {
        /*
        //LB
        if (x == 0 && y == 0)
            map[x, y] = 20;
        //RB
        else if (x == width - 1 && y == 0)
            map[x, y] = 21;
        //LT
        else if (x == 0 && y == height - 1)
            map[x, y] = 22;
        //RT
        else if (x == height && y == height - 1)
            map[x, y] = 24;
        //vertical 
        else if (y == 0 || y == height - 1)
            map[x, y] = 25;
        //horizontal
        else if (x == 0 || x == height - 1)
            map[x, y] = 26;
            */
    }
    
}

public class Room
{
    //0 : Top 1 : Left 2 : Bottom 3 : Right 해당 룸의 각 사방 룸 확인
    public Room[] surroundRoom = new Room[4];
    
}
