using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Board Size")]
    public int boardRows;
    public int boardColumns;

    [Header("Room Size")]
    public int minRoomSize;
    public int maxRoomSize;

    [Header("Tile Object")]
    public GameObject floorTile;
    public GameObject corridorTile;
    public List<GameObject> wallTile;

    private GameObject[,] boardPositionFloor;

    // 1 - floor, 2 - corridor, 3 - wall
    private TileData[,] map;

    private List<GameObject> roomList = new List<GameObject>();

    private void Start()
    {
        map = new TileData[boardRows, boardColumns];

        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, boardRows, boardColumns));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();

        boardPositionFloor = new GameObject[boardRows, boardColumns];
        SetCorridors(rootSubDungeon);
        SetRoom(rootSubDungeon);

        DrawMap();
    }


    void DrawMap()
    {
        for (int i = 0; i < boardRows - 1; i++)
        {
            for (int j = 0; j < boardColumns - 1; j++)
            {
                if (map[i, j] != null)
                {
                    switch (map[i, j].tileNum)
                    {
                        case 0:
                            break;
                        case 1:
                            GameObject floor = Instantiate(floorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            floor.transform.SetParent(transform);
                            break;
                        case 2:
                            GameObject corridor = Instantiate(corridorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            corridor.transform.SetParent(transform);
                            break;
                        case 3:
                            GameObject instance = Instantiate(wallTile[map[i, j].dir], new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                            instance.transform.SetParent(transform);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 공간분할 재귀
    /// </summary>
    /// <param name="_subDungeon"></param>
    public void CreateBSP(SubDungeon _subDungeon)
    {
        Debug.Log("Splitting Sub-Dungeon : " + _subDungeon.debugId + " : " + _subDungeon.rect);

        if (_subDungeon.CheckLeaf())
        {
            //rect높이, 가로 길이가 maxroomsize보다 클경우, 
            if (_subDungeon.rect.width > maxRoomSize
              || _subDungeon.rect.height > maxRoomSize
              || Random.Range(0.0f, 1.0f) > 0.25f)
            {
                //Split 실행 split실행후 다시 bsp체크
                if (_subDungeon.Split(minRoomSize))
                {
                    Debug.Log("Splitted Sub-dungeon : " + _subDungeon.debugId + " in "
                      + _subDungeon.left.debugId + ": " + _subDungeon.left.rect + ", "
                      + _subDungeon.right.debugId + ": " + _subDungeon.right.rect);

                    //재귀
                    CreateBSP(_subDungeon.left);
                    CreateBSP(_subDungeon.right);
                }
            }
        }
    }

    /// <summary>
    /// Tile생성
    /// </summary>
    /// <param name="_subDungeon"></param>
    public void SetRoom(SubDungeon _subDungeon)
    {
        if (_subDungeon == null)
            return;

        if (_subDungeon.CheckLeaf())
        {
            for (int i = (int)_subDungeon.room.x-1; i <= _subDungeon.room.xMax; i++)
            {
                for (int j = (int)_subDungeon.room.y-1; j <= _subDungeon.room.yMax; j++)
                {
                    if (i == _subDungeon.room.x-1 && j == _subDungeon.room.y-1)
                        map[i, j] = new TileData(3, 1);
                    else if (i == _subDungeon.room.x-1 && j == _subDungeon.room.yMax)
                        map[i, j] = new TileData(3, 7);
                    else if (i == _subDungeon.room.xMax && j == _subDungeon.room.y-1)
                        map[i, j] = new TileData(3, 3);
                    else if (i == _subDungeon.room.xMax && j == _subDungeon.room.yMax)
                        map[i, j] = new TileData(3, 9);
                    else if (i == _subDungeon.room.x-1) 
                        map[i, j] = new TileData(3, 4);
                    else if (j == _subDungeon.room.y-1) 
                        map[i, j] = new TileData(3, 2);
                    else if (i == _subDungeon.room.xMax)
                        map[i, j] = new TileData(3, 6);
                    else if (j == _subDungeon.room.yMax)
                        map[i, j] = new TileData(3, 8);
                    else
                        map[i, j] = new TileData(1, 0);
                }
            }
        }
        else
        {
            SetRoom(_subDungeon.left);
            SetRoom(_subDungeon.right);
        }
    }

    /// <summary>
    /// 통로 그리는 함수
    /// </summary>
    /// <param name="_subDungeon"></param>
    private void SetCorridors(SubDungeon _subDungeon)
    {
        if (_subDungeon == null)
            return;

        SetCorridors(_subDungeon.left);
        SetCorridors(_subDungeon.right);

        foreach (Rect cor in _subDungeon.corridors)
        {
            for (int i = (int)cor.x; i < cor.xMax; i++)
            {
                for (int j = (int)cor.y; j < cor.yMax; j++)
                {
                        //GameObject instance = Instantiate(corridorTile, new Vector3(i, j, 0f), Quaternion.identity);
                        //instance.transform.SetParent(transform);
                        //boardPositionFloor[i, j] = instance;
                        
                        map[i, j] = new TileData(2, 0);
                }
            }
        }
    }
}

/// <summary>
/// 던전클래스
/// </summary>
public class SubDungeon
{
    //해당 객체의 설정 공간
    public SubDungeon left, right;
    public Rect rect;

    public Rect room = new Rect(-1, -1, 0, 0);
    //복도
    public List<Rect> corridors = new List<Rect>();

    public int debugId;
    private static int debugCounter = 0;

    public SubDungeon(Rect _rect)
    {
        rect = _rect;
        debugId = debugCounter;
        debugCounter++;
    }

    /// <summary>
    /// leaf check, 없으면 true 있으면 false 
    /// </summary>
    /// <returns></returns>
    public bool CheckLeaf()
    {
        return left == null && right == null;
    }

    /// <summary>
    /// 파라미터 최소설정 방사이즈
    /// </summary>
    /// <param name="_minRoomSize"></param>
    /// <returns></returns>
    public bool Split(int _minRoomSize)
    {
        //leaf가 이미 존재하고 있는 경우 
        if (!CheckLeaf())
            return false;

        // split했을 경우 최소 방사이즈보다 작을 경우 leaf를 생성할 필요가 없으므로 return
        if (Mathf.Min(rect.height, rect.width) / 2 < _minRoomSize)
        {
            Debug.Log("Sub-Dungeon : " + debugId + " will be a leaf");
            return false;
        }

        //SplitH 설정 (어떤 방향으로 split할것인지 체크)  세로가 길경우 true, 가로가 길경우 false
        //큰차이가 없는경우 랜덤으로 설정
        bool splitHeight;

        if (rect.height / rect.width >= 1.25)
            splitHeight = true;
        else if (rect.width / rect.height >= 1.25)
            splitHeight = false;
        else
            splitHeight = Random.Range(0.0f, 1.0f) > 0.5f;

        //True일 경우 가로로 split False일 경우 세로로 split 
        if (splitHeight)
        {
            int split = Random.Range(_minRoomSize, (int)(rect.width - _minRoomSize));

            left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
            right = new SubDungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
        }
        else
        {
            int split = Random.Range(_minRoomSize, (int)(rect.height - _minRoomSize));

            left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
            right = new SubDungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
        }

        return true;
    }

    /// <summary>
    /// BSP로 분할한 공간에서 방사이즈 및 위치 초기화
    /// </summary>
    public void CreateRoom()
    {
        //leaf 존재할경우 
        if (left != null)
            left.CreateRoom();
        if (right != null)
            right.CreateRoom();
        //통로 생성
        if (left != null && right != null)
            CreateCorridorBetween(left, right);

        //leaf가 없을경우에는 그 범위안에서 room생성 
        if (CheckLeaf())
        {
            //방사이즈 설정 (생성한 방을 감쌀 타일을 생성하기 위해 -2)
            int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
            int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2);
            //방위치 설정
            int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
            int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);
            //World 좌표로 위치 설정, 사이즈 설정
            room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
            Debug.Log("Create Room : " + room + " in sub-dungeon : " + debugId + " " + rect);
        }
    }

    /// <summary>
    /// 생성된 방의 rect 값을 가져오는 함수 
    /// </summary>
    /// <returns></returns>
    public Rect GetRoom()
    {
        if (CheckLeaf())
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
    public void CreateCorridorBetween(SubDungeon _left, SubDungeon _right)
    {
        Rect leftRoom = _left.GetRoom();
        Rect rightRoom = _right.GetRoom();

        Debug.Log("Creating corridor(s) between " + left.debugId + "(" + leftRoom + ") and " + right.debugId + " (" + rightRoom + ")");

        // 각각의 방에서 연결점이 될 포인트를 랜덤으로 설정
        Vector2 leftRoomPoint = new Vector2((int)Random.Range(leftRoom.x + 1, leftRoom.xMax - 1)
                                      , (int)Random.Range(leftRoom.y + 1, leftRoom.yMax - 1));
        Vector2 rightRoomPoint = new Vector2((int)Random.Range(rightRoom.x + 1, rightRoom.xMax - 1)
                                       , (int)Random.Range(rightRoom.y + 1, rightRoom.yMax - 1));
        
        //각각 포인트 차이를 설정
        int corridorWidth = (int)(rightRoomPoint.x - leftRoomPoint.x);
        int corridorHeight = (int)(rightRoomPoint.y - leftRoomPoint.y);

        Debug.Log("LeftPoint : " + leftRoomPoint + ", RightPoint : " + rightRoomPoint + ", w : " + corridorWidth + ", h : " + corridorHeight);

        //가로 길 
        if (corridorWidth >= 0)
            corridors.Add(new Rect(leftRoomPoint.x, leftRoomPoint.y, corridorWidth, 1));
        else
            corridors.Add(new Rect(rightRoomPoint.x, rightRoomPoint.y, Mathf.Abs(corridorWidth), 1));
        //세로
        if (corridorHeight >= 0)
            corridors.Add(new Rect(rightRoomPoint.x, leftRoomPoint.y, 1, corridorHeight));
        else
            corridors.Add(new Rect(rightRoomPoint.x, rightRoomPoint.y, 1, Mathf.Abs(corridorHeight)));

        Debug.Log("Corridors !");
        foreach (Rect cor in corridors)
            Debug.Log("Corridor : " + cor);
    }
}

class TileData
{
    public int tileNum;
    public int dir;

    public TileData(int num, int d)
    {
        tileNum = num;
        dir = d;
    }
}

