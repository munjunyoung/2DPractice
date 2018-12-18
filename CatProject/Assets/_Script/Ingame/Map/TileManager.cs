using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

enum RoomType { Type1 = 0 }
enum TileType { BackGround = 0, Floor, Obstacle, Ground , GroundOutLine }
/// <summary>
/// 싱글턴을 사용하려다가 참조할 곳이 boardManager밖에 없어서 컴포넌트 참조
/// </summary>
public class TileManager : MonoBehaviour
{
    //Resouces Load Path
    private string[] roomTypePathArray = { "Type1" };
    private string[] tileTypePathArray = { "1.BackGround", "2.Floor", "3.Obstacle", "4.Ground", "5.GroundOutLine" };
    
    public TileObject[] tileReferenceArray;
    
    public void Awake()
    {
        LoadTile();
    }

    /// <summary>
    /// 심플하게 변경하나 인스펙터에서 타입을 정확히 구분하기 힘듬
    /// </summary>
    private void LoadTile()
    {
        tileReferenceArray = new TileObject[roomTypePathArray.Length];
        for (int i = 0; i<roomTypePathArray.Length;i++)
        {
            tileReferenceArray[i].tileType = new TypeofTile[tileTypePathArray.Length];
            for(int j = 0; j<tileTypePathArray.Length;j++)
            {
                GameObject[] tmp = Resources.LoadAll<GameObject>(roomTypePathArray[i] + "/" + tileTypePathArray[j]);
                tileReferenceArray[i].tileType[j].sprite = tmp;
            }
        }
    }
}

/// <summary>
/// 
/// 0 : BackGround 
/// 1 : Floor
/// 2 : Obstacle
/// 3 : Wall
/// </summary>
[Serializable]
public struct TileObject
{
    [Header("0 : BackGround")]
    [Header("1 : Floor")]
    [Header("2 : Obstacle")]
    [Header("3 : Wall")]
    [Header("4 : GroundOutLine")]
    public TypeofTile[] tileType; 

}

/// <summary>
/// 벽과 같은 타일에서도 여러종류를 저장하기 위함(방향 등)
/// </summary>
[Serializable]
public struct TypeofTile
{
    public GameObject[] sprite;
}


/// <summary>
/// 번호로 모두 처리하면 좀더 간단하게 처리 할 수 있지만 일단 구분하기 쉽게 하기 위해 
/// case문을 처리하고 폴더명을 정확히 정의함
/// </summary>
//public void LoadTileObject()

//{
//    for (int i = 0; i < roomTypePathArray.Length; i++)
//    {
//        foreach (string _tiletype in tileTypePathArray)
//        {
//            GameObject[] tmp = Resources.LoadAll<GameObject>(roomTypePathArray[i] + "/" + _tiletype);
//            switch (_tiletype)
//            {
//                case "1.BackGround":
//                    TileReferenceArray[i].backGroundSprite = tmp;
//                    break;
//                case "2.Floor":
//                    TileReferenceArray[i].floorSprite = tmp;
//                    break;
//                case "3.Obstacle":
//                    TileReferenceArray[i].obstacleSprite = tmp;
//                    break;
//                case "4.Wall":
//                    TileReferenceArray[i].wallSprite = tmp;
//                    break;
//                default:
//                    Debug.Log("해당 폴더명에 맞는 오브젝트가 존재하지 않습니다.");
//                    break;

//            }
//        }
//    }
//}