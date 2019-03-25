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
    private static readonly string[] roomTypePathArray = { "TileType1" , "TileType2" };
    private static readonly string[] tileTypePathArray = { "0.BackGround", "1.Entrance" };
    //[HideInInspector]
    public TypeOfTileSetType[] loadTileArray;
    public List<GeneratedTerrainData> loadTerrainDataList = new List<GeneratedTerrainData>();
    
    public void Awake()
    {
        loadTileArray = LoadAllTile();
        loadTerrainDataList = LoadAllTerrainData();
    }
    
    /// <summary>
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
    /// <returns></returns>
    public List<GeneratedTerrainData> LoadAllTerrainData()
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

            //시작과 끝지점의 가장 아래지형의 바닥의 높이 저장
            int startHeight = -2;
            int endHeight = -2;

            for (int j=0; j<tilemapdata.cellBounds.yMax;j++)
            {
                //시작높이
                if (startHeight == -2&&tmptileinfoarray[0, j]==null)
                    startHeight = j - 1;
                //끝높이
                if (endHeight == -2&&tmptileinfoarray[tilemapdata.cellBounds.xMax - 1, j]==null)
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
    
}

/// <summary>
/// NOTE : 타일타입의 종류를 고름 ( 맵 에디터 툴에서도 사용하므로 클래스 내부에 선언하지 않음)
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