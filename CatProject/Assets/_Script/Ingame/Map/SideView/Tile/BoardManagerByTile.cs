using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(TileLoadManager))]
public class BoardManagerByTile : MonoBehaviour
{
    [HideInInspector]
    public List<DungeonRoomByTile> roomList = new List<DungeonRoomByTile>();
    private Dictionary<int, List<DungeonRoomByTile>> LevelRoomDic = new Dictionary<int, List<DungeonRoomByTile>>();
    private TypeOfTileSetType[] tileReferenceArray;

    //RoomParent들의 부모가 될 오브젝트 (Grid)
    private GameObject RoomsParentObject;

    [Header("ROOM OPTION")]
    [SerializeField, Range(1, 100)]
    private int numberOfRoom;
    [SerializeField, Range(30, 60)]
    private int widthMinSize;
    [SerializeField, Range(60, 100)]
    private int widthMaxSize;
    [SerializeField, Range(15, 30)]
    private int heightMinSize;
    [SerializeField, Range(30, 60)]
    private int heightMaxSize;

    private void Awake()
    {
        CreateParentGridObject();
        tileReferenceArray = GetComponent<TileLoadManager>().loadTileArray;
        //Rooms 생성
        CreateRooms(numberOfRoom);
        //Rooms 레벨 설정
        SetRoomLevel();
        //Rooms 연결
        RandomEdgeConnected();

        PrintTest();
        //Rooms Draw
        DrawRoom();

        roomList[0].roomOwnObjecet.SetActive(true);
    }

    /// <summary>
    /// 오브젝트를 생성할때 방들의 상위 오브젝트가 될 부모 설정
    /// 함수로 만든 이유는 씬이 넘어갈때마다 필요할 거라고 생각되었기 때문 (방을 남겨두기에는 하위오브젝트들을 destroy함으로써 gc가 돌꺼같나..?)
    /// 씬하나에 끝날 경우에는 미리 생성해두어도 상관없을듯
    /// </summary>
    private void CreateParentGridObject()
    {
        RoomsParentObject = new GameObject();
        RoomsParentObject.AddComponent<Grid>();
        RoomsParentObject.transform.position = Vector3.zero;
        RoomsParentObject.transform.rotation = Quaternion.identity;
        RoomsParentObject.transform.localScale = Vector3.one;
        RoomsParentObject.name = "Rooms";
    }

    /// <summary>
    /// 파라미터 숫자만큼 방 생성
    /// </summary>
    /// <param name="_numberOfroom"></param>
    private void CreateRooms(int _numberOfroom)
    {
        for(int i =0; i<_numberOfroom; i++)
        {
            roomList.Add(new DungeonRoomByTile(i,(int)RoomType.Type1, widthMinSize, widthMaxSize, heightMinSize, heightMaxSize));

            int floorlength = tileReferenceArray[roomList[i].roomType].tileType[(int)TileType.Floor].tile.Length;
            int groundlength = tileReferenceArray[roomList[i].roomType].tileType[(int)TileType.Ground].tile.Length;
            
            roomList[i].SetGroundNormal(floorlength);           
        }
    }
    
    private int levelCount;
    private int setSameLevelPer = 50;
    /// <summary>
    /// 방들의 레벨 설정
    /// </summary>
    private void SetRoomLevel()
    {
        //시작 0레벨 방 설정;
        levelCount = 0;
        LevelRoomDic.Add(levelCount, new List<DungeonRoomByTile>());
        
        for (int i = 0; i < roomList.Count-1; i++)
        {
            roomList[i].Level = levelCount;
            LevelRoomDic[levelCount].Add(roomList[i]);

            if (Random.Range(0, 100) > setSameLevelPer)
            {
                setSameLevelPer = 50;
                levelCount++;
                LevelRoomDic.Add(levelCount, new List<DungeonRoomByTile>());
            }           
            else
            {
                setSameLevelPer -= 25;
            }
        }

        //마지막 보스방 설정
        if (roomList[roomList.Count - 2].Level.Equals(levelCount))
        {
            levelCount++;
            LevelRoomDic.Add(levelCount, new List<DungeonRoomByTile>());
            setSameLevelPer = 50;
        }
        
        roomList[roomList.Count-1].Level = levelCount;
        LevelRoomDic[levelCount].Add(roomList[roomList.Count - 1]);
    }

    /// <summary>
    /// 두개 연결
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    private void ConnectEdge(DungeonRoomByTile room1, DungeonRoomByTile room2)
    {
        room1.neighborRooms.Add(room2);
        room2.neighborRooms.Add(room1);
    }

