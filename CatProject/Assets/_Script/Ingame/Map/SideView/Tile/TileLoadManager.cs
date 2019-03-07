﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

enum RoomType { Type1 = 0 , Type2}
public enum TileType { BackGround = 0, Floor, Ground, Wall, Entrance }
public class TileLoadManager : MonoBehaviour
{
    //Resouces Load Path
    public static readonly string[] roomTypePathArray = { "TileType1" , "TileType2" };
    public static readonly string[] tileTypePathArray = { "0.BackGround", "1.Floor", "2.Ground", "3.Wall", "4.Entrance" };
    [HideInInspector]
    public TypeOfTileSetType[] loadTileArray;

    public void Awake()
    {
        loadTileArray = LoadTile();
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
            for (int j = 0; j < tileTypePathArray.Length; j++)
            {
                Tile[] tmp = Resources.LoadAll<Tile>(roomTypePathArray[i] + "/" + tileTypePathArray[j]);
                tmptilearray[i].tileType[j].tile = tmp;
            }
        }
        return tmptilearray;
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
    [Header("2 : Ground")]
    [Header("3 : Wall")]
    [Header("4 : Entrance")]
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