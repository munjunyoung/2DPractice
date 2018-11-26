using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TileReference : MonoBehaviour
{
    //Resouces Load Path
    private string[] roomTypePathArray = { "Type1" };
    private string[] tileTypePathArray = { "1.BackGround", "2.Floor", "3.Obstacle", "4.Wall" };

    public TileObject[] TileReferenceArray;

    private void Awake()
    {
        TileReferenceArray = new TileObject[roomTypePathArray.Length];

        LoadTileObject();
    }

    /// <summary>
    /// 간단하게 번호로 모두 처리하면 좀더 간단하게 처리 할 수 있지만 일단 구분하기 쉽게 하기 위해 
    /// case문을 처리하고 폴더명을 정확히 정의함
    /// </summary>
    public void LoadTileObject()
    {
        for (int i = 0; i < roomTypePathArray.Length; i++)
        {
            foreach (string _tiletype in tileTypePathArray)
            {
                GameObject[] tmp = Resources.LoadAll<GameObject>(roomTypePathArray[i] + "/" + _tiletype);
                switch (_tiletype)
                {
                    case "1.BackGround":
                        TileReferenceArray[i].backGroundSprite = tmp;
                        break;
                    case "2.Floor":
                        TileReferenceArray[i].floorSprite = tmp;
                        break;
                    case "3.Obstacle":
                        TileReferenceArray[i].obstacleSprite = tmp;
                        break;
                    case "4.Wall":
                        TileReferenceArray[i].wallSprite = tmp;
                        break;
                    default:
                        Debug.Log("해당 폴더명에 맞는 오브젝트가 존재하지 않습니다.");
                        break;

                }
            }
        }
    }
}

/// <summary>
/// 3차원으로 배열을 바꾸어 인덱스로 처리하면 더욱 심플해지지만 명시적으로 처리하기 위해 4개의 배열을 사용
/// </summary>
[Serializable]
public struct TileObject
{
    public GameObject[] backGroundSprite;
    public GameObject[] floorSprite;
    public GameObject[] obstacleSprite;
    public GameObject[] wallSprite;

}
