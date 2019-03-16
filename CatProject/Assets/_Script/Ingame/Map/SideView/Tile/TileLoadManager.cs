using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

enum RoomType { Type1 = 0 , Type2}
public enum TileType { BackGround = 0, Entrance, Terrain }
public class TileLoadManager : MonoBehaviour
{
    
    //Resouces Load Path
    public static readonly string[] roomTypePathArray = { "TileType1" , "TileType2" };
    public static readonly string[] tileTypePathArray = { "0.BackGround", "1.Entrance" };
    [HideInInspector]
    public TypeOfTileSetType[] loadTileArray;
    public List<GeneratedTerrainData> loadTerrainDataList = new List<GeneratedTerrainData>();
    

    public void Awake()
    {
        loadTileArray = LoadTile();
        loadTerrainDataList = LoadTerrainData();
    }
    
    /// <summary>
    /// NOTE : Resource Load를 통하여 모든 타일들을 불러옴
    /// NOTE : 에디터에서도 사용하기위하여 STATIC으로 선언(Path 포함)
    /// </summary>
    public static TypeOfTileSetType[] LoadTile()
    {
        TypeOfTileSetType[] tmptilearray;
        tmptilearray = new TypeOfTileSetType[roomTypePathArray.Length];
        for (int i = 0; i < roomTypePathArray.Length; i++)
        {
            tmptilearray[i].tileType = new TypeOfTileSet[tileTypePathArray.Length];
            //RuleTile 저장
            tmptilearray[i].terrainRuleTile = Resources.Load<RuleTile>("Tile/"+ roomTypePathArray[i] + "/RuleTile/RuleTile_Terrain");
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
    public List<GeneratedTerrainData> LoadTerrainData()
    {
        List<GeneratedTerrainData> tmpterrainlist = new List<GeneratedTerrainData>(); 

        Tilemap[] tmploadprefab = Resources.LoadAll<Tilemap>("GeneratedMapData");
        foreach(var tilemapdata in tmploadprefab)
        {
            //데이터를 저장할 맵 배열 생성
            TileInfo[,] tmptileinfoarray = new TileInfo[tilemapdata.cellBounds.xMax, tilemapdata.cellBounds.yMax];
            
            //해당 tilemapdata의 모든 포지션을 검색하여 tile이 설정되어있는 부분을 체크
            foreach(var tilepos in tilemapdata.cellBounds.allPositionsWithin)
            {
                //현재 포지션에 타일이 존재하지 않을경우 
                if (!tilemapdata.HasTile(tilepos))
                    continue;
                tmptileinfoarray[tilepos.x, tilepos.y] = new TileInfo(TileType.Terrain);
            }

            tmpterrainlist.Add(new GeneratedTerrainData(tmptileinfoarray, tilemapdata.cellBounds));
           
        }

        return tmpterrainlist;
    }
}

/// <summary>
/// 타일타입의 종류를 고름
/// </summary>
[Serializable]
public struct TypeOfTileSetType
{
    [Header("0 : BackGround")]
    [Header("1 : Entrance")]
    public TypeOfTileSet[] tileType;
    public RuleTile terrainRuleTile;
}

/// <summary>
/// 타일의 종류
/// </summary>
[Serializable]
public struct TypeOfTileSet
{
    public Tile[] tile;
}

/// <summary>
/// 생성된 배열값과 해당 사이즈 나중에 행과 열의 크기를 구하는방법을 사용하는거보다 미리 저장해서 꺼내쓰는게 좋을것 같다)
/// </summary>
public class GeneratedTerrainData
{
    public TileInfo[,] tilearray;
    public BoundsInt size;

    public GeneratedTerrainData(TileInfo[,] _tilearray, BoundsInt _size)
    {
        tilearray = _tilearray;
        size = _size;
    }
}