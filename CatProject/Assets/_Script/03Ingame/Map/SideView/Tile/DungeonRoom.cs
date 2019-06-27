using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Room_ClearType { None = 0, Battle, Puzzle, Boss }

/// <summary>
/// NOTE : DungeonRoom 클래스
/// TODO : 함수와 클래스 변수들을 분리해야하는 개선 사항 가능성
/// </summary>
public class DungeonRoom
{
    //object
    public GameObject roomModel = null;
    //Room info
    public TileInfo[,] roomTileArray;
    public Rect roomRect = new Rect(0, 0, 0, 0);
    public int roomNumberOfList = -1;
    public int roomSpriteType = -1;
    public Room_ClearType roomClearType = Room_ClearType.None;
    public int level = -1;

    public List<EntranceConnectRoom> entranceInfoList = new List<EntranceConnectRoom>();
    public List<SpawnMonsterInfo> monsterInfoList = new List<SpawnMonsterInfo>();
    public List<SpawnStructureInfo> desStructureInfoList = new List<SpawnStructureInfo>();
    public List<SpawnBossInfo> bossInfoList = new List<SpawnBossInfo>();
    public List<SpawnItemInfo> itemInfoList = new List<SpawnItemInfo>();
    public List<SpawnSwitchInfo> SwitchInfoList = new List<SpawnSwitchInfo>();
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

