using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileLoadManager))]
public class BoardManagerByTile : MonoBehaviour
{
    private static BoardManagerByTile instance;
    [HideInInspector]
    public List<DungeonRoomByTile> roomList = new List<DungeonRoomByTile>();
    private Dictionary<int, List<DungeonRoomByTile>> LevelRoomDic = new Dictionary<int, List<DungeonRoomByTile>>();

    private TypeOfTileSetType[] tileReferenceArray;

    //RoomParent들의 부모가 될 오브젝트 (Grid)
    private GameObject parentModelOfRooms;

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

    //NOTE : Reference GameObject Model
    [Header("Reference GameObject Model")]
    [SerializeField]
    private GameObject entranceModel = null;

    private void Awake()
    {
        instance = this;
        //Grid 오브젝트 생성
        CreateParentGridObject();
        //오브젝트 타일 참조
        tileReferenceArray = GetComponent<TileLoadManager>().loadTileArray;
        //Rooms 생성
        CreateRooms(numberOfRoom);
        //Rooms 레벨 설정
        SetRoomLevel();
        //Rooms 연결
        RandomEdgeConnect();
        //Rooms Entrance 오브젝트 생성
        foreach(DungeonRoomByTile room in roomList)
            room.SetEntrance();
        //Rooms Draw
        DrawRoom();
        //출입문 진입시 출현할 포지션 설정
        SetConnectedEntrance();
        
    }
    
    /// <summary>
    /// NOTE : 오브젝트를 생성할때 방들의 상위 오브젝트가 될 부모 설정 함수
    /// TODO : 씬하나에 끝날 경우에는 미리 생성하는 것으로 해당 함수 제거 요망 씬이 넘어갈때마다 필요할 거라고 생각되었기 때문 (방을 남겨두기에는 하위오브젝트들을 destroy함으로써 gc가 돌꺼같나..?)
    /// </summary>
    private void CreateParentGridObject()
    {
        parentModelOfRooms = new GameObject();
        parentModelOfRooms.AddComponent<Grid>().cellGap = new Vector3(-0.01f, -0.01f, 0);
        parentModelOfRooms.transform.position = Vector3.zero;
        parentModelOfRooms.transform.rotation = Quaternion.identity;
        parentModelOfRooms.transform.localScale = Vector3.one;
        parentModelOfRooms.name = "Rooms";
    }

    /// <summary>
    /// NOTE : 파라미터 숫자만큼 DungeonRoomByTile 클래스 객체 생성 함수
    /// </summary>
    /// <param name="_numberOfroom"></param>
    private void CreateRooms(int _numberOfroom)
    {
        for (int i = 0; i < _numberOfroom; i++)
        {
            roomList.Add(new DungeonRoomByTile(i, (int)RoomType.Type1, widthMinSize, widthMaxSize, heightMinSize, heightMaxSize));

            int floorlength = tileReferenceArray[roomList[i].roomSpriteType].tileType[(int)TileType.Floor].tile.Length;
            int groundlength = tileReferenceArray[roomList[i].roomSpriteType].tileType[(int)TileType.Ground].tile.Length;
            //테두리 설정
            roomList[i].SetGroundNormal(floorlength);
            //시작방과 끝방을 제외한 나머지는 높낮이 랜덤 Ground설정
            if(i>-1&&i<_numberOfroom)
                roomList[i].SetGroundHegihtRandomly(floorlength, groundlength);
        }
    }
    
