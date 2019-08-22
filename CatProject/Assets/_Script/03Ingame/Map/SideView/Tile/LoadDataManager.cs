using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

enum Room_TileType { Type1 = 0, Type2 }
public enum TileType {Terrain, Structure , Monster, Item}
public enum STRUCTURE_TYPE { Entrance ,Box, Switch, Garbage }
public enum Box_Type { Normal }
public enum Switch_Type {  Normal }

public enum MONSTER_TYPE { Fox = 0 , Dog = 1, Eagle , Squirrel };
public enum Fox_Type { Normal }

public enum Item_TYPE { Catnip = 0 , BigCatnip};
public enum Catnip_Type { Normal }
public class LoadDataManager : MonoBehaviour
{
    private static LoadDataManager _instance = null;
    public static LoadDataManager instance
    {
        get
        {
            if(_instance==null)
                _instance = new LoadDataManager();

            return _instance;
        }
    }
    //Resouces Load Path
    private static readonly string[] roomTypePathArray = { "TileType1", "TileType2" };
    private static readonly string[] tileTypePathArray = { "0.BackGround", "1.Entrance" };

    private readonly string skillSpritePath = "SkillSprite";

    private readonly string structurePefabPath = "Prefab/Structure";
    private readonly string monsterPrefabPath = "Prefab/Character/Monster";
    private readonly string itemPrefabPath = "Prefab/Item";
    private readonly string skillEffectPrefabPath = "Prefab/SkillEffect";
    //Tile 
    public TypeOfTileSetType[] tileDataArray;
    public Dictionary<string, List<GeneratedTerrainData>> terrainDataDic = new Dictionary<string, List<GeneratedTerrainData>>();
   
    public Dictionary<string, Sprite> skillSpriteDic = new Dictionary<string, Sprite>();
    //Prefab
    public Dictionary<string, Monster> monsterPrefabDic = new Dictionary<string, Monster>();
    public Dictionary<string, StructureObject> structurePrefabDic = new Dictionary<string, StructureObject>();
    public Dictionary<string, ItemSc> itemPrefabDic = new Dictionary<string, ItemSc>();
    public Dictionary<string, GameObject> SkillEffectPrefabDic = new Dictionary<string, GameObject>();
    //private void Start()
    //{
    //    tileDataArray = LoadAllTile();
    //    terrainDataDic = LoadAllTerrainData();

    //    SetLoadData(itemPrefabDic, itemPrefabPath);
    //    SetLoadData(skillSpriteDic, skillSpritePath);
    //    SetLoadData(monsterPrefabDic, monsterPrefabPath);
    //    SetLoadData(structurePrefabDic, structurePefabPath);
    //    SetLoadData(desStructurePrefabDic, destructibleStructurePrefabPath);
    //    SetLoadData(switchPrefabDic, switchPrefabPath);
    //}

    /// <summary>
    /// NOTE : 필요한 데이터들 한번에 로드
    /// </summary>
    public LoadDataManager()
    {
        tileDataArray = LoadAllTile();
        terrainDataDic = LoadAllTerrainData();

        SetLoadData(skillSpriteDic, skillSpritePath);
        SetLoadData(itemPrefabDic, itemPrefabPath);
        SetLoadData(monsterPrefabDic, monsterPrefabPath);
        SetLoadData(structurePrefabDic, structurePefabPath);
        SetLoadData(SkillEffectPrefabDic, skillEffectPrefabPath);
    }

    /// <summary>a
    /// NOTE : Resource Load를 통하여 모든 타일들을 불러옴
    /// NOTE : 에디터에서도 사용하기위하여 STATIC으로 선언(Path 포함)
    /// </summary>
    public static TypeOfTileSetType[] LoadAllTile()
    {
        TypeOfTileSetType[] tmptilearray;
        tmptilearray = new TypeOfTileSetType[roomTypePathArray.Length];
        for (int i = 0; i < roomTypePathArray.Length; i++)
        {
            tmptilearray[i].terrainRuleTile = Resources.Load<RuleTile>("Tile/" + roomTypePathArray[i] + "/RuleTile/RuleTile_Terrain");
            //일반 Tile들 저장
            tmptilearray[i].backGroundTile = Resources.LoadAll<Tile>("Tile/" + roomTypePathArray[i] + "/" + tileTypePathArray[0]);
            tmptilearray[i].entranceSprite = Resources.LoadAll<Sprite>("Tile/" + roomTypePathArray[i] + "/" + tileTypePathArray[1]);
           
        }
        return tmptilearray;
    }