        roomTileArray = new TileInfo[(int)roomRect.width, (int)roomRect.height];
    }

    public DungeonRoom(int _roomType, int _width, int _height)
    {
        roomSpriteType = _roomType;
        roomRect = new Rect(0, 0, _width, _height);
        roomTileArray = new TileInfo[(int)roomRect.width, (int)roomRect.height];
    }

    #region Set Monster and Entrance
    /// <summary>
    /// NOTE : 저장된 NeighborRooms 정보를 통해 출입구 랜덤 생성 
    /// TODO : 현재는 가로값을 랜덤으로 설정하고 높이는 무조건 땅위에 생성하도록 설정하여 개선 가능성이 매우 높음
    /// </summary>
    /// 

    public void SetEntrancePos()
    {

        int entranceCount = entranceInfoList.Count;
        List<int> posXisalready = new List<int>();
        //이미 그려져있는 출입문 체크
        for (int x = 0; x < roomRect.xMax; x++)
        {
            for (int y = 0; y < roomRect.yMax; y++)
            {
                if (roomTileArray[x, y] != null)
                {
                    if (roomTileArray[x, y].tileType.Equals(TileType.Structure))
                    {
                        posXisalready.Add(x);
                        entranceCount--;
                    }
                }
            }
        }
        //만약 랜덤설정할 필요가 없을경우 패스
        if (entranceCount < 0)
        {
            return;
        }
        switch (roomClearType)
        {
            case Room_ClearType.None:
            case Room_ClearType.Boss:
                for (int j = 0; j < roomRect.yMax; j++)
                {
                    if (roomTileArray[5, j] == null)
                    {
                        roomTileArray[5, j] = new TileInfo(TileType.Structure);
                        break;
                    }
                }
                break;
            default:
                //int distanceOtherObject = 3;
                //List<int> posX = new List<int>();
                ////Entrance갯수 만큼 x포지션 저장
                //for (int i = 0; i < entranceCount; i++)
                //{
                //    var randomXvalue = (int)Random.Range(2, roomRect.xMax - 2);

                //    //같은 값이 있는지 체크?
                //    for (int j = 0; j < posX.Count; j++)
                //    {
                //        if (posX[j] - distanceOtherObject >= randomXvalue && posX[j] + distanceOtherObject <= randomXvalue)
                //        {
                //                foreach (var tmpx in posXisalready)
                //                {
                //                    if (tmpx - distanceOtherObject >= randomXvalue && tmpx + distanceOtherObject <= randomXvalue)
                //                    {
                //                        randomXvalue = (int)Random.Range(2, roomRect.xMax - 2);
                //                        //다시 처음부터 확인하기 위함
                //                        j = 0;
                //                    }
                //                }
                //        }
                //    }
                //    posX.Add(randomXvalue);
                //}
                ////저장한 x포지션을 기준으로 y값을 순회하여 roomarray의 0 값을 검색하여 설정
                //foreach (int tmpx in posX)
                //{
                //    var tx = tmpx;
                //    for (int j = 0; j < roomRect.yMax; j++)
                //    {
                //        if (roomTileArray[tmpx, j] == null)
                //        {
                //            if (roomTileArray[tmpx + 1, j].Equals(TileType.Switch))
                //                tx = tmpx + 1;
                //            else if(roomTileArray[tmpx-1,j].Equals(TileType.Switch))
                //            roomTileArray[tx, j] = new TileInfo(TileType.Entrance, 0);
                //            break;
                //        }
                //    }
                //}
                for (int i = 0; i < entranceCount; i++)
                {
                    var rx = (int)Random.Range(2, roomRect.xMax - 2);
                    for (int y = 0; y < roomRect.yMax; y++)
                    {
                        if (roomTileArray[rx, y] == null)
                        {
                            if (y != 0)
                            {
                                //아래가 지형일 경우에만 
                                if (roomTileArray[rx, y - 1].tileType.Equals(TileType.Terrain))
                                {
                                    //양옆에 아무것도 없을때만
                                    if (roomTileArray[rx - 1, y] == null && roomTileArray[rx + 1, y] == null)
                                    {
                                        roomTileArray[rx, y] = new TileInfo(TileType.Structure);
                                        break;
                                    }
                                    //리셋 
                                    else
                                    {
                                        i--;
                                        break;
                                    }

                                }
                                //리셋 
                                else
                                {
                                    i--;
                                    break;
                                }
                            }
                            //y값이 0일 때는
                            else
                            {
                                if (roomTileArray[rx - 1, y] == null && roomTileArray[rx + 1, y] == null)
                                {
                                    roomTileArray[rx, y] = new TileInfo(TileType.Structure);
                                    break;
                                }
                                //리셋 
                                else
                                {
                                    i--;
                                    break;
                                }

                            }
                        }
                    }
                }
                break;
        }

    }

    /// <summary>
    /// NOTE : NormalPosition 설정
    /// </summary>
    /// <param name="_obnumber"></param>
    /// <returns></returns>
    private List<Vector2> NormalPosSet(int _obnumber)
    {
        List<Vector2> pos = new List<Vector2>();
        List<int> posX = new List<int>();

        for (int i = 0; i < _obnumber; i++)
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

        Debug.Log(_obnumber + " / " + posX.Count);
        //저장한 x포지션을 기준으로 y값을 순회하여 roomarray의 0 값을 검색하여 설정 
        foreach (int tmpx in posX)
        {
            for (int j = 1; j < roomRect.yMax - 1; j++)
            {
                if (roomTileArray[tmpx, j] == null)
                {
                    pos.Add(new Vector2(tmpx, j + 1));
                    break;
                }
            }
        }
        return pos;
    }

    /// <summary>
    /// NOTE : 오브젝트 포지션
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_data"></param>
    public void SetPrefabInfoList<T>(T _data)
    {
        var type = _data.GetType();

        //갯수 설정 Level에 따라 다름
        int obnumber = level; //(int)(level * 0.5f) + 1;
        List<Vector2> pos = new List<Vector2>();
        pos = NormalPosSet(obnumber);

        if (type.Equals(monsterInfoList.GetType()))
        {
            foreach (var p in pos)
                monsterInfoList.Add(new SpawnMonsterInfo(MONSTER_TYPE.Fox, p));
        }
        else if (type.Equals(desStructureInfoList.GetType()))
        {
            foreach (var p in pos)
                desStructureInfoList.Add(new SpawnStructureInfo(Structure_Type.Box, p));
        }
        else if (type.Equals(itemInfoList.GetType()))
        {
            foreach (var p in pos)
                itemInfoList.Add(new SpawnItemInfo(Item_TYPE.Catnip, p));
        }
    }

    /// <summary>
    /// NOTE : 보스 포지션 설정
    /// </summary>
    public void SetBossPos()
    {
        Vector2 startpos = Vector2.zero;
        int xpos = (int)roomRect.width - 7;
        for (int j = 0; j < roomRect.height - 1; j++)
        {
            if (roomTileArray[xpos, j] == null)
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
        if (CheckMonsterAlive() && CheckPuzzleClear() && GetItemCheck() && CheckSwitchClear())
            UnLockEntrances();
        else
            LockEntrance();
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
    private bool CheckPuzzleClear()
    {
        bool checkcomplete = true;
        if (SwitchInfoList.Count > 0)
            return checkcomplete;

        if (desStructureInfoList.Count > 0)
        {
            foreach (var dsinfo in desStructureInfoList)
            {
                if (dsinfo.structureModel.isAlive)
                    checkcomplete = false;
            }
        }

        return checkcomplete;
    }

    /// <summary>
    /// NOTE : Switch Check
    /// </summary>
    /// <returns></returns>
    private bool CheckSwitchClear()
    {
        bool checkcomplete = true;

        if (SwitchInfoList.Count > 0)
        {
            foreach (var swinfo in SwitchInfoList)
            {
                if (!swinfo.switchModel.SwitchOn)
                    checkcomplete = false;
            }
        }

        return checkcomplete;
    }

    /// <summary>
    /// 보스 관련 체크 
    /// </summary>
    public void BossClearCheck()
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

        if (checkcomplete)
            InGameManager.instance.CheckStageClear();

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

    private void LockEntrance()
    {
        foreach (var entranceinfo in entranceInfoList)
            entranceinfo.entrance.LockEntracne();
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
            if (monsterinfo.monsterModel.isActiveAndEnabled)
                monsterinfo.monsterModel.StopAction(_stopcount);
        }
    }

    /// <summary>
    /// NOTE : 아이템 trigger 처리 기능 정지
    /// </summary>
    /// <param name="_stopcount"></param>
    public void ItemStop(float _stopcount)
    {
        foreach (var iteminfo in itemInfoList)
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

    public SpawnMonsterInfo(MONSTER_TYPE _mtype, Vector2 _startpos, Monster ob)
    {
        mType = _mtype;
        startPos = _startpos;
        monsterModel = ob;
    }
}

/// <summary>
/// NOTE : DungeonRoom 클래스에서 설정하는 파괴되는구조물 정보 
/// </summary>
public class SpawnStructureInfo
{
    public Structure_Type sType;
    public Vector2 startPos;
    public StructureObject structureModel;

    public SpawnStructureInfo(Structure_Type _stype, Vector2 _startpos)
    {
        sType = _stype;
        startPos = _startpos;
        structureModel = null;
    }

    public SpawnStructureInfo(Structure_Type _stype, Vector2 _startpos, StructureObject ob)
    {
        sType = _stype;
        startPos = _startpos;
        structureModel = ob;
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
/// NOTE : 퍼즐형 스위치 클래스
/// </summary>
public class SpawnSwitchInfo
{
    public Vector2 startpos;
    public SwitchObSc switchModel;

    public SpawnSwitchInfo(Vector2 _startpos, SwitchObSc ob)
    {
        startpos = _startpos;
        switchModel = ob;
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

    public EntranceConnectRoom(DungeonRoom _room, Vector2 _startpos, EntranceSc ob)
    {
        connectedRoom = _room;
        startPos = _startpos;
        entrance = ob;
    }
}

/// <summary>
/// NOTE : DungeonRoom내부구조물을 한번에 적어서 설정하기위해 클래스 배열로 생성
/// </summary>
public class TileInfo
{
    public TileType tileType;
    public string tileName;


    public TileInfo(TileType _tiletype, string _tilename)
    {
        tileType = _tiletype;
        tileName = _tilename;
    }

    public TileInfo(TileType _tiletype)
    {
        tileType = _tiletype;
        tileName = null;
    }
}