    /// <summary>
    /// NOTE : 생성 된 방들의 레벨 설정
    /// XXX : 시작 방은 같은 레벨 여러개 설정, 보스방은 한개만 설정 
    /// </summary>
    private void SetRoomLevel()
    {
        int levelCount = 0;
        int setSameLevelPer = 50;
        
        LevelRoomDic.Add(levelCount, new List<DungeonRoomByTile>());

        for (int i = 0; i < roomList.Count - 1; i++)
        {
            roomList[i].level = levelCount;
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
        if (roomList[roomList.Count - 2].level.Equals(levelCount))
        {
            levelCount++;
            LevelRoomDic.Add(levelCount, new List<DungeonRoomByTile>());
            setSameLevelPer = 50;
        }

        roomList[roomList.Count - 1].level = levelCount;
        LevelRoomDic[levelCount].Add(roomList[roomList.Count - 1]);
    }

    /// <summary>
    /// NOTE : 파라미터로 설정한 방 2개 각각 neighBorRooms 객체 추가 (출입구 연결) 
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    private void ConnectEdge(DungeonRoomByTile room1, DungeonRoomByTile room2)
    {
        room1.neighborRooms.Add(new EntranceConnectRoom(room2));
        room2.neighborRooms.Add(new EntranceConnectRoom(room1));
    }

    /// <summary>
    /// NOTE : 각 방 랜덤 연결  설명 : 개발노트 ( C.91 )
    /// </summary>
    private void RandomEdgeConnect()
    {
        for (int i = 0; i < LevelRoomDic.Count - 1; i++)
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
                        ConnectEdge(LevelRoomDic[i][tmpIndex], LevelRoomDic[i + 1][Random.Range(0, LevelRoomDic[i + 1].Count)]);
                }
            }
            //같은 레벨 방이 1개 일 경우
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
    /// NOTE : 방 연결 리스트 Log print
    /// </summary>
    private void PrintLogRoomNeighbors()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log(i + "번째 Room Neighbor! Level : " + roomList[i].level);
            for (int j = 0; j < roomList[i].neighborRooms.Count; j++)
            {
                Debug.Log("[" + i + "]번 Room <Level : " + roomList[i].level + "> -> [" + roomList[i].neighborRooms[j].connectedRoom.roomNumberOfList+ "]번 Room <Level : " + roomList[roomList[i].neighborRooms[j].connectedRoom.roomNumberOfList].level + ">");
            }
        }
    }

    /// <summary>
    /// NOTE : 생성해둔 DungeonRoomByTile객체의 정보를 통하여 설정
    /// </summary>
    private void DrawRoom()
    {
        int countroom = 1;
        foreach (DungeonRoomByTile _room in roomList)
        {
            int _roomtype = _room.roomSpriteType;
            //부모가될 오브젝트 생성
            GameObject tmpParent = new GameObject();
            tmpParent.transform.position = Vector3.zero;
            tmpParent.transform.rotation = Quaternion.identity;
            tmpParent.name = "Room" + countroom;
            tmpParent.transform.SetParent(parentModelOfRooms.transform);

            //배경 오브젝트 생성
            GameObject backgroundob = new GameObject("BackGround", typeof(SpriteRenderer));
            backgroundob.transform.localPosition = Vector3.zero;
            backgroundob.transform.localRotation = Quaternion.identity;
            backgroundob.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
            backgroundob.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
            backgroundob.GetComponent<SpriteRenderer>().sprite = tileReferenceArray[_roomtype].tileType[0].tile[0].sprite;
            backgroundob.GetComponent<SpriteRenderer>().size = _room.roomRect.size - Vector2.one;
            backgroundob.transform.SetParent(tmpParent.transform);
            
            //Ground TileMap 오브젝트 생성
            var tmpFloortilemap = CreateTileMap("Floor","Floor","Ground",tmpParent);
            var tmpGroundtilemap = CreateTileMap("Ground","Ground","Ground",tmpParent);

            //설정한 방의 배열정보를 통하여 타일 설정 및 출입문 오브젝트 생성
            for (int i = 0; i < _room.roomRect.xMax; i++)
            {
                for (int j = 0; j < _room.roomRect.yMax; j++)
                {
                    if (_room.roomArray[i, j] != null)
                    {
                        switch(_room.roomArray[i,j].tileType)
                        {
                            case TileType.Entrance:
                                GameObject tmpob = Instantiate(entranceModel, new Vector3(i, j+1f, 0), Quaternion.identity);
                                tmpob.GetComponent<SpriteRenderer>().sprite = tileReferenceArray[_roomtype].tileType[(int)_room.roomArray[i, j].tileType].tile[_room.roomArray[i, j].tileNumber].sprite;
                                tmpob.GetComponent<SpriteRenderer>().sortingLayerName = "Entrance";
                                tmpob.transform.SetParent(tmpParent.transform);
                                foreach(EntranceConnectRoom nroom in _room.neighborRooms)
                                {
                                    if (nroom.entrance == null)
                                    {
                                        tmpob.GetComponent<EntranceSc>().currentRoomNumber = _room.roomNumberOfList;
                                        nroom.entrance = tmpob.GetComponent<EntranceSc>();
                                        break;
                                    }
                                }
                                break;
                            case TileType.Floor:
                                tmpFloortilemap.SetTile(new Vector3Int(i, j, 0), tileReferenceArray[_roomtype].tileType[(int)_room.roomArray[i, j].tileType].tile[_room.roomArray[i, j].tileNumber]);
                                break;
                            case TileType.Ground:
                            case TileType.Wall:
                                tmpGroundtilemap.SetTile(new Vector3Int(i, j, 0), tileReferenceArray[_roomtype].tileType[(int)_room.roomArray[i, j].tileType].tile[_room.roomArray[i, j].tileNumber]);
                                break;
                            default:
                                
                                break;
                        }   
                    }
                }
            }
            countroom++;
            _room.roomModel = tmpParent;
            tmpParent.SetActive(false);
        }
    }

    /// <summary>
    /// NOTE : TileMap Object 동적 생성
    /// </summary>
    /// <param name="nameoftilemap"></param>
    /// <param name="tagname"></param>
    /// <param name="layername"></param>
    /// <param name="parentob"></param>
    /// <returns></returns>
    private Tilemap CreateTileMap(string nameoftilemap,string tagname, string layername, GameObject parentob)
    {
        GameObject tmpob = new GameObject(nameoftilemap, typeof(Tilemap));
        tmpob.AddComponent<TilemapRenderer>().sortingLayerName = layername;
        tmpob.AddComponent<TilemapCollider2D>().usedByComposite = true;
        tmpob.AddComponent<CompositeCollider2D>();
        tmpob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        tmpob.tag = tagname;
        tmpob.transform.SetParent(parentob.transform);
        Tilemap tmptilemap = tmpob.GetComponent<Tilemap>();

        return tmptilemap;
    }   
    
    /// <summary>
    /// NOTE : 현재 입구 스크립트에 nextEntrance 데이터 초기화
    /// NOTE : 해당 함수를 생성할때 처리하지 않은 이유는 방의 스프라이트들이 순서대로 생성되어 모두 생성한 후에 연결 가능하다.
    /// NOTE : 매번 다음 방을 진입할 때마다 다음 방의 이웃들을 검색하여 하는것보다 미리 한번에 포지션을 저장해두는 것이 효율이 좋을 것 같다.
    /// </summary>
    private void SetConnectedEntrance()
    {
        //모든 방 순회
        foreach(DungeonRoomByTile room in roomList)
        {
            //순회하는 방의 이웃룸들 순회
            foreach(EntranceConnectRoom currentroomneigbor in room.neighborRooms)
            {
                //이웃룸의 연결된 다음 방의 이웃들 순회 하여 entranceSc에 다음방 포지션 변수 초기화
                foreach (EntranceConnectRoom nextroomneighbor in roomList[currentroomneigbor.connectedRoom.roomNumberOfList].neighborRooms)
                {
                    //연결된 방의 통로중 현재 방이 nextroom일 경우 체크하여 position 저장
                    if (currentroomneigbor.entrance.currentRoomNumber.Equals(nextroomneighbor.connectedRoom.roomNumberOfList))
                        currentroomneigbor.entrance.connectedNextEntrance = nextroomneighbor.entrance;
                }

            }
        }
    }
}

