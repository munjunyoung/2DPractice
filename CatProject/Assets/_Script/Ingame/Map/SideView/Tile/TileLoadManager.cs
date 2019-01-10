using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

enum RoomType { Type1 = 0 }
enum TileType { BackGround = 0, Floor, Entrance, Ground, GroundOutLine }
public class TileLoadManager : MonoBehaviour
{
    //Resouces Load Path
    private string[] roomTypePathArray = { "TileType1" };
    private string[] tileTypePathArray = { "1.BackGround", "2.Floor", "3.Entrance", "4.Ground", "5.GroundOutLine" };
    [HideInInspector]
    public TypeOfTileSetType[] loadTileArray;

    public void Awake()
    {
        LoadTile();
    }
    
    /// <summary>
    /// NOTE : Resource Load를 통하여 모든 타일들을 불러옴
    /// </summary>
    private void LoadTile()
    {
        loadTileArray = new TypeOfTileSetType[roomTypePathArray.Length];
        for (int i = 0; i < roomTypePathArray.Length; i++)
        {
            loadTileArray[i].tileType = new TypeOfTileSet[tileTypePathArray.Length];
            for (int j = 0; j < tileTypePathArray.Length; j++)
            {
                Tile[] tmp = Resources.LoadAll<Tile>(roomTypePathArray[i] + "/" + tileTypePathArray[j]);
                loadTileArray[i].tileType[j].tile = tmp;
            }
        }
    }
}

/// <summary>
/// 타일타입의 종류를 고름
/// </summary>
[Serializable]
public struct TypeOfTileSetType
{
    [Header("0 : BackGround")]
    [Header("1 : Floor")]
    [Header("2 : Obstacle")]
    [Header("3 : Wall")]
    [Header("4 : GroundOutLine")]
    public TypeOfTileSet[] tileType;
}

/// <summary>
/// 타일의 종류
/// </summary>
[Serializable]
public struct TypeOfTileSet
{
    public Tile[] tile;

}