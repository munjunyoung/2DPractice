using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManagerScrollView : MonoBehaviour {
    
    public List<DungeonRoom> roomList;

    [Header("RoomSize")]
    [Range(30,100)]
    public int widthMinSize;
    [Range(20, 30)]
    public int widthMaxSize;
    [Range(15, 30)]
    public int heightSize;


    private void Start()
    {
        
    }

    private void CreateRooms()
    {

    }

    private void DrawRoom()
    {
        
    }
}

/// <summary>
/// 
/// </summary>
public class DungeonRoom
{
    public Rect room;
    public int[,] roomArray;
    

    
    public DungeonRoom(int _widthMin, int _widthMax, int _height)
    {
        room = new Rect(0, 0, Random.Range(_widthMin, _widthMax), _height);
        roomArray = new int[(int)room.width, (int)room.height];
        SetOutLine();

    }

    private void SetOutLine()
    {
        //땅과 천장 설정
        for(int i = 0; i< room.xMax; i++)
        {
            roomArray[i, (int)room.y] = 1;
            roomArray[i, (int)room.yMax] = 1;
        }
    }

    private void DrawRoom()
    {
        
    }
}
