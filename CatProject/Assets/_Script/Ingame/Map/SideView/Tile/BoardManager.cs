using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [HideInInspector]
    public List<DungeonRoom> roomList = new List<DungeonRoom>();
    private Dictionary<int, List<DungeonRoom>> LevelRoomDic = new Dictionary<int, List<DungeonRoom>>();

    private LoadDataManager loadData;
    //Room들의 부모가 될 오브젝트 (Grid)
    private GameObject parentModelOfRooms;

    [Header("ROOM OPTION")]
    [SerializeField, Range(3, 100)]
    private int numberOfRoom;
    [SerializeField, Range(30, 60)]
    private int widthMinSize;
    [SerializeField, Range(60, 100)]
    private int widthMaxSize;
    [SerializeField, Range(30, 60)]
    private int heightMinSize;
    [SerializeField, Range(60, 100)]
    private int heightMaxSize;
    
    private void Awake()
    {
        loadData = new LoadDataManager();
        //Grid 오브젝트 생성
        CreateParentGridObject();
        //Rooms 생성
        CreateRooms(numberOfRoom);
    }

    #region Create Room
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
        //DungeonRoom 클래스 생성
        for (int i = 0; i < _numberOfroom; i++)
            roomList.Add(new DungeonRoom(i, (int)Room_TileType.Type2, widthMinSize, widthMaxSize, heightMinSize, heightMaxSize));
        //RoomLevel설정
        SetRoomLevel();
        //0레벨을 제외한 나머지 랜덤 Terrain 설정
        foreach(var room in roomList)
        {
            if (!room.level.Equals(0))
                SetRandomTerrainRoom(room);
        }       
        //Rooms 연결
        RandomEdgeConnect();
        //Rooms Entrance 오브젝트 생성
        foreach (DungeonRoom room in roomList)
            room.SetEntrancePos();
        
        //Rooms Draw
        DrawRoom();
        //출입문 진입시 출현할 포지션 설정
        SetConnectedEntrance();
        //적생성
        CreateMonster();
    }
    
    /// <summary>
    /// NOTE : 미리 저장해둔 지형들을 사이즈를 검색하여 랜덤으로 선택하고 배치
    /// </summary>
    private void SetRandomTerrainRoom(DungeonRoom _tmpRoom)
    {
        //Terrain을 배치할때 각 Terrain마다 높이를 고려해야 하므로
        int possibleJumpHeightValue = 5;
        var remainXSize = _tmpRoom.roomRect.xMax - _tmpRoom.currentXPos;
        List<GeneratedTerrainData> possibleTerrain = new List<GeneratedTerrainData>();

        foreach (var tmpt in loadData.terrainDataList)
        {
            //현재 남아있는 Xsize와 ysize를 방에 들어갈수있는지 체크하고 임시 생성한 리스트에 추가
            if (remainXSize > tmpt.size.xMax && (_tmpRoom.roomRect.yMax) > tmpt.size.yMax)
            {
                //Terrain을 처음 생성할 때
                if (_tmpRoom.beforeTerrainData == null)
                {
                    possibleTerrain.Add(tmpt);
                }
                else
                {
                    if (Mathf.Abs(_tmpRoom.beforeTerrainData.endHeight - tmpt.startHeight) <= possibleJumpHeightValue)
                        possibleTerrain.Add(tmpt);
                }
            }
        }

        //한개라도 가능한 지형이 있을경우 선택하여 타일 저장
        if (possibleTerrain.Count > 0)
        {
            GeneratedTerrainData selectedTerrain = possibleTerrain[Random.Range(0, possibleTerrain.Count - 1)];

            //Debug.Log("START : " + selectedTerrain.startHeight + ", END : " + selectedTerrain.endHeight);
            //현재 비어있는Room의 x값 초기화
            for (int i = 0; i < selectedTerrain.size.xMax; i++)
            {
                for (int j = 0; j < selectedTerrain.size.yMax; j++)
                {
                    if (_tmpRoom.roomGroundArray[_tmpRoom.currentXPos + i, j] == null)
                        _tmpRoom.roomGroundArray[_tmpRoom.currentXPos + i, j] = selectedTerrain.tileArray[i, j];
                }
            }

            _tmpRoom.beforeTerrainData = selectedTerrain;
            _tmpRoom.currentXPos = _tmpRoom.currentXPos + selectedTerrain.size.xMax;
           
            
            SetRandomTerrainRoom(_tmpRoom);
        }
        //없을 경우 마지막으로 끝나는 지형의 나머지 값을 체크
        else
        {
            //음수처리 안해도될듯 하다
            int nextheight = Random.Range(_tmpRoom.beforeTerrainData.endHeight - possibleJumpHeightValue, _tmpRoom.beforeTerrainData.endHeight + possibleJumpHeightValue);
            
            for(int i = _tmpRoom.currentXPos; i<_tmpRoom.roomRect.xMax;i++)
            {
                for(int j = 0; j<nextheight;j++)
                    _tmpRoom.roomGroundArray[i, j] = new TileInfo(TileType.Terrain);
            }
        }
    }
    #endregion

    #region ConnectRoom
    /// <summary>
    /// NOTE : 생성 된 방들의 레벨 설정, 방은 항상 3개 이상 생성해야한다.
    /// XXX : 시작 방은 같은 레벨 여러개 설정, 보스방은 한개만 설정 
    /// </summary>
    private void SetRoomLevel()
    {
        if (roomList.Count < 2)
            return;
        //0번쨰 방 - 시작방 설정 
        LevelRoomDic.Add(0, new List<DungeonRoom>());
        roomList[0].level = 0;
        LevelRoomDic[0].Add(roomList[0]);

        int levelCount = 1;
        int setSameLevelPer = 50;

        LevelRoomDic.Add(levelCount, new List<DungeonRoom>());

        for (int i = 1; i < roomList.Count - 1; i++)
        {
            roomList[i].level = levelCount;
            LevelRoomDic[levelCount].Add(roomList[i]);

            if (Random.Range(0, 100) > setSameLevelPer)
            {
                setSameLevelPer = 50;
                levelCount++;
                LevelRoomDic.Add(levelCount, new List<DungeonRoom>());
            }
            else
            {
                setSameLevelPer -= 25;
            }
        }

        //마지막 방 - 보스방 설정
        if (roomList[roomList.Count - 2].level.Equals(levelCount))
        {
            levelCount++;
            LevelRoomDic.Add(levelCount, new List<DungeonRoom>());
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
    private void ConnectEdge(DungeonRoom room1, DungeonRoom room2)
    {
        room1.entranceInfoList.Add(new EntranceConnectRoom(room2));
        room2.entranceInfoList.Add(new EntranceConnectRoom(room1));
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
            for (int j = 0; j < roomList[i].entranceInfoList.Count; j++)
                Debug.Log("[" + i + "]번 Room <Level : " + roomList[i].level + "> -> [" + roomList[i].entranceInfoList[j].connectedRoom.roomNumberOfList + "]번 Room <Level : " + roomList[roomList[i].entranceInfoList[j].connectedRoom.roomNumberOfList].level + ">");
        }
    }

    /// <summary>
    /// NOTE : 현재 입구 스크립트에 nextEntrance 데이터 초기화
    /// NOTE : 해당 함수를 생성할때 처리하지 않은 이유는 방의 스프라이트들이 순서대로 생성되어 모두 생성한 후에 연결 가능하다.
    /// NOTE : 매번 다음 방을 진입할 때마다 다음 방의 이웃들을 검색하여 하는것보다 미리 한번에 포지션을 저장해두는 것이 효율이 좋을 것 같다.
    /// </summary>
    private void SetConnectedEntrance()
    {
        //모든 방 순회
        foreach (DungeonRoom room in roomList)
        {
            //순회하는 방의 이웃룸들 순회
            foreach (EntranceConnectRoom currentroomneigbor in room.entranceInfoList)
            {
                //이웃룸의 연결된 다음 방의 이웃들 순회 하여 entranceSc에 다음방 포지션 변수 초기화
                foreach (EntranceConnectRoom nextroomneighbor in roomList[currentroomneigbor.connectedRoom.roomNumberOfList].entranceInfoList)
                {
                    //연결된 방의 통로중 현재 방이 nextroom일 경우 체크하여 position 저장
                    if (currentroomneigbor.entrance.currentRoomNumber.Equals(nextroomneighbor.connectedRoom.roomNumberOfList))
                        currentroomneigbor.entrance.connectedNextEntrance = nextroomneighbor.entrance;
                }

            }
        }
    }
    #endregion

    #region Draw
    /// <summary>
    /// NOTE : 생성해둔 DungeonRoomByTile객체의 정보를 통하여 설정
    /// </summary>
    private void DrawRoom()
    {
        int countroom = 1;
        foreach (DungeonRoom _room in roomList)
        {
            int _roomtype = _room.roomSpriteType;
            //부모가될 오브젝트 생성
            GameObject tmpParent = new GameObject();
            tmpParent.transform.position = Vector3.zero;
            tmpParent.transform.rotation = Quaternion.identity;
            tmpParent.name = "Room" + countroom;
            tmpParent.transform.SetParent(parentModelOfRooms.transform);
            
            CreateBackGround(_room).transform.SetParent(tmpParent.transform);

            //Ground TileMap 오브젝트 생성
            //var tmpFloortilemap = CreateTileMap("Floor", "Floor", "Ground", tmpParent);
            var tmpterraintilemapob = CreateTileMap("Ground", "Ground", "Ground", tmpParent);
            tmpterraintilemapob = DrawRoomEdge(tmpterraintilemapob ,_room);
            //설정한 방의 배열정보를 통하여 타일 설정 및 출입문 오브젝트 생성
            for (int i = 0; i < _room.roomRect.xMax; i++)
            {
                for (int j = 0; j < _room.roomRect.yMax; j++)
                {
                    if (_room.roomGroundArray[i, j] != null)
                    {
                        switch (_room.roomGroundArray[i, j].tileType)
                        {
                            case TileType.Entrance:
                                GameObject tmpob = Instantiate(loadData.structurePrefab[TileType.Entrance.ToString()], new Vector3(i, j + 1f, 0), Quaternion.identity);
                                tmpob.GetComponent<SpriteRenderer>().sprite = loadData.tileDataArray[_roomtype].tileType[(int)_room.roomGroundArray[i, j].tileType].tile[_room.roomGroundArray[i, j].tileNumber].sprite;
                                tmpob.GetComponent<SpriteRenderer>().sortingLayerName = "Entrance";
                                tmpob.GetComponent<EntranceSc>().doorOpenSprite = loadData.tileDataArray[_roomtype].tileType[(int)_room.roomGroundArray[i, j].tileType].tile[_room.roomGroundArray[i, j].tileNumber+1].sprite;
                                tmpob.transform.SetParent(tmpParent.transform);
                                foreach (EntranceConnectRoom nroom in _room.entranceInfoList)
                                {
                                    if (nroom.entrance == null)
                                    {
                                        tmpob.GetComponent<EntranceSc>().currentRoomNumber = _room.roomNumberOfList;
                                        nroom.entrance = tmpob.GetComponent<EntranceSc>();
                                        break;
                                    }
                                }
                                break;
                            case TileType.Terrain:
                                tmpterraintilemapob.SetTile(new Vector3Int(i, j, 0), loadData.tileDataArray[_roomtype].terrainRuleTile);
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
    /// NOTE : 모든 방의 테두리 생성
    /// </summary>
    /// <param name="_terraintilemap"></param>
    /// <param name="_room"></param>
    /// <returns></returns>
    private Tilemap DrawRoomEdge(Tilemap _terraintilemap, DungeonRoom _room)
    {
        //bottom, top
        for (int i = 0; i < _room.roomRect.xMax; i++)
        {
            _terraintilemap.SetTile(new Vector3Int(i, -1, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
            _terraintilemap.SetTile(new Vector3Int(i, -2, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
            _terraintilemap.SetTile(new Vector3Int(i, (int)_room.roomRect.yMax, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
            _terraintilemap.SetTile(new Vector3Int(i, (int)_room.roomRect.yMax + 1, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
        }

        //left, right
        for (int j = -2; j < _room.roomRect.yMax + 2; j++)
        {
            _terraintilemap.SetTile(new Vector3Int(-1, j, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
            _terraintilemap.SetTile(new Vector3Int(-2, j, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
            _terraintilemap.SetTile(new Vector3Int((int)_room.roomRect.xMax, j, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
            _terraintilemap.SetTile(new Vector3Int((int)_room.roomRect.xMax+1, j, 0), loadData.tileDataArray[_room.roomSpriteType].terrainRuleTile);
        }
        return _terraintilemap;
    }

    /// <summary>
    /// NOTE : 배경 생성 부모오브젝트를 생성 
    ///      : 배경이 여러개일 가능성이 높으므로 해당 배경의 수만큼 배경오브젝트를 생성하고 모든 컴포넌트 추가 및 설정
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private GameObject CreateBackGround(DungeonRoom room)
    {
        GameObject tmpParent = new GameObject("BackGroundParent");
        int count = 0;
        //배경 오브젝트 생성
        foreach (var tmptile in loadData.tileDataArray[room.roomSpriteType].tileType[0].tile)
        {
            GameObject backgroundob = new GameObject("BackGround", typeof(SpriteRenderer));
            backgroundob.transform.localPosition = Vector3.zero;
            backgroundob.transform.localRotation = Quaternion.identity;
            backgroundob.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
            backgroundob.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
            backgroundob.GetComponent<SpriteRenderer>().sprite = tmptile.sprite;
            backgroundob.GetComponent<SpriteRenderer>().size = room.roomRect.size;
            backgroundob.GetComponent<SpriteRenderer>().sortingOrder = count;
            count++;
            backgroundob.transform.SetParent(tmpParent.transform);
        }
        return tmpParent;
    }

    /// <summary>
    /// NOTE : TileMap Object 동적 생성
    /// </summary>
    /// <param name="nameoftilemap"></param>
    /// <param name="tagname"></param>
    /// <param name="layername"></param>
    /// <param name="parentob"></param>
    /// <returns></returns>
    private Tilemap CreateTileMap(string nameoftilemap, string tagname, string layername, GameObject parentob)
    {
        GameObject tmpob = new GameObject(nameoftilemap, typeof(Tilemap));
        tmpob.AddComponent<TilemapRenderer>().sortingLayerName = layername;
        tmpob.AddComponent<TilemapCollider2D>().usedByComposite = true;
        tmpob.AddComponent<CompositeCollider2D>();
        tmpob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        tmpob.layer = 8;
        tmpob.tag = tagname;
        tmpob.transform.SetParent(parentob.transform);
        Tilemap tmptilemap = tmpob.GetComponent<Tilemap>();

        return tmptilemap;
    }

    /// <summary>
    /// 몬스터 생성
    /// </summary>
    private void CreateMonster()
    {
        foreach (DungeonRoom room in roomList)
        {
            room.SetMonstersPos();
            
            foreach (SpawnMonsterInfo monsterinfo in room.monsterInfoList)
            {
                Monster tmpm = Instantiate(loadData.monsterPrefab[monsterinfo.mType.ToString()], monsterinfo.startPos, Quaternion.identity, room.roomModel.transform);
                tmpm.ownRoom = room;
                monsterinfo.monsterModel = tmpm;
                
            }
        }
    }
    #endregion
}