    /// <summary>
    /// 각 방 랜덤 연결
    /// </summary>
    private void RandomEdgeConnected()
    {
        //..RoomList;
        for (int i = 0; i < LevelRoomDic.Count-1; i++)
        {
            //같은 레벨 방이 2개 이상 있을 경우
            if (LevelRoomDic[i].Count >= 2)
            {
                for (int j = 0; j < LevelRoomDic[i].Count - 1; j++)
                    ConnectEdge(LevelRoomDic[i][j], LevelRoomDic[i][j + 1]);
                
                //다음레벨로 길이 끊기지 않도록 한개는 우선 적용 (우선 적용할 방은 같은 레벨을 가진 방중 랜덤으로 선택)
                var s = Random.Range(0, LevelRoomDic[i].Count - 1);
                ConnectEdge(LevelRoomDic[i][s], LevelRoomDic[i + 1][Random.Range(0, LevelRoomDic[i + 1].Count - 1)]);

                for (int k = 0; k < LevelRoomDic[i].Count - 1; k++)
                {
                    s++;
                    var tmpIndex = s % LevelRoomDic[i].Count;
                    if (Random.Range(0, 100) > 50)
                        ConnectEdge(LevelRoomDic[i][tmpIndex], LevelRoomDic[i+1][Random.Range(0, LevelRoomDic[i + 1].Count)]);
                }
            }
            else if (LevelRoomDic[i].Count == 1)
            {
                ConnectEdge(LevelRoomDic[i][0], LevelRoomDic[i + 1][Random.Range(0, LevelRoomDic[i + 1].Count - 1)]);
            }
            else
            {
                Debug.Log(" 레벨 [" + i + "] 를 가진 방이 존재하지 않습니다.");
            }
        }
    }

    /// <summary>
    /// 테스트
    /// </summary>
    private void PrintTest()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log("Room[" + i + "] : " + roomList[i].Level);
        }
        
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log(i + "번째 Room Neighbor! Level : " + roomList[i].Level);
            for (int j = 0; j < roomList[i].neighborRooms.Count; j++)
            {
                Debug.Log("[" + i + "]번 Room <Level : " + roomList[i].Level + "> -> [" + roomList[i].neighborRooms[j].roomNumber + "]번 Room <Level : " + roomList[roomList[i].neighborRooms[j].roomNumber].Level+">");
            }
        }
    }

    /// <summary>
    /// 생성해둔 DungeonRoomByTile객체의 정보를 통하여 설정
    /// </summary>
    private void DrawRoom()
    {
        int countroom = 1;
        foreach(DungeonRoomByTile _room in roomList)
        {
            GameObject tmpParent = new GameObject();
            tmpParent.transform.position = Vector3.zero;
            tmpParent.transform.rotation = Quaternion.identity;
            tmpParent.name = "Room" + countroom;
            tmpParent.transform.SetParent(RoomsParentObject.transform);

            int _roomtype = _room.roomType;

            var tmpTilemap = CreateTileMap("TileMap_Ground");
            tmpTilemap.transform.SetParent(tmpParent.transform);
            //타일 셋
            for (int i = 0; i < _room.room.xMax; i++)
            {
                for (int j = 0; j < _room.room.yMax; j++)
                {
                    if (_room.roomArray[i, j] != null)
                        tmpTilemap.SetTile(new Vector3Int(i, j, 0), tileReferenceArray[_roomtype].tileType[_room.roomArray[i,j].tileType].tile[_room.roomArray[i,j].tileNumber]);
                }
            }
            countroom++;
            _room.roomOwnObjecet = tmpParent;
            tmpParent.SetActive(false);
        }
    }

    /// <summary>
    /// TileMap Object 동적 생성
    /// </summary>
    /// <param name="nameoftilemap"></param>
    /// <returns></returns>
    private Tilemap CreateTileMap(string nameoftilemap)
    {
        GameObject tmpob = new GameObject(nameoftilemap, typeof(Tilemap));
        tmpob.AddComponent<TilemapRenderer>();
        tmpob.AddComponent<TilemapCollider2D>();
        tmpob.GetComponent<TilemapCollider2D>().usedByComposite = true;
        tmpob.AddComponent<CompositeCollider2D>();
        tmpob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        tmpob.tag = "Ground";

        Tilemap tmptilemap = tmpob.GetComponent<Tilemap>();

        return tmptilemap;
    }
    
}

public class DungeonRoomByTile
{
    //자신의 오브젝트 (오브젝트 리스트를 따로 만들어서 관리하지 않기 위함)
    public int roomNumber;
    public GameObject roomOwnObjecet;

    public Rect room;
    public int roomType;
    public TileInfo[,] roomArray;

    //0은 왼쪽 1은 중앙 2는 오른쪽
    public List<DungeonRoomByTile> neighborRooms = new List<DungeonRoomByTile>();
    //public DungeonRoomByTile nextRoom = null;
    //public DungeonRoomByTile prevRoom = null;
    //방의 가중치 
    public int Level;


