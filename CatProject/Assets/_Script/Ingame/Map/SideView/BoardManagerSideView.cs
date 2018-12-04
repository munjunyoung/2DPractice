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
    [Header("ROOM OPTION")]
    public Transform RoomsObject;
    [Range(1, 100)]
    public int numberOfRoom;
    [Range(30, 60)]
    public int widthMinSize;
    [Range(60, 100)]
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
        roomGameObjectList[0].SetActive(true);
    }

    private void CreateRooms(int _numberOfRoom)
    {
        for (int i = 0; i < _numberOfRoom; i++)
        {
            roomList.Add(new DungeonRoom((int)RoomType.Type1, widthMinSize, widthMaxSize, heightMinSize, heightMaxSize));

            int groundlength = tileArray[roomList[i].roomType].tileType[(int)TileType.Ground].sprite.Length;
            int floorlength = tileArray[roomList[i].roomType].tileType[(int)TileType.Floor].sprite.Length;
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
                    if (_room.roomArray[i, j] != null)
                    {
                        //해당 타일의 랜덤 설정
                        GameObject tmpTileObj = Instantiate(tileArray[_roomType].
                            tileType[_room.roomArray[i, j].tileType].sprite[_room.roomArray[i, j].tileNumber],
                            new Vector3(i, j, 0f), Quaternion.identity);
                        //부모 설정
                        tmpTileObj.transform.SetParent(tmpParent.transform);
                    }
                }
            }
            //배경 셋팅
            var xmax = _room.room.xMax;
            var ymax = _room.room.yMax;
            Vector2 centerpos = new Vector3((xmax / 2) - 0.5f, (ymax / 2) - 0.5f);
            var tmpbackground = Instantiate(tileArray[_roomType].tileType[(int)TileType.BackGround].sprite[0], centerpos, Quaternion.identity);
            tmpbackground.transform.localScale = new Vector2(xmax, ymax);
            tmpbackground.transform.SetParent(tmpParent.transform);

            //부모 설정
            roomGameObjectList.Add(tmpParent);
            tmpParent.SetActive(false);
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


    //가로 세로 랜덤값
    private int groundWidthMin = 5;
    private int groundWidthMax = 10;
    private int groundHeightValue = 5;
    private int groundMinheight = 1; //최소 floor 높이값
    /// <summary>
    /// 땅 생성 
    /// </summary>
    /// <param name="floortilelength"></param>
    /// <param name="groundtilelength"></param>
    public void SetGround(int floortilelength, int groundtilelength)
    {
        int beforeheight = Random.Range(groundMinheight, (int)(room.yMax / 3));
        int currentheight = beforeheight;
        int beforewidth = 5;
        bool changeheight = false; //높이가 바뀔 때 설정

        int heightgapcount = 0;

        for (int i = 0; i < room.xMax; i++)
        {
            for (int j = currentheight; j >= 0; j--)
            {
                //제일 위일 경우 floor를 생성 
                if (j.Equals(currentheight))
                {
                    roomArray[i, j] = new TileMap((int)TileType.Floor, Random.Range(0, floortilelength));
                }
                else
                {
                    //높이가 바뀌지 않을 경우 
                    if (!changeheight)
                    {
                        roomArray[i, j] = new TileMap((int)TileType.Ground, Random.Range(0, groundtilelength));
                    }
                    //높이 바뀔 경우
                    else
                    {
                        heightgapcount = Mathf.Abs(beforeheight - currentheight);
                        //gap count가 1이하면 그대로 생성
                        if (heightgapcount < 1)
                        {
                            roomArray[i, j] = new TileMap((int)TileType.Ground, Random.Range(0, groundtilelength));
                        }
                        //gap count가 2이상일 경우 생성
                        else
                        {
                            //땅이 높아질 경우 현재 땅들을 변경
                            if (beforeheight < currentheight)
                            {
                                //현재 j값의 - gapcount만큼 j값을 감소시키면서 셋팅함
                                var currentjvalue = j;
                                for (; j > (currentjvalue - heightgapcount); j--)
                                {
                                    roomArray[i, j] = new TileMap((int)TileType.GroundOutLine, 0);
                                }

                            }
                            //땅이 낮아질 경우 이전 땅들을 변경 i-1 
                            else if (beforeheight > currentheight)
                            {
                                //현재 j값을 건들지 않고 처리 
                                var currentjvalue = j;
                                for (int k = currentjvalue + 1; k <= currentjvalue + heightgapcount; k++)
                                {
                                    roomArray[i - 1, k] = new TileMap((int)TileType.GroundOutLine, 1);
                                }
                            }
                            roomArray[i, j] = new TileMap((int)TileType.Ground, Random.Range(0, groundtilelength));
                            changeheight = false;
                        }
                    }
                }
            }

            //높이 변경 및 넓이 설정
            beforewidth--;
            if (beforewidth <= 0)
            {
                beforeheight = currentheight;
                //랜덤으로 설정할 경우 -가 계속되어 1이하로 내려가는 경우가 생기기 때문에 tmph를 따로 설정 하여 currenth를 따로 초기화
                var tmpH = Random.Range(-groundHeightValue, groundHeightValue);
                currentheight = (currentheight + tmpH >= groundMinheight) ? currentheight + tmpH : groundMinheight;

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
