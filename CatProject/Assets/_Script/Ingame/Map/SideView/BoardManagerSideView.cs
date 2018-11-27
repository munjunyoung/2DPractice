using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileManager))]
public class BoardManagerSideView : MonoBehaviour
{
    private List<DungeonRoom> roomList = new List<DungeonRoom>();
    private List<GameObject> roomObjectParent = new List<GameObject>();

    public TileManager tileRefereceSc;

    public Transform RoomsObject;

    [Header("RoomNumber")]
    [Range(1, 100)]
    public int numberOfRoom;

    [Header("RoomSize")]
    [Range(20, 30)]
    public int widthMinSize;
    [Range(30, 100)]
    public int widthMaxSize;
    [Range(15, 30)]
    public int heightSize;

    private void Start()
    {
        CreateRooms(numberOfRoom);
        DrawRoom();
    }

    private void CreateRooms(int _numberOfRoom)
    {
        for(int i = 0; i<_numberOfRoom; i++)
            roomList.Add(new DungeonRoom((int)RoomType.Type1, widthMinSize, widthMaxSize, heightSize));
    }

    private void DrawRoom()
    {
        foreach (DungeonRoom _room in roomList)
        {
            GameObject tmpParent = new GameObject();
            tmpParent.transform.SetParent(RoomsObject);
            tmpParent.transform.position = Vector3.zero;
            tmpParent.transform.rotation = Quaternion.identity;
            tmpParent.name = "Room";
            int _roomType = _room.roomType;

            for (int i = 0; i < _room.room.xMax; i++)
            {
                for (int j = 0; j < _room.room.yMax; j++)
                {
                    GameObject tmpTileObj = Instantiate(tileRefereceSc.TileReferenceArray[_roomType].
                        tileType[_room.roomArray[i,j]].sprite[0], new Vector3(i, j, 0f), Quaternion.identity);
                    //부모 설정
                    tmpTileObj.transform.SetParent(tmpParent.transform);
                    roomObjectParent.Add(tmpTileObj);
                }
            }
        }
    }
}

/// <summary>
/// 
/// </summary>
public class DungeonRoom
{
    //해당 통로
    //public DungeonRoom left;
    //public DungeonRoom right;

    public Rect room;
    public int roomType;
    public int[,] roomArray;

    public DungeonRoom(int _roomType, int _widthMin, int _widthMax, int _height)
    {
        roomType = _roomType;

        var tmpW = Random.Range(_widthMin, _widthMax);
        room = new Rect(0, 0, tmpW, _height);

        roomArray = new int[(int)room.width, (int)room.height];
        SetOutLine();
    }

    private void SetOutLine()
    {
        //땅과 천장 설정
        for (int i = 0; i < room.xMax; i++)
        {
            roomArray[i, (int)room.y] = (int)TileType.Wall;
            roomArray[i, (int)room.yMax - 1] = (int)TileType.Wall;
        }
    }
}
