using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Room_ClearType {None = 0, Battle , Puzzle, Boss }

public enum MONSTER_TYPE { Fox = 0, Dog = 1};
public enum DesStructure_TYPE { Frog = 0 };
public enum Item_TYPE { Catnip = 0 };
/// <summary>
/// NOTE : DungeonRoom 클래스
/// TODO : 함수와 클래스 변수들을 분리해야하는 개선 사항 가능성
/// </summary>
public class DungeonRoom
{
    //object
    public GameObject roomModel = null;
    //Room info
    public TileInfo[,] roomGroundArray;
    public Rect roomRect = new Rect(0, 0, 0, 0);
    public int roomNumberOfList = -1;
    public int roomSpriteType = -1;
    public Room_ClearType roomClearType = Room_ClearType.None;
    public int level = -1;
    public bool clearCheck = false;

    public List<EntranceConnectRoom> entranceInfoList = new List<EntranceConnectRoom>();
    public List<SpawnMonsterInfo> monsterInfoList = new List<SpawnMonsterInfo>();
    public List<SpawnDesStructureInfo> desStructureInfoList = new List<SpawnDesStructureInfo>();
    public List<SpawnBossInfo> bossInfoList = new List<SpawnBossInfo>();
    public List<SpawnItemInfo> itemInfoList = new List<SpawnItemInfo>();
    //Terrain
    public GeneratedTerrainData beforeTerrainData = null;
    public int currentXPos;
    

    /// <summary>
    /// NOTE : 방의 사이즈 랜덤 설정 생성자
    /// </summary>
    /// <param name="_roomNumber"></param>
    /// <param name="_roomSpriteType"></param>
    /// <param name="_widthMin"></param>
    /// <param name="_widthMax"></param>
    /// <param name="_heightMin"></param>
    /// <param name="_heightMax"></param>
    public DungeonRoom(int _roomNumber, int _roomSpriteType, int _widthMin, int _widthMax, int _heightMin, int _heightMax)
    {
        roomNumberOfList = _roomNumber;
        roomSpriteType = _roomSpriteType;

        var tmpW = Random.Range(_widthMin, _widthMax);
        var tmpH = Random.Range(_heightMin, _heightMax);
        roomRect = new Rect(0, 0, tmpW, tmpH);
        
        roomGroundArray = new TileInfo[(int)roomRect.width, (int)roomRect.height];
    }

    public DungeonRoom(int _roomType, int _width, int _height)
    {
        roomSpriteType = _roomType;
        roomRect = new Rect(0, 0, _width, _height);
        roomGroundArray = new TileInfo[(int)roomRect.width, (int)roomRect.height];
    }

    #region Set Monster and Entrance
    /// <summary>
    /// NOTE : 저장된 NeighborRooms 정보를 통해 출입구 랜덤 생성 
    /// TODO : 현재는 가로값을 랜덤으로 설정하고 높이는 무조건 땅위에 생성하도록 설정하여 개선 가능성이 매우 높음
    /// </summary>
    /// 
    int distanceOtherObject = 3;
    public void SetEntrancePos()
    {
        switch(roomClearType)
        {
            case Room_ClearType.None:
            case Room_ClearType.Boss:
                for (int j = 0; j < roomRect.yMax; j++)
                {
                    if (roomGroundArray[5, j] == null)
                    {
                        roomGroundArray[5, j] = new TileInfo(TileType.Entrance, 0);
                        break;
                    }
                }
                break;
            default:
                List<int> posX = new List<int>();
                //Entrance갯수 만큼 x포지션 저장
                for (int i = 0; i < entranceInfoList.Count; i++)
                {
                    var randomXvalue = (int)Random.Range(2, roomRect.xMax - 2);

                    //같은 값이 있는지 체크?
                    for (int j = 0; j < posX.Count; j++)
                    {
                        if (posX[j]-distanceOtherObject>randomXvalue&&posX[j]+distanceOtherObject<randomXvalue)
                        {
                            randomXvalue = (int)Random.Range(2, roomRect.xMax - 2);
                            //다시 처음부터 확인하기 위함
                            j = 0;
                        }
                    }
                    posX.Add(randomXvalue);
                }
                //저장한 x포지션을 기준으로 y값을 순회하여 roomarray의 0 값을 검색하여 설정
                foreach (int tmpx in posX)
                {
                    for (int j = 0; j < roomRect.yMax; j++)
                    {
                        if (roomGroundArray[tmpx, j] == null)
                        {
                            roomGroundArray[tmpx, j] = new TileInfo(TileType.Entrance, 0);
                            break;
                        }
                    }
                }
                break;
        }
       
    }