    /// <summary>
    /// NOTE : 미리 생성해둔 지형 데이터 TileInfo 배열로 전환
    /// </summary>
    /// <returns></returns>
    private Dictionary<string,List<GeneratedTerrainData>> LoadAllTerrainData()
    {
        Dictionary<string, List<GeneratedTerrainData>> tmpterrainDataDic = new Dictionary<string, List<GeneratedTerrainData>>();
        Tilemap[] puzzleloadprefab = Resources.LoadAll<Tilemap>("GeneratedMapData/Puzzle");
        Tilemap[] terrainloadprefab = Resources.LoadAll<Tilemap>("GeneratedMapData/Terrain");
        tmpterrainDataDic.Add("Puzzle", SetTerrainList(puzzleloadprefab));
        tmpterrainDataDic.Add("Terrain", SetTerrainList(terrainloadprefab));

        return tmpterrainDataDic;
    }

    /// <summary>
    /// NOTE : Tilemap 데이터 Tileinfo array로 변환 및 시작과 끝 높이 저장
    /// </summary>
    /// <param name="_prefab"></param>
    /// <returns></returns>
    private List<GeneratedTerrainData> SetTerrainList(Tilemap[] _prefab)
    {
        List<GeneratedTerrainData> tmpterrainlist = new List<GeneratedTerrainData>();
        foreach (var tilemapdata in _prefab)
        {
            //데이터를 저장할 맵 배열 생성
            TileInfo[,] tmptileinfoarray = AnalyzeTileMap(tilemapdata);

            //시작과 끝지점의 가장 아래지형의 바닥의 높이 저장
            int startHeight = -2;
            int endHeight = -2;

            for (int j = 0; j < tilemapdata.cellBounds.yMax; j++)
            {
                //시작높이
                if (startHeight == -2 && tmptileinfoarray[0, j] == null)
                    startHeight = j - 1;
                //끝높이
                if (endHeight == -2 && tmptileinfoarray[tilemapdata.cellBounds.xMax - 1, j] == null)
                    endHeight = j - 1;

                //두개 모두 채워졌을경우 반복문 종료
                if (!startHeight.Equals(-2) && !endHeight.Equals(-2))
                    break;

                //마지막 횟수 일때에 모두다 값이 변경되지 않았을 경우 (null이 없이 모두 지형일 경우)
                if (j.Equals(tilemapdata.cellBounds.yMax - 1))
                {
                    startHeight = startHeight.Equals(-2) ? tilemapdata.cellBounds.yMax - 1 : startHeight;
                    endHeight = endHeight.Equals(-2) ? tilemapdata.cellBounds.yMax - 1 : endHeight;
                }
            }
            tmpterrainlist.Add(new GeneratedTerrainData(tmptileinfoarray, tilemapdata.cellBounds, startHeight, endHeight));
        }

        return tmpterrainlist;
    }

    /// <summary>
    /// NOTE : Tilemap 을 Tileinfo array로 변경
    /// </summary>
    /// <param name="_tilemaps"></param>
    public TileInfo[,] AnalyzeTileMap(Tilemap _tm)
    {
        
        TileInfo[,] tmptilearray = new TileInfo[_tm.size.x, _tm.size.y];
        foreach (var tmpos in _tm.cellBounds.allPositionsWithin)
        {
            //해당 포지션에 아무것도 없을경우 진행
            if (!_tm.HasTile(tmpos))
                continue;
            //땅일 경우 바로 생성 
            if (_tm.GetTile(tmpos).name.Equals("RuleTile_Terrain"))
            {
                tmptilearray[tmpos.x, tmpos.y] = new TileInfo(TileType.Terrain);
                continue;
            }
            // ex) Monster_0  -> _기준으로 구분하여 처리
            var name = _tm.GetTile(tmpos).name;
            int subidx = name.IndexOf("_");
            string tiletype = name.Substring(0, subidx);
            string secondstring = name.Substring(subidx + 1).ToString();

            int secondsubidx = secondstring.IndexOf("_");
            string tilename = secondstring.Substring(0, secondsubidx);
            string tilenamestype = secondstring.Substring(secondsubidx + 1);
            
            //Debug.Log(_tm.GetTile(tmpos).name);
            //Debug.Log("load data - type : " + tiletype + " name : " + tilename + " typename : " + tilenamestype);
            switch (tiletype)
            {
                case "Structure":   
                    tmptilearray[tmpos.x, tmpos.y] = new TileInfo(TileType.Structure, tilename, tilenamestype);
                    break;
                case "Monster":
                    tmptilearray[tmpos.x, tmpos.y] = new TileInfo(TileType.Monster, tilename, tilenamestype);
                    break;
                case "Item":
                    tmptilearray[tmpos.x, tmpos.y] = new TileInfo(TileType.Item, tilename, tilenamestype);
                    break;
                case "Switch":
                    tmptilearray[tmpos.x, tmpos.y] = new TileInfo(TileType.Structure, tilename, tilenamestype);
                    break;
            }
        }
        return tmptilearray;
    }


