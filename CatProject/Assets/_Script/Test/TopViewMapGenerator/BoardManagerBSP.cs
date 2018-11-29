using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// BSP
/// </summary>
public class BoardManagerBSP : MonoBehaviour
{
    enum Dir { Set = 0, LeftBottom, Bottom, RightBottom, Left, Center, Right, LeftTop, Top, RightTop }
    enum DungeonType { Green = 0, Blue, Gray }
    enum TileType { BackGround = 0, Floor, Corridor, Wall }
    [Header("Board Size")]
    public int boardRows;
    public int boardColumns;

    [Header("Room Size")]
    [Range(2, 64)]
    public int minRoomSize;
    [Range(2, 64)]
    public int maxRoomSize;

    //0 - backGround(Water), 1 - floor, 2 - corridor, 3 - wall
    public TileData[,] map;
    [Header("Tile Reference Object")]
    public TileReferenceObject[] tileRefernce;

    private void Start()
    {
        map = new TileData[boardRows, boardColumns];

        Space rootSpace = new Space(new Rect(0, 0, boardRows, boardColumns));

        CreateBSP(rootSpace);
        rootSpace.CreateRoom();

        SetRoom(rootSpace);
        SetCorridors(rootSpace);
        SetRoomOutLine(rootSpace);
        SetBackGround(rootSpace);


        DrawMap();
    }

    /// <summary>
    /// 공간분할
    /// </summary>
    /// <param name="_Space"></param>
    public void CreateBSP(Space _Space)
    {
        Debug.Log("Splitting Space : " + _Space.spaceID + " : " + _Space.rect);

        if (!_Space.CheckLeaf())
        {
            //rect높이, 가로 길이가 maxroomsize보다 클경우, 
            if (_Space.rect.width > maxRoomSize
              || _Space.rect.height > maxRoomSize)
            {
                //Split 실행 split실행후 다시 bsp체크
                if (_Space.Split(minRoomSize))
                {
                    Debug.Log("Splitted Space : " + _Space.spaceID + " in "
                      + _Space.left.spaceID + ": " + _Space.left.rect + ", "
                      + _Space.right.spaceID + ": " + _Space.right.rect);

                    //재귀
                    CreateBSP(_Space.left);
                    CreateBSP(_Space.right);
                }
            }
        }
    }