    public DungeonRoomByTile(int _roomNumber, int _roomType, int _widthMin, int _widthMax, int _heightMin, int _heightMax)
    {
        roomNumber = _roomNumber;
        roomType = _roomType;

        var tmpW = Random.Range(_widthMin, _widthMax);
        var tmpH = Random.Range(_heightMin, _heightMax);
        room = new Rect(0, 0, tmpW, tmpH);

        roomArray = new TileInfo[(int)room.width, (int)room.height];
    }

    /// <summary>
    /// 땅 depth 1 테두리 생성
    /// </summary>
    /// <param name="floortilelength"></param>
    /// <param name="groundtilelength"></param>
    public void SetGroundNormal(int floortilelength)
    {
        //bottom, top
        for (int i = 1; i < room.xMax - 1; i++)
        {
            roomArray[i, 0] = new TileInfo((int)TileType.Floor, Random.Range(0, floortilelength));
            roomArray[i, (int)room.yMax - 1] = new TileInfo((int)TileType.Floor, Random.Range(0, floortilelength));
        }

        //left, right
        for (int j = 0; j < room.yMax; j++)
        {
            roomArray[0, j] = new TileInfo((int)TileType.GroundOutLine, 0);
            roomArray[(int)room.xMax - 1, j] = new TileInfo((int)TileType.GroundOutLine, 0);
        }
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
    public void SetGroundHegihtRandomly(int floortilelength, int groundtilelength)
    {
        int beforeheight = Random.Range(groundMinheight, (int)(room.yMax / 3));
        int currentheight = beforeheight;
        int beforewidth = 5;
        bool changeheight = false; //높이가 바뀔 때 설정

        int heightgapcount = 0;

        for (int i = 1; i < room.xMax-1; i++)
        {
            for (int j = currentheight; j >= 0; j--)
            {
                //제일 위일 경우 floor를 생성 
                if (j.Equals(currentheight))
                {
                    roomArray[i, j] = new TileInfo((int)TileType.Floor, Random.Range(0, floortilelength));
                }
                else
                {
                    //높이가 바뀌지 않을 경우 
                    if (!changeheight)
                    {
                        roomArray[i, j] = new TileInfo((int)TileType.Ground, Random.Range(0, groundtilelength));
                    }
                    //높이 바뀔 경우
                    else
                    {
                        heightgapcount = Mathf.Abs(beforeheight - currentheight);
                        //gap count가 1이하면 그대로 생성
                        if (heightgapcount < 1)
                        {
                            roomArray[i, j] = new TileInfo((int)TileType.Ground, Random.Range(0, groundtilelength));
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
                                    roomArray[i, j] = new TileInfo((int)TileType.GroundOutLine, 0);
                                }

                            }
                            //땅이 낮아질 경우 이전 땅들을 변경 i-1 
                            else if (beforeheight > currentheight)
                            {
                                //현재 j값을 건들지 않고 처리 
                                var currentjvalue = j;
                                for (int k = currentjvalue + 1; k <= currentjvalue + heightgapcount; k++)
                                {
                                    roomArray[i - 1, k] = new TileInfo((int)TileType.GroundOutLine, 1);
                                }
                            }
                            roomArray[i, j] = new TileInfo((int)TileType.Ground, Random.Range(0, groundtilelength));
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





//public Tile[] ground;
//public Tile[] wall;
//public Tilemap map;


//GameObject room = new GameObject("Room", typeof(Grid));
//GameObject t=new GameObject("TileMap_Ground", typeof(Tilemap));
//t.AddComponent<TilemapRenderer>();
//map = t.GetComponent<Tilemap>();
//t.AddComponent<TilemapCollider2D>();
//t.GetComponent<TilemapCollider2D>().usedByComposite = true;
//t.AddComponent<CompositeCollider2D>();
//t.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
//t.transform.parent = room.transform;
//t.tag = "Ground";
//MapSetTest(100,50);

//private void MapSetTest(int width, int height)
//{
//    //for (int i = 0; i < width; i++)
//    //{
//    //    var t1 = new Vector3Int(i, 0, 0);
//    //    var t2 = new Vector3Int(i, height, 0);
//    //    map.SetTile(t1, ground[Random.Range(0, 3)]);
//    //    map.SetTile(t2, ground[Random.Range(0, 3)]);
//    //}

//    //for (int j = 0; j < height; j++)
//    //{
//    //    var t1 = new Vector3Int(0, j, 0);
//    //    var t2 = new Vector3Int(width, j, 0);
//    //    map.SetTile(t1, wall[0]);
//    //    map.SetTile(t2, wall[1]);
//    //}
//}