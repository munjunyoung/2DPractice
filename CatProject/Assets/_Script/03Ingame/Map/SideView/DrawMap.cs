using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/// <summary>
/// NOTE : Draw Map 클래스 
/// </summary>
public class DrawMap : MonoBehaviour
{
    private static DrawMap _instance = null;
    public static DrawMap instance
    {
        get
        {
            if (_instance == null)
                _instance = new DrawMap();

            return _instance;
        }
    }

    private List<Tilemap> tilemapOb;
    private LoadDataManager loadData;
    public GameObject gridOb;


    private void Start()
    {
        loadData = LoadDataManager.instance;
        FindTilemap();
    }

    public DrawMap()
    {
        loadData = LoadDataManager.instance;
        //FindTilemap();
    }

    /// <summary>
    /// NOTE : 타일맵 검색
    /// </summary>
    private void FindTilemap()
    {
        var tmpgrid = GameObject.FindWithTag("DrawGrid");
        List<DungeonRoom> roomlist = new List<DungeonRoom>();
        Tilemap[] tilemaps = tmpgrid.transform.GetComponentsInChildren<Tilemap>();

        foreach (var tm in tilemaps)
        {
            DungeonRoom tmproom = new DungeonRoom(1, tm.size.x, tm.size.y);
            tmproom.roomTileArray = loadData.AnalyzeTileMap(tm);
            roomlist.Add(tmproom);
        }

        gridOb = new GameObject("Rooms", typeof(Grid));
        gridOb.GetComponent<Grid>().cellGap = new Vector3(-0.001f, -0.001f, 0);

        foreach (var room in roomlist)
            DrawTilemap(room, false).transform.SetParent(gridOb.transform);
        roomlist[0].roomModel.SetActive(true);
        tmpgrid.SetActive(false);
    }