    /// <summary>
    /// NOTE : dictionary와 path를 변수를 선언하고 파라미터 입력, 해당 데이터 로드하고 dictionary 초기화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_dic"></param>
    /// <param name="_path"></param>
    private void SetLoadData<T>(Dictionary<string, T> _dic, string _path)
    {
        //T 타입 데이터 캐스팅 로드
        var loadob = Resources.LoadAll(_path, typeof(T)).Cast<T>().ToArray();


        foreach (var lo in loadob)
        {
            //key값의 name설정을 위한 object로 타입 변환
            UnityEngine.Object tmp = lo as UnityEngine.Object;
            _dic.Add(tmp.name, lo);
        }
    }

}

/// <summary>
/// NOTE : 타일타입의 종류를 고름 배경 타일, 출입구 타일 , terrainRuleTile : RuleTile 타입 Terrain 데이터
/// </summary>
public struct TypeOfTileSetType
{
    public Tile[] backGroundTile;
    /// <summary>
    ///  NOTE : 0번은 닫힌 문 1번은 열린 문
    /// </summary>
    public Sprite[] entranceSprite;
    public RuleTile terrainRuleTile;
}


/// <summary>
/// NOTE : 생성된 배열값과 해당 사이즈 나중에 행과 열의 크기를 구하는방법을 사용하는거보다 미리 저장해서 꺼내쓰는게 좋을것 같다)
/// </summary>
public class GeneratedTerrainData
{
    public TileInfo[,] tileArray;
    public BoundsInt size;
    public int startHeight;
    public int endHeight;

    public GeneratedTerrainData(TileInfo[,] _tilearray, BoundsInt _size, int _startheight, int _endheight)
    {
        tileArray = _tilearray;
        size = _size;
        startHeight = _startheight;
        endHeight = _endheight;
    }
}




///// <summary>
///// NOTE : 몬스터 PREFAB 모두 로드 하고 Dictionary에 저장후 리턴 
///// </summary>
//private Dictionary<string, Monster> LoadMonsterPrefab()
//{
//    var monsters = Resources.LoadAll<GameObject>(monsterPrefabPath);
//    Dictionary<string, Monster> tmpdic = new Dictionary<string, Monster>();
//    foreach (var m in monsters)
//        tmpdic.Add(m.name, m.GetComponent<Monster>());
//    return tmpdic;
//}

///// <summary>
///// NOTE : 구조물 PREFAB 모두 로드 하고 Dictionary에 저장후 리턴 
///// </summary>
///// <returns></returns>
//private Dictionary<string, GameObject> LoadStructurePrefab()
//{
//    var structures = Resources.LoadAll<GameObject>(structurePefabPath);
//    Dictionary<string, GameObject> tmpDic = new Dictionary<string, GameObject>();
//    foreach (var s in structures)
//        tmpDic.Add(s.name, s);

//    return tmpDic;
//}

///// <summary>
///// NOTE : 파괴되는 구조물 PREFAB 모두 로드 하고 Dictionary에 저장후 리턴
///// </summary>
///// <returns></returns>
//private Dictionary<string, DesStructure> LoadDesStructurePrefab()
//{
//    var destructure = Resources.LoadAll<GameObject>(DestructibleStructurePrefabPath);
//    Dictionary<string, DesStructure> tmpdic = new Dictionary<string, DesStructure>();
//    foreach (var ds in destructure)
//        tmpdic.Add(ds.name, ds.GetComponent<DesStructure>());
//    return tmpdic;
//}

///// <summary>
///// NOTE : 스킬관련 Sprite 이미지
///// </summary>
///// <returns></returns>
//private Dictionary<string, Sprite> LoadSkillSprite()
//{
//    var sprites = Resources.LoadAll<Sprite>(SkillSpritePath);
//    Dictionary<string, Sprite> tmpdic = new Dictionary<string, Sprite>();
//    foreach (var s in sprites)
//        tmpdic.Add(s.name, s);

//    return tmpdic;
//}

//private Dictionary<string, GameObject> LoadItemPrefab()
//{
//    var itemob = Resources.LoadAll<GameObject>(ItemPrefabPath);
//    Dictionary<string, GameObject> tmpdic = new Dictionary<string, GameObject>();
//    foreach (var it in itemob)
//        tmpdic.Add(it.name, it);

//    return tmpdic;
//}
