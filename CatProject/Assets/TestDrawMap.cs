using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/// <summary>
/// NOTE : Draw Map 클래스 
/// </summary>
public class TestDrawMap : MonoBehaviour
{
    private static TestDrawMap _instance = null;
    public static TestDrawMap instance
    {
        get
        {
            if (_instance == null)
                _instance = new TestDrawMap();

            return _instance;
        }
    }

    private List<Tilemap> tilemapOb;
    private LoadDataManager loadData;

    private void Start()
    {
        loadData = LoadDataManager.instance;
        FindTilemap();
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
            roomlist.Add(AnalyzeTileMap(tm));

        DrawTilemap(roomlist);

        tmpgrid.SetActive(false);
    }


    /// <summary>
    /// NOTE : Tilemap 을 Tileinfo array로 변경
    /// </summary>
    /// <param name="_tilemaps"></param>
    private DungeonRoom AnalyzeTileMap(Tilemap _tm)
    {
        DungeonRoom tmproom = new DungeonRoom(1, _tm.size.x, _tm.size.y);
        foreach (var tmpos in _tm.cellBounds.allPositionsWithin)
        {
            if (!_tm.HasTile(tmpos))
                continue;
            Debug.Log(_tm.GetTile(tmpos));
            switch (_tm.GetTile(tmpos).name)
            {
                case "RuleTile_Terrain":
                    tmproom.roomGroundArray[tmpos.x, tmpos.y] = new TileInfo(TileType.Terrain);
                    break;
                case "tile_Destructure":
                    tmproom.roomGroundArray[tmpos.x, tmpos.y] = new TileInfo(TileType.Destructure);
                    break;
                case "tile_Monster":
                    tmproom.roomGroundArray[tmpos.x, tmpos.y] = new TileInfo(TileType.Monster);
                    break;
                case "tile_Item":
                    tmproom.roomGroundArray[tmpos.x, tmpos.y] = new TileInfo(TileType.Item);
                    break;
            }
        }
        return tmproom;
    }

    private void DrawTilemap(List<DungeonRoom> _rooms)
    {
        //부모 생성 
        GameObject gridob = new GameObject("TmpGrid", typeof(Grid));
        gridob.GetComponent<Grid>().cellGap = new Vector3(-0.001f, -0.001f, 0);

        int count = 0;
        foreach (var room in _rooms)
        {
            var tmpob = new GameObject("Room" + count, typeof(Tilemap), typeof(CompositeCollider2D));
            tmpob.transform.SetParent(gridob.transform);
            tmpob.AddComponent<TilemapCollider2D>().usedByComposite = true;
            tmpob.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            tmpob.AddComponent<TilemapRenderer>().sortingLayerName = "Ground";
            tmpob.tag = "Ground";
            tmpob.layer = 8;
            Tilemap tmptilemap = tmpob.GetComponent<Tilemap>();
            count++;

            for (int x = 0; x < room.roomRect.xMax; x++)
            {
                for (int y = 0; y < room.roomRect.yMax; y++)
                {
                    if (room.roomGroundArray[x, y] != null)
                    {
                        switch (room.roomGroundArray[x, y].tileType)
                        {
                            case TileType.Terrain:
                                tmptilemap.SetTile(new Vector3Int(x, y, 0), loadData.tileDataArray[room.roomSpriteType].terrainRuleTile);
                                break;
                            case TileType.Entrance:
                                GameObject tmpen = Instantiate(loadData.structurePrefab[TileType.Entrance.ToString()], new Vector3(x + 0.5f, y + 1f, 0), Quaternion.identity);
                                tmpen.GetComponent<SpriteRenderer>().sprite = loadData.tileDataArray[room.roomSpriteType].entranceTile[room.roomGroundArray[x, y].tileNumber].sprite;
                                tmpen.GetComponent<SpriteRenderer>().sortingLayerName = "Entrance";
                                tmpen.GetComponent<EntranceSc>().doorOpenSprite = loadData.tileDataArray[room.roomSpriteType].entranceTile[room.roomGroundArray[x, y].tileNumber + 1].sprite;
                                tmpen.transform.SetParent(tmpob.transform);
                                break;
                            case TileType.Destructure:
                                DesStructure tmpds = Instantiate(loadData.desStructurePrefab[DesStructure_TYPE.Frog.ToString()], new Vector3Int(x, y, 0), Quaternion.identity, tmptilemap.transform);
                                break;
                            case TileType.Monster:
                                Monster tmpm = Instantiate(loadData.monsterPrefab[MONSTER_TYPE.Fox.ToString()], new Vector3Int(x, y, 0), Quaternion.identity, tmptilemap.transform);
                                break;
                            case TileType.Item:
                                ItemSc tmpitem = Instantiate(loadData.itemPrefabDic[Item_TYPE.Catnip.ToString()], new Vector3Int(x, y, 0), Quaternion.identity, tmptilemap.transform);
                                break;
                            default:
                                Debug.Log(room.roomGroundArray[x, y].tileType.ToString());
                                break;
                        }
                    }
                }
            }


        }
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