    /// <summary>
    /// Map list에 저장된 데이터를 통하여 sprite 생성 0 : blank, 1 : floor, 2: corridor, 3: wall
    /// </summary>
    void DrawMap()
    {
        for (int i = 0; i < boardRows - 1; i++)
        {
            for (int j = 0; j < boardColumns - 1; j++)
            {
                if (map[i, j] != null)
                {
                    switch (map[i, j].tileType)
                    {
                        case 0:
                            GameObject bg = Instantiate(tileRefernce[map[i, j].roomType].backGroundSprite[map[i, j].tileDir], new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            bg.transform.SetParent(transform);
                            break;
                        case 1:
                            GameObject floor = Instantiate(tileRefernce[map[i, j].roomType].floorSprite[map[i, j].tileDir], new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            floor.transform.SetParent(transform);
                            break;
                        case 2:
                            GameObject corridor = Instantiate(tileRefernce[map[i, j].roomType].corridorSprite[map[i, j].tileDir], new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            corridor.transform.SetParent(transform);
                            break;
                        case 3:
                            GameObject wall = Instantiate(tileRefernce[map[i, j].roomType].wallSprite[map[i, j].tileDir], new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            wall.transform.SetParent(transform);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.Log("오류");
                }
            }
        }
    }

    /// <summary>
    /// 방으로 설정한 부분을 배열 map에서 타일 설정
    /// </summary>
    /// <param name="_Space"></param>
    public void SetRoom(Space _Space)
    {
        if (_Space == null)
            return;

        if (_Space.CheckLeaf())
        {
            SetRoom(_Space.left);
            SetRoom(_Space.right);
        }
        else
        {
            var roomType = _Space.roomType;
            for (int i = (int)_Space.room.x; i < _Space.room.xMax; i++)
            {
                for (int j = (int)_Space.room.y; j < _Space.room.yMax; j++)
                {
                    map[i, j] = new TileData(roomType, (int)TileType.Floor, UnityEngine.Random.Range(0, tileRefernce[roomType].floorSprite.Count));
                }
            }
        }
    }

    /// <summary>
    /// 방과 방사이를 이어주는 통로를 설정함
    /// </summary>
    /// <param name="_Space"></param>
    private void SetCorridors(Space _Space)
    {
        if (_Space == null)
            return;

        SetCorridors(_Space.left);
        SetCorridors(_Space.right);

        foreach (var cor in _Space.corridors)
        {
            var corridorType = cor.type;
            for (int i = (int)cor.rect.x; i < cor.rect.xMax; i++)
            {
                for (int j = (int)cor.rect.y; j < cor.rect.yMax; j++)
                {
                    if (map[i, j] == null)
                    {
                        map[i, j] = cor.rect.width > cor.rect.height ?
                            new TileData(corridorType, (int)TileType.Corridor, UnityEngine.Random.Range(0, 2)) :
                            new TileData(corridorType, (int)TileType.Corridor, UnityEngine.Random.Range(4, 5));
                    }
                }
            }

        }

    }

    /// <summary>
    /// 방 노드를 모두 돌면서 테두리 생성
    /// </summary>
    /// <param name="_Space"></param>
    private void SetRoomOutLine(Space _Space)
    {
        if (_Space == null)
            return;

        if (_Space.CheckLeaf())
        {
            SetRoomOutLine(_Space.left);
            SetRoomOutLine(_Space.right);
        }
        else
        {
            var x = (int)_Space.room.x;
            var y = (int)_Space.room.y;
            var xMax = (int)_Space.room.xMax;
            var yMax = (int)_Space.room.yMax;
            var wallTileNum = (int)TileType.Wall;
            var roomType = _Space.roomType;

            for (int i = x - 1; i <= xMax; i++)
            {
                for (int j = y - 1; j <= yMax; j++)
                {
                    if (map[i, j] == null)
                    {
                        if (i == x - 1 && j == y - 1)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.LeftBottom);
                        else if (i == x - 1 && j == yMax)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.LeftTop);
                        else if (i == xMax && j == y - 1)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.RightBottom);
                        else if (i == xMax && j == yMax)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.RightTop);
                        else if (i == x - 1)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.Left);
                        else if (j == y - 1)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.Bottom);
                        else if (i == xMax)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.Right);
                        else if (j == yMax)
                            map[i, j] = new TileData(roomType, wallTileNum, (int)Dir.Top);
                    }
                }
            }
        }
    }

    private void SetBackGround(Space _Space)
    {
        if (_Space == null)
            return;

        if (_Space.CheckLeaf())
        {
            SetBackGround(_Space.left);
            SetBackGround(_Space.right);
        }
        else
        {
            var roomType = _Space.roomType;
            for (int i = (int)_Space.rect.x; i < _Space.rect.xMax; i++)
            {
                for (int j = (int)_Space.rect.y; j < _Space.rect.yMax; j++)
                {
                    if (map[i, j] == null)
                        map[i, j] = new TileData(roomType, (int)TileType.BackGround, UnityEngine.Random.Range(0, tileRefernce[roomType].backGroundSprite.Count));
                }
            }
        }
    }
}

/// <summary>
/// Space클래스 
/// </summary>
public class Space
{
    //해당 객체의 설정 공간
    public Space left, right;
    public Rect rect;

    public Rect room = new Rect(-1, -1, 0, 0);
    public int roomType = -1;

    public List<Corridor> corridors = new List<Corridor>();
    private int corridorThicKness = 1;

