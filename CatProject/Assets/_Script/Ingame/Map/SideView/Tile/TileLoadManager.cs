using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;


public class TileLoadManager : MonoBehaviour
{
    //Resouces Load Path
    private string[] roomTypePathArray = { "TileType1" };
    private string[] tileTypePathArray = { "1.BackGround", "2.Floor", "3.Obstacle", "4.Ground", "5.GroundOutLine" };

    public TypeOfTileSetType[] loadTileArray;

    public void Awake()
    {
        LoadTile();
    }


    /// <summary>
    /// 
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