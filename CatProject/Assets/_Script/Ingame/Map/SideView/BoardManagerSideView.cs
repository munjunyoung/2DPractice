using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileManager))]
public class BoardManagerSideView : MonoBehaviour
{
    private List<DungeonRoom> roomList = new List<DungeonRoom>();
    private List<GameObject> roomGameObjectList = new List<GameObject>();

    //타일 클래스에서 복사할 tileArray
    public TileObject[] tileArray;

    //RoomParent들의 부모가 될 오브젝트
    [Header("Rooms Parent Object")]
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
    public int heightMinSize;
    [Range(30, 60)]
    public int heightMaxSize;
    
    private void Start()
    {
        tileArray = GetComponent<TileManager>().tileReferenceArray;
        CreateRooms(numberOfRoom);
        DrawRoom();
    }

    private void CreateRooms(int _numberOfRoom)
    {
        for (int i = 0; i < _numberOfRoom; i++)
        {
            roomList.Add(new DungeonRoom((int)RoomType.Type1, widthMinSize, widthMaxSize, heightMinSize, heightMaxSize));

            int groundlength = tileArray[roomList[i].roomType].tileType[(int)TileType.Ground].sprite.Length;
            int floorlength = tileArray[roomList[i].roomType].tileType[(int)TileType.Floor].sprite.Length;
            roomList[i].SetBackGround();
            roomList[i].SetGround(groundlength, floorlength);
        }
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
                    //해당 타일의 랜덤 설정
                    GameObject tmpTileObj = Instantiate(tileArray[_roomType].
                        tileType[_room.roomArray[i, j].tileType].sprite[_room.roomArray[i,j].tileNumber],
                        new Vector3(i, j, 0f), Quaternion.identity);
                    //부모 설정
                    tmpTileObj.transform.SetParent(tmpParent.transform);
                }
            }
            roomGameObjectList.Add(tmpParent);
        }
    }
}

/// <summary>
/// 
/// </summary>
internal class DungeonRoom 
{
    //해당 통로
    //public DungeonRoom left;
    //public DungeonRoom right;
    public Rect room;
    public int roomType;
    public TileMap[,] roomArray;

    public DungeonRoom(int _roomType, int _widthMin, int _widthMax, int _heightMin, int _heightMax)
    {
        roomType = _roomType;

        var tmpW = Random.Range(_widthMin, _widthMax);
        var tmpH = Random.Range(_heightMin, _heightMax);
        room = new Rect(0, 0, tmpW, tmpH);

        roomArray = new TileMap[(int)room.width, (int)room.height];
    }

    public void SetBackGround()
    {
        for(int i=0; i<room.xMax;i++)
        {
            for(int j=0; j<room.yMax;j++)
            {
                roomArray[i, j] = new TileMap();
            }
        }
    }
    
    //가로 세로 랜덤값
    private int groundWidthMin = 5;
    private int groundWidthMax = 10;
    private int groundHeightValue = 5;
    /// <summary>
    /// 땅 생성 
    /// </summary>
    /// <param name="floortilelength"></param>
    /// <param name="groundtilelength"></param>
    public void SetGround(int floortilelength, int groundtilelength)
    {
        int beforeheight = Random.Range(0, (int)(room.yMax / 3));
        int currentheight = beforeheight;
        int beforewidth = 5;
        bool changeheight = false; //높이가 바뀔 때 설정

        for (int i = 0; i < room.xMax; i++)
        {
            for (int j = currentheight; j >= 0; j--)
            {
                //제일 위일 경우 floor를 생성 
                if (j.Equals(currentheight))
                {
                    roomArray[i, j] = new TileMap((int)TileType.Floor, Random.Range(0, floortilelength));
                    continue;
                }
                else
                {
                    //높이 바뀔 경우
                    if (changeheight)
                    {
                        //땅이 높아질 경우 현재 돌고있는 땅들을 변경 i 
                        if (beforeheight < currentheight)
                        {
                            if (currentheight - beforeheight >= 2)
                            {
                                
                            }
                            else
                            {

                            }
                        }
                        //땅이 낮아질 경우 이전 땅들을 변경 i-1
                        else if (beforeheight > currentheight)
                        {
                            if (beforeheight - currentheight >= 2)
                            {

                            }
                            else
                            {

                            }
                        }
                        //높이가 2이상 일경우 테두리 생성
                        changeheight = false;
                    }
                    else
                    {
                        roomArray[i, j] = new TileMap((int)TileType.Ground, Random.Range(0, groundtilelength));
                    }
                }
            }

            //높이 변경 및 넓이 설정
            beforewidth--;
            if (beforewidth<=0)
            {
                beforeheight = currentheight;

                var tmpH = Random.Range(-groundHeightValue, groundHeightValue);
                currentheight = (currentheight + tmpH >= 0) ? currentheight + tmpH : 0;

                beforewidth = Random.Range(groundWidthMin, groundWidthMax);

                changeheight = true;
            }
        }
    }
}

class TileMap
{
    public int tileType;
    public int tileNumber;

    public TileMap(int _tiletype, int _tilenumber)
    {
        tileType = _tiletype;
        tileNumber = _tilenumber;
    }

    public TileMap()
    {
        tileType = 0;
        tileNumber = 0;
    }
}