/// <summary>
/// NOTE : DungeonRoom 클래스
/// TODO : 함수와 클래스 변수들을 분리해야하는 개선 사항 가능성
/// </summary>
public class DungeonRoomByTile
{
    public int roomNumberOfList = -1;
    //오브젝트
    public GameObject roomModel = null;
    //생성한 방의 정보
    public Rect roomRect = new Rect(0,0,0,0);
    public int roomSpriteType = -1;
    public TileInfo[,] roomArray;
    public int level = -1;
    public bool unLockState = false;

    public List<EntranceConnectRoom> neighborRooms = new List<EntranceConnectRoom>();

    /// <summary>
    /// NOTE : 방의 사이즈 랜덤 설정 생성자
    /// </summary>
    /// <param name="_roomNumber"></param>
    /// <param name="_roomType"></param>
    /// <param name="_widthMin"></param>
    /// <param name="_widthMax"></param>
    /// <param name="_heightMin"></param>
    /// <param name="_heightMax"></param>
    public DungeonRoomByTile(int _roomNumber, int _roomType, int _widthMin, int _widthMax, int _heightMin, int _heightMax)
    {
        roomNumberOfList = _roomNumber;
        roomSpriteType = _roomType;

        var tmpW = Random.Range(_widthMin, _widthMax);
        var tmpH = Random.Range(_heightMin, _heightMax);
        roomRect = new Rect(0, 0, tmpW, tmpH);

        roomArray = new TileInfo[(int)roomRect.width, (int)roomRect.height];
    }

    /// <summary>
    /// NOTE : 땅 depth 1 테두리 생성
    /// </summary>
    /// <param name="floortilelength"></param>
    /// <param name="groundtilelength"></param>
    public void SetGroundNormal(int floortilelength)
    {
        //bottom, top
        for (int i = 1; i < roomRect.xMax-1; i++)
        {
            roomArray[i, 0] = new TileInfo(TileType.Floor, Random.Range(0, floortilelength));
            roomArray[i, (int)roomRect.yMax-1] = new TileInfo(TileType.Floor, Random.Range(0, floortilelength));
        }

        //left, right
        for (int j = 0; j < roomRect.yMax; j++)
        {
            roomArray[0, j] = new TileInfo(TileType.Wall, 0);
            roomArray[(int)roomRect.xMax-1, j] = new TileInfo(TileType.Wall, 0);
        }
    }

