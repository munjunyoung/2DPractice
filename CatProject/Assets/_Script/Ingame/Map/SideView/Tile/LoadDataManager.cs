﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

enum Room_TileType { Type1 = 0, Type2 }
public enum TileType { BackGround = 0, Entrance, Terrain }
public class LoadDataManager 
{
    private LoadDataManager _instance;
    public LoadDataManager instance; 
    //Resouces Load Path
    private static readonly string[] roomTypePathArray = { "TileType1", "TileType2" };
    private static readonly string[] tileTypePathArray = { "0.BackGround", "1.Entrance" };
    private readonly string structurePefabPath = "Structure";
    private readonly string monsterPrefabPath = "Character/Monster";
    private readonly string DestructibleStructurePrefabPath = "DestructibleStructure";
    private readonly string SkillSpritePath = "BuffSkillSprite";
    //Tile 
    public TypeOfTileSetType[] tileDataArray;
    public List<GeneratedTerrainData> terrainDataList = new List<GeneratedTerrainData>();
    //Prefab
    public Dictionary<string, Monster> monsterPrefab = new Dictionary<string, Monster>();
    public Dictionary<string, GameObject> structurePrefab = new Dictionary<string, GameObject>();
    public Dictionary<string, DesStructure> DesStructurePrefab = new Dictionary<string, DesStructure>();
    public Dictionary<string, Sprite> skillSpriteDic = new Dictionary<string, Sprite>();
    /// <summary>
    /// 
    /// </summary>
    public LoadDataManager()
    {
        tileDataArray = LoadAllTile();
        terrainDataList = LoadAllTerrainData();

        monsterPrefab = LoadMonsterPrefab();
        structurePrefab = LoadStructurePrefab();
        DesStructurePrefab = LoadDesStructurePrefab();
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
            tmptilearray[i].tileType = new TypeOfTileSet[tileTypePathArray.Length];
            //RuleTile 저장
            tmptilearray[i].terrainRuleTile = Resources.Load<RuleTile>("Tile/" + roomTypePathArray[i] + "/RuleTile/RuleTile_Terrain");
            //일반 Tile들 저장
            for (int j = 0; j < tileTypePathArray.Length; j++)
            {
                Tile[] tmp = Resources.LoadAll<Tile>("Tile/" + roomTypePathArray[i] + "/" + tileTypePathArray[j]);
                tmptilearray[i].tileType[j].tile = tmp;
            }
        }
        return tmptilearray;
    }

    /// <summary>
    /// NOTE : 미리 생성해둔 지형 데이터 TileInfo 배열로 전환
    /// </summary>
    /// <returns></returns>
    private List<GeneratedTerrainData> LoadAllTerrainData()
    {
        List<GeneratedTerrainData> tmpterrainlist = new List<GeneratedTerrainData>();

        Tilemap[] tmploadprefab = Resources.LoadAll<Tilemap>("GeneratedMapData");
        foreach (var tilemapdata in tmploadprefab)
        {
            //데이터를 저장할 맵 배열 생성
            TileInfo[,] tmptileinfoarray = new TileInfo[tilemapdata.cellBounds.xMax, tilemapdata.cellBounds.yMax];
            //해당 tilemapdata의 모든 포지션을 검색하여 tile이 설정되어있는 부분을 체크
            foreach (var tilepos in tilemapdata.cellBounds.allPositionsWithin)
            {
                //현재 포지션에 타일이 존재하지 않을경우 
                if (!tilemapdata.HasTile(tilepos))
                    continue;
                tmptileinfoarray[tilepos.x, tilepos.y] = new TileInfo(TileType.Terrain);
            }

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
    /// NOTE : 몬스터 PREFAB 모두 로드 하고 Dictionary에 저장후 리턴 
    /// </summary>
    private Dictionary<string, Monster> LoadMonsterPrefab()
    {
        var monsters = Resources.LoadAll<GameObject>(monsterPrefabPath);
        Dictionary<string, Monster> tmpdic = new Dictionary<string, Monster>();
        foreach (var m in monsters)
            tmpdic.Add(m.name, m.GetComponent<Monster>());
        return tmpdic;
    }

    /// <summary>
    /// NOTE : 구조물 PREFAB 모두 로드 하고 Dictionary에 저장후 리턴 
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, GameObject> LoadStructurePrefab()
    {
        var structures = Resources.LoadAll<GameObject>(structurePefabPath);
        Dictionary<string, GameObject> tmpDic = new Dictionary<string, GameObject>();
        foreach (var s in structures)
            tmpDic.Add(s.name, s);

        return tmpDic;
    }
    
    /// <summary>
    /// NOTE : 파괴되는 구조물 PREFAB 모두 로드 하고 Dictionary에 저장후 리턴
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, DesStructure> LoadDesStructurePrefab()
    {
        var destructure = Resources.LoadAll<GameObject>(DestructibleStructurePrefabPath);
        Dictionary<string, DesStructure> tmpdic = new Dictionary<string, DesStructure>();
        foreach (var ds in destructure)
            tmpdic.Add(ds.name, ds.GetComponent<DesStructure>());
        return tmpdic;
    }

    private Dictionary<string, Sprite> LoadSkillSprite()
    {
        var sprites = Resources.LoadAll<GameObject>(SkillSpritePath);
        Dictionary<string, Sprite> tmpdic = new Dictionary<string, Sprite>();
        foreach (var s in sprites)
            tmpdic.Add(s.name, s.GetComponent<Sprite>());

        return tmpdic;
    }

}

/// <summary>
/// NOTE : 타일타입의 종류를 고름 tileType : 로드된 Tile들의 배열, terrainRuleTile : RuleTile 타입 Terrain 데이터
/// </summary>
public struct TypeOfTileSetType
{
    public TypeOfTileSet[] tileType;
    public RuleTile terrainRuleTile;
}

/// <summary>
/// NOTE : 타일의 종류 0 - 배경 1 - 출입구 
/// </summary>
public struct TypeOfTileSet
{
    public Tile[] tile;
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