    public int spaceID;
    private static int debugCounter = 0;

    public Space(Rect _rect)
    {
        rect = _rect;
        spaceID = debugCounter;
        debugCounter++;
    }

    /// <summary>
    /// leaf check, 없으면 true 있으면 false 
    /// </summary>
    /// <returns></returns>
    public bool CheckLeaf()
    {
        return !(left == null && right == null);
    }

    /// <summary>
    /// 파라미터 최소설정 방사이즈
    /// </summary>
    /// <param name="_minRoomLength"></param>
    /// <returns></returns>
    public bool Split(int _minRoomLength)
    {
        //leaf가 한개라도 존재하고 있는 경우 
        if (CheckLeaf())
            return false;

        // split했을 경우 최소 방사이즈보다 작을 경우 leaf를 생성할 필요가 없으므로 return
        if (Mathf.Min(rect.height, rect.width) / 2 < _minRoomLength)
        {
            Debug.Log("Space : " + spaceID + " will be a leaf");
            return false;
        }

        //SplitH 설정 (어떤 방향으로 split할것인지 체크)  세로가 길경우 true, 가로가 길경우 false
        //큰차이가 없는경우 랜덤으로 설정
        bool splitHeight;

        if (rect.height > rect.width)
            splitHeight = true;
        else if (rect.width > rect.height)
            splitHeight = false;
        else
            splitHeight = (UnityEngine.Random.Range(0, 1) > 0.5);

        //True일 경우 가로로 split False일 경우 세로로 split 
        if (splitHeight)
        {
            //int split = UnityEngine.Random.Range(_minRoomSize, (int)(rect.width - _minRoomSize));
            int split = (int)(rect.width * 0.5);
            left = new Space(new Rect(rect.x, rect.y, rect.width, split));
            right = new Space(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
        }
        else
        {
            //int split = UnityEngine.Random.Range(_minRoomSize, (int)(rect.height - _minRoomSize));
            int split = (int)(rect.height * 0.5);
            left = new Space(new Rect(rect.x, rect.y, split, rect.height));
            right = new Space(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
        }

        return true;
    }

    /// <summary>
    /// BSP로 분할한 공간에서 방사이즈 및 위치 초기화
    /// </summary>
    public void CreateRoom()
    {
        if (CheckLeaf())
        {
            //leaf 존재할경우 
            if (left != null)
                left.CreateRoom();
            if (right != null)
                right.CreateRoom();
            //통로 생성
            if (left != null && right != null)
                CreateCorridorBetween(left, right);
        }
        //leaf가 없을경우에는 그 범위안에서 room생성 
        else
        {
            roomType = UnityEngine.Random.Range(0, 10)%2;
            //방사이즈 설정 (생성한 방을 감쌀 타일을 생성하기 위해 -2)
            int roomWidth = (int)UnityEngine.Random.Range(rect.width * 0.5f, rect.width - 2);
            int roomHeight = (int)UnityEngine.Random.Range(rect.height * 0.5f, rect.height - 2);
            //방위치 설정
            int roomX = (int)UnityEngine.Random.Range(1, rect.width - roomWidth - 2);
            int roomY = (int)UnityEngine.Random.Range(1, rect.height - roomHeight - 2);
            //World 좌표로 위치 설정, 사이즈 설정
            room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
            Debug.Log("방 생성 : " + room + " in Space: " + spaceID + " " + rect);
        }
    }

    /// <summary>
    /// 생성된 방의 rect 값을 가져오는 함수 
    /// 해당 방에 leaf가 존재할 경우 leaf에서 가져옴 (양쪽에 있어도 왼쪽을 우선으로 가져옴)
    /// </summary>
    /// <returns></returns>
    public Rect GetRoom()
    {
        if (!CheckLeaf())
            return room;

        if (left != null)
        {
            var leftTmpRoom = left.GetRoom();
            if (leftTmpRoom.x != -1)
                return leftTmpRoom;
        }
        if (right != null)
        {
            Rect rightTmpRoom = right.GetRoom();
            if (rightTmpRoom.x != -1)
                return rightTmpRoom;
        }
        //아무것도 아닐경우 
        return new Rect(-1, -1, 0, 0);
    }


    /// <summary>
    /// 통로 생성
    /// </summary>
    /// <param name="_left"></param>
    /// <param name="_right"></param>
    public void CreateCorridorBetween(Space _left, Space _right)
    {
        Rect leftRoom = _left.GetRoom();
        Rect rightRoom = _right.GetRoom();


        Debug.Log("Creating corridor(s) between " + left.spaceID + "(" + leftRoom + ") and " + right.spaceID + " (" + rightRoom + ")");

        // 각각의 방에서 연결점이 될 포인트를 랜덤으로 설정
        Vector2 leftRoomPoint = new Vector2((int)UnityEngine.Random.Range(leftRoom.x + 1, leftRoom.xMax - 1)
                                          , (int)UnityEngine.Random.Range(leftRoom.y + 1, leftRoom.yMax - 1));
        Vector2 rightRoomPoint = new Vector2((int)UnityEngine.Random.Range(rightRoom.x + 1, rightRoom.xMax - 1)
                                           , (int)UnityEngine.Random.Range(rightRoom.y + 1, rightRoom.yMax - 1));


        //통로를 만드는 조건문을 줄이기 위해서
        if (leftRoomPoint.x > rightRoomPoint.x)
        {
            var tmp = leftRoomPoint;
            leftRoomPoint = rightRoomPoint;
            rightRoomPoint = tmp;
        }

        //각각 포인트 차이를 설정
        int corridorWidth = (int)(rightRoomPoint.x - leftRoomPoint.x);
        int corridorHeight = (int)(rightRoomPoint.y - leftRoomPoint.y);

        Debug.Log("LeftPoint : " + leftRoomPoint + ", RightPoint : " + rightRoomPoint + ", w : " + corridorWidth + ", h : " + corridorHeight);


        
        //타입 설정 
        int type;
        if (_left.roomType != -1)
            type = _left.roomType;
        else if (_right.roomType != -1)
            type = _right.roomType;
        else
            type = UnityEngine.Random.Range(0, 1);

        if (corridorWidth != 0)
            corridors.Add(new Corridor(type, new Rect(leftRoomPoint.x, leftRoomPoint.y, corridorWidth + 1, corridorThicKness)));
        if (corridorHeight > 0)
            corridors.Add(new Corridor(type, new Rect(rightRoomPoint.x, leftRoomPoint.y, corridorThicKness, corridorHeight)));
        else if (corridorHeight < 0)
            corridors.Add(new Corridor(type, new Rect(rightRoomPoint.x, rightRoomPoint.y, corridorThicKness, Mathf.Abs(corridorHeight))));
        //세로


        //Debug.Log("Corridors 생성!");
        //foreach (Rect cor in corridors)
        //    Debug.Log("Corridor : " + cor);
    }
}

public struct Corridor
{
    public int type;
    public Rect rect;

    public Corridor(int _type, Rect _rect)
    {
        rect = _rect;
        type = _type;
    }
}

/// <summary>
/// Map으로 저장할 tileData
/// </summary>
public class TileData
{
    public int roomType;
    public int tileType;
    public int tileDir;

    public TileData()
    {
        roomType = 0;
        tileType = 0;
        tileDir = 0;
    }

    public TileData(int _roomType, int _tileType, int _tileDir)
    {
        roomType = _roomType;
        tileType = _tileType;
        tileDir = _tileDir;
    }
}

[Serializable]
public struct TileReferenceObject
{
    public List<GameObject> backGroundSprite;
    public List<GameObject> floorSprite;
    public List<GameObject> corridorSprite;
    public List<GameObject> wallSprite;
}