    //가로 세로 랜덤값
    private int groundWidthMin = 5;
    private int groundWidthMax = 10;
    private int groundHeightValue = 5;
    private int groundMinheight = 1; //최소 floor 높이값
    /// <summary>
    /// NOTE : Ground 높이 랜덤 생성
    /// </summary>
    /// <param name="floortilelength"></param>
    /// <param name="groundtilelength"></param>
    public void SetGroundHegihtRandomly(int floortilelength, int groundtilelength)
    {
        int beforeheight = Random.Range(groundMinheight, (int)(roomRect.yMax / 3));
        int currentheight = beforeheight;
        int beforewidth = 5;
        bool changeheight = false; //높이가 바뀔 때 설정

        int heightgapcount = 0;

        for (int i = 1; i < roomRect.xMax - 1; i++)
        {
            for (int j = currentheight; j >= 0; j--)
            {
                //제일 위일 경우 floor를 생성 
                if (j.Equals(currentheight))
                {
                    roomArray[i, j] = new TileInfo(TileType.Floor, Random.Range(0, floortilelength));
                }
                else
                {
                    //높이가 바뀌지 않을 경우 
                    if (!changeheight)
                    {
                        roomArray[i, j] = new TileInfo(TileType.Ground, Random.Range(0, groundtilelength));
                    }
                    //높이 바뀔 경우
                    else
                    {
                        heightgapcount = Mathf.Abs(beforeheight - currentheight);
                        //gap count가 1이하면 그대로 생성
                        if (heightgapcount < 1)
                        {
                            roomArray[i, j] = new TileInfo(TileType.Ground, Random.Range(0, groundtilelength));
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
                                    roomArray[i, j] = new TileInfo(TileType.Wall, 0);
                                }

                            }
                            //땅이 낮아질 경우 이전 땅들을 변경 i-1 
                            else if (beforeheight > currentheight)
                            {
                                //현재 j값을 건들지 않고 처리 
                                var currentjvalue = j;
                                for (int k = currentjvalue + 1; k <= currentjvalue + heightgapcount; k++)
                                {
                                    roomArray[i - 1, k] = new TileInfo(TileType.Wall, 1);
                                }
                            }
                            roomArray[i, j] = new TileInfo(TileType.Ground, Random.Range(0, groundtilelength));
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
    
    /// <summary>
    /// NOTE : 저장된 NeighborRooms 정보를 통해 출입구 랜덤 생성 
    /// TODO : 현재는 가로값을 랜덤으로 설정하고 높이는 무조건 땅위에 생성하도록 설정하여 개선 가능성이 매우 높음
    /// </summary>
    public void SetEntrance()
    {
        List<int> posX = new List<int>();
        //Entrance갯수 만큼 x포지션 저장
        for (int i = 0; i < neighborRooms.Count; i++)
        {
            var tmpvalue = (int)Random.Range(1, roomRect.xMax - 1);

            //같은 값이 있는지 체크?
            for (int j = 0; j < posX.Count; j++)
            {
                if (posX[j].Equals(tmpvalue))
                {
                    tmpvalue = (int)Random.Range(2, roomRect.xMax - 2);
                    //다시 처음부터 확인하기 위함
                    j = 0;
                }
            }
            posX.Add(tmpvalue);
        }
        //저장한 x포지션을 기준으로 y값을 순회하여 roomarray의 0 값을 검색하여 설정
        foreach (int tmpx in posX)
        {
            for (int j = 1; j < roomRect.yMax - 1; j++)
            {
                if (roomArray[tmpx, j] == null)
                {
                    roomArray[tmpx, j] = new TileInfo(TileType.Entrance, 0);
                    break;
                }
            }
        }
    }
}

/// <summary>
/// NOTE : 출입구 클래스 연결된 방과 해당 오브젝트 (struct으로 구현하였다가 foreach문에서 반복 변수 초기화가 불가하여 class로 변경(구조체 : 값복사, 클래스 : 참조복사)
/// </summary>
public class EntranceConnectRoom
{
    public DungeonRoomByTile connectedRoom;
    public EntranceSc entrance;

    public EntranceConnectRoom(DungeonRoomByTile room)
    {
        connectedRoom = room;
        entrance = null;
    }
}