    /// <summary>
    /// NOTE : Tileinfo를 통하여 Draw
    /// </summary>
    /// <param name="_rooms"></param>
    public GameObject DrawTilemap(DungeonRoom room, bool ingame)
    {
        //Tilemap 생성 및 초기화
        var tmpob = new GameObject("Room" + room.roomNumberOfList, typeof(Tilemap), typeof(CompositeCollider2D));
        tmpob.AddComponent<TilemapCollider2D>().usedByComposite = true;
        tmpob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        tmpob.AddComponent<TilemapRenderer>().sortingLayerName = "Ground";
        tmpob.tag = "Ground";
        tmpob.layer = 8;
        Tilemap tmptilemap = tmpob.GetComponent<Tilemap>();

        //내부 생성
        for (int x = 0; x < room.roomRect.xMax; x++)
        {
            for (int y = 0; y < room.roomRect.yMax; y++)
            {
                if (room.roomTileArray[x, y] != null)
                {
                    TileInfo tmpdata = room.roomTileArray[x, y];
                    switch (tmpdata.tileType)
                    {
                        case TileType.Terrain:
                            tmptilemap.SetTile(new Vector3Int(x, y, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                            break;
                        case TileType.Structure:
                            StructureObject tmpen = Instantiate(loadData.structurePrefabDic[tmpdata.tileName], new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, tmptilemap.transform);
                            tmpen.ownRoom = room;
                            if (tmpdata.tileName == STRUCTURE_TYPE.Entrance.ToString())
                            {
                                tmpen.transform.localPosition += new Vector3(0, 0.5f ,0);
                                //Maptool에서 사용할경우 대비
                                if (room.entranceInfoList.Count == 0)
                                    room.entranceInfoList.Add(new EntranceConnectRoom(null, tmpen.GetComponent<EntranceSc>()));

                                tmpen.ownSpRenderer.sprite = loadData.tileDataArray[room.roomSpriteType].entranceSprite[0];
                                tmpen.ownSpRenderer.sortingLayerName = "Entrance";
                                tmpen.GetComponent<EntranceSc>().spriteArray[0] = loadData.tileDataArray[room.roomSpriteType].entranceSprite[0];
                                tmpen.GetComponent<EntranceSc>().spriteArray[1] = loadData.tileDataArray[room.roomSpriteType].entranceSprite[1];
                                tmpen.transform.SetParent(tmpob.transform);
                                foreach (EntranceConnectRoom nroom in room.entranceInfoList)
                                {
                                    if (nroom.entrance == null)
                                    {
                                        tmpen.GetComponent<EntranceSc>().ownRoom = room;
                                        nroom.entrance = tmpen.GetComponent<EntranceSc>();
                                        break;
                                    }
                                }
                            }
                            else if(tmpdata.tileName == STRUCTURE_TYPE.Box.ToString())
                            {
                                room.boxInfoList.Add(new SpawnBoxInfo(tmpdata.tileNamesType.ToString(), tmpen));
                            }
                            else if(tmpdata.tileName == STRUCTURE_TYPE.Switch.ToString())
                            {
                                room.switchInfoList.Add(new SpawnSwitchInfo(tmpdata.tileNamesType ,tmpen));
                            }
                            break;
                        case TileType.Monster:
                            Monster tmpmonster = Instantiate(loadData.monsterPrefabDic[room.roomTileArray[x, y].tileName], new Vector3Int(x, y, 0), loadData.monsterPrefabDic[room.roomTileArray[x, y].tileName].transform.rotation, tmptilemap.transform);
                            tmpmonster.ownRoom = room;
                            room.monsterInfoList.Add(new SpawnMonsterInfo(tmpmonster.mType, new Vector2(x, y), tmpmonster));
                            break;
                        case TileType.Item:
                            ItemSc tmpitem = Instantiate(loadData.itemPrefabDic[Item_TYPE.Catnip.ToString()], new Vector3Int(x, y, 0), Quaternion.identity, tmptilemap.transform);
                            break;
                        default:
                            Debug.Log(room.roomTileArray[x, y].tileType.ToString());
                            break;
                    }
                }
            }
        }

        //인게임일 경우에만 생성 (맵툴의 경우에는 배경을 남겨두므로 제외)
        if (ingame)
        {
            //Edge생성
            for (int i = 0; i < room.roomRect.xMax; i++)
            {
                tmptilemap.SetTile(new Vector3Int(i, -1, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                tmptilemap.SetTile(new Vector3Int(i, -2, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                tmptilemap.SetTile(new Vector3Int(i, (int)room.roomRect.yMax, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                tmptilemap.SetTile(new Vector3Int(i, (int)room.roomRect.yMax + 1, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
            }

            //left, right
            for (int j = -2; j < room.roomRect.yMax + 2; j++)
            {
                tmptilemap.SetTile(new Vector3Int(-1, j, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                tmptilemap.SetTile(new Vector3Int(-2, j, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                tmptilemap.SetTile(new Vector3Int((int)room.roomRect.xMax, j, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                tmptilemap.SetTile(new Vector3Int((int)room.roomRect.xMax + 1, j, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
            }
            //배경 생성 
            GameObject tmpParent = new GameObject("BackGroundParent");
            int count = 0;
            foreach (var tmptile in loadData.tileDataArray[room.roomSpriteType].backGroundTile)
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
            tmpParent.transform.SetParent(tmpob.transform);
        }
        room.roomModel = tmpob;
        tmpob.SetActive(false);

        return tmpob;
    }

}

//gridob = new GameObject("TmpGrid", typeof(Grid));

//        foreach (var tm in _tilemaps)
//        {
//            var tmpob = new GameObject(tm.name, typeof(Tilemap), typeof(CompositeCollider2D));
//tmpob.transform.SetParent(gridob.transform);
//            tmpob.AddComponent<TilemapCollider2D>().usedByComposite = true;
//            tmpob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
//            tmpob.AddComponent<TilemapRenderer>().sortingLayerName = "Ground";
//            tmpob.tag = "Ground";
//            tmpob.layer = 8;
//            var tmptilemap = tmpob.GetComponent<Tilemap>();

//            foreach (var tmpos in tm.cellBounds.allPositionsWithin)
//            {
//                if(!tm.HasTile(tmpos))
//                    continue;
//                Debug.Log(tm.GetTile(tmpos).name);
//                switch(tm.GetTile(tmpos).name)
//                {
//                    case "RuleTile_Terrain":
//                        tmptilemap.SetTile(tmpos, loadData.tileDataArray[roomtype].terrainRuleTile);
//                        break;
//                    case "tile_Destructure":
//                        DesStructure tmpds =
//                            Instantiate(loadData.desStructurePrefab[DesStructure_TYPE.Frog.ToString()], tmpos, Quaternion.identity, tmptilemap.transform);
//                        break;
//                    case "tile_Monster":
//                        Monster tmpm = Instantiate(loadData.monsterPrefab[MONSTER_TYPE.Fox.ToString()], tmpos, Quaternion.identity, room.roomModel.transform);
//                        break;
//                    case "tile_Item":
//                        break;
//                }
//                blankTilemaps.Add(tmptilemap);
//            }
//        }