    /// <summary>
    /// NOTE : 몬스터 생성 위치 설정
    /// </summary>
    public void SetMonstersPos()
    {
        //각 방의 몬스터 숫자설정
        //..level에따른 설정 
        //Test를 위해 1로만 설정
        //현재는 레벨만큼 몬스터 생성
        int numberofmonster = level;

        //몬스터 숫자만큼 x포지션 저장
        List<int> posX = new List<int>();
        for (int i = 0; i < numberofmonster; i++)
        {
            var tmpvalue = (int)Random.Range(1, roomRect.xMax - 1);

            
            //같은 값이 있는지 체크?
            for (int j = 0; j < posX.Count; j++)
            {

                if (posX[j].Equals(tmpvalue))
                {
                    tmpvalue = (int)Random.Range(1, roomRect.xMax - 1);
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
                if (roomGroundArray[tmpx, j] == null)
                {
                    monsterInfoList.Add(new SpawnMonsterInfo(MONSTER_TYPE.Fox, new Vector2(tmpx, j + 0.5f)));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// NOTE : 아이템 숫자 및 포지션 설정
    /// </summary>
    public void SetItemPos()
    {
        List<int> posX = new List<int>();
        int itemnumber = Random.Range(1, 10);

        for(int i = 0; i< itemnumber; i++)
        {
            var tmpvalue = (int)Random.Range(1, roomRect.xMax - 1);
            for(int j=0; j< posX.Count; j++)
            {
                if(posX[j].Equals(tmpvalue))
                {
                    //random값을 다시 설정하고 j를 0으로 바꿈으로써 다시 체크
                    tmpvalue = (int)Random.Range(1, roomRect.xMax - 1);
                    j = 0;
                }
            }
            posX.Add(tmpvalue);
        }
        //저장한 x포지션을 기준으로 y값을 순회하여 roomarray의 0 값을 검색하여 설정
        foreach (int tmpx in posX)
        {
            for (int j = 0; j < roomRect.yMax - 1; j++)
            {
                if (roomGroundArray[tmpx, j] == null)
                {
                    itemInfoList.Add(new SpawnItemInfo(Item_TYPE.Catnip, new Vector2(tmpx, j+1)));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// NOTE : 파괴되는 구조물의 위치 설정
    /// </summary>
    public void SetDesStructurePos()
    {
        //각 방의 몬스터 숫자설정
        //..level에따른 설정 
        //Test를 위해 1로만 설정
        //현재는 레벨만큼 몬스터 생성
        int number = level;

        //몬스터 숫자만큼 x포지션 저장
        List<int> posX = new List<int>();
        for (int i = 0; i < number; i++)
        {
            var tmpvalue = (int)Random.Range(2, roomRect.xMax - 2);

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
                if (roomGroundArray[tmpx, j] == null)
                {
                    desStructureInfoList.Add(new SpawnDesStructureInfo(DesStructure_TYPE.Frog, new Vector2(tmpx, j + 1f)));
                    break;
                }
            }
        }

    }

    /// <summary>
    /// NOTE : 보스 포지션 설정
    /// </summary>
    public void SetBossPos()
    {
        Vector2 startpos = Vector2.zero; 
        int xpos = (int)roomRect.width - 7;
        for(int j=0; j<roomRect.height-1;j++)
        {
            if (roomGroundArray[xpos, j] == null)
                startpos = new Vector2(xpos, j);
        }
        bossInfoList.Add(new SpawnBossInfo(MONSTER_TYPE.Dog, startpos));
    }
    #endregion

    #region Room Complete Check

    /// <summary>
    /// NOTE : 방 체크
    /// </summary>
    public void CheckLockRoom()
    {
        // 몬스터 , 보스 , 아이템 체크 후 출입구 개방
        if (CheckMonsterAlive() && PuzzleClearCheck() && BossAliveCheck() && GetItemCheck())
        {
            UnLockEntrances();
            clearCheck = true;
            InGameManager.instance.CheckAllStageClear();
        }
    }
    /// <summary>
    /// NOTE : ROOM CHECK UN LOCK
    /// TODO : 현재는 몬스터의 존재 유무로만 LOCK 해제, 이후에 추가적으로 방의 타입에 따라 룸을 해제하는 방식을 변경 해야한다.
    /// </summary>
    private bool CheckMonsterAlive()
    {
        bool checkcomplete = true;
        if (monsterInfoList.Count > 0)
        {
            foreach (var monsterinfo in monsterInfoList)
            {
                if (monsterinfo.monsterModel.isAlive)
                    checkcomplete = false;
            }
        }
        return checkcomplete;
    }
    
    /// <summary>
    /// NOTE : 퍼즐관련 체크 현재는 부서지는 구조물을 부셨느냐 만 체크함
    /// </summary>
    /// <returns></returns>
    private bool PuzzleClearCheck()
    {
        bool checkcomplete = true;

        if(desStructureInfoList.Count>0)
        {
            foreach(var dsinfo in desStructureInfoList)
            {
                if (dsinfo.desStructureModel.isAlive)
                    checkcomplete = false;
            }
        }

        return checkcomplete;
    }

    /// <summary>
    /// 보스 관련 체크 
    /// </summary>
    private bool BossAliveCheck()
    {
        bool checkcomplete = true;

        if (bossInfoList.Count > 0)
        {
            foreach (var bossinfo in bossInfoList)
            {
                if (bossinfo.monsterModel.isAlive)
                    checkcomplete = false;
            }
        }
        //..
        return checkcomplete;
    }

    /// <summary>
    /// 
    /// </summary>
    private bool GetItemCheck()
    {
        bool checkcomplete = true;
        //..
        return checkcomplete;
    }
    
    /// <summary>
    /// NOTE : 방의 체크리스트를 모두 확인한 후 조건을 모두 통과할 경우 통로 오픈
    /// </summary>
    private void UnLockEntrances()
    {
        foreach (var entranceinfo in entranceInfoList)
            entranceinfo.entrance.UnLockEntrance();
        Debug.Log("UNLOCK ROOM [" + roomNumberOfList + "]");
    }
    #endregion

    /// <summary>
    /// NOTE : 방이 변경되었을 때 파라미터 값만큼 몬스터 이동 정지
    /// </summary>
    /// <param name="_stopCount"></param>
    public void MonsterStop(float _stopcount)
    {
        foreach (var monsterinfo in monsterInfoList)
        {
            if(monsterinfo.monsterModel.isActiveAndEnabled)
            monsterinfo.monsterModel.StopAction(_stopcount);
        }
    }
    
    /// <summary>
    /// NOTE : 아이템 trigger 처리 기능 정지
    /// </summary>
    /// <param name="_stopcount"></param>
    public void ItemStop(float _stopcount)
    {
        foreach(var iteminfo in itemInfoList)
        {
            if (iteminfo.itemModel.isActiveAndEnabled)
                iteminfo.itemModel.StopAction(_stopcount);
        }
    }
}


/// <summary>
/// NOTE : DungeonRoom 클래스에서 설정하는 몬스터 정보 (구조체로 선언하려다 foreach문에서 멤버변수 초기화가 되지 않아 클래스로 변경)
/// </summary>
public class SpawnMonsterInfo
{
    public MONSTER_TYPE mType;
    public Vector2 startPos;
    public Monster monsterModel;

    public SpawnMonsterInfo(MONSTER_TYPE _mtype, Vector2 _startpos)
    {
        mType = _mtype;
        startPos = _startpos;
        monsterModel = null;
    }
}

/// <summary>
/// NOTE : DungeonRoom 클래스에서 설정하는 파괴되는구조물 정보 
/// </summary>
public class SpawnDesStructureInfo
{
    public DesStructure_TYPE dsType;
    public Vector2 startPos;
    public DesStructure desStructureModel;

    public SpawnDesStructureInfo(DesStructure_TYPE _dstype, Vector2 _startpos)
    {
        dsType = _dstype;
        startPos = _startpos;
        desStructureModel = null;
    }   
}

/// <summary>
/// NOTE : BOSS INFO 클래스
/// </summary>
public class SpawnBossInfo
{
    public MONSTER_TYPE mType;
    public Vector2 startPos;
    public Monster monsterModel;

    public SpawnBossInfo(MONSTER_TYPE _mtype, Vector2 _startpos)
    {
        mType = _mtype;
        startPos = _startpos;
        monsterModel = null;
    }
}

/// <summary>
/// NOTE : 아이템 INFO 클래스
/// </summary>
public class SpawnItemInfo
{
    public Item_TYPE iType;
    public Vector2 startpos;
    public ItemSc itemModel;
    
    public SpawnItemInfo(Item_TYPE _itype, Vector2 _startpos)
    {
        iType = _itype;
        startpos = _startpos;
        itemModel = null;
    }
}

/// <summary>
/// NOTE : 출입구 클래스 연결된 방과 해당 오브젝트 (struct으로 구현하였다가 foreach문에서 반복 변수 초기화가 불가하여 class로 변경(구조체 : 값복사, 클래스 : 참조복사)
/// </summary>
public class EntranceConnectRoom
{
    public Vector2 startPos;
    public DungeonRoom connectedRoom;
    public EntranceSc entrance;

    public EntranceConnectRoom(DungeonRoom _room)
    {
        connectedRoom = _room;
        startPos = Vector2.zero;
        entrance = null;
    }
}

/// <summary>
/// NOTE : DungeonRoom내부구조물을 한번에 적어서 설정하기위해 클래스 배열로 생성
/// </summary>
public class TileInfo
{
    public TileType tileType;
    public int tileNumber;

    public TileInfo(TileType _tiletype, int _tilenumber)
    {
        tileType = _tiletype;
        tileNumber = _tilenumber;
    }

    public TileInfo(TileType _tiletype)
    {
        tileType = _tiletype;
        tileNumber = 0;
    }
}
