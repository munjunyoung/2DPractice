using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneration : MonoBehaviour
{
    public int width;
    public int height;

    private string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    [Header("UI GameObject")]
    public List<GameObject> tileList = new List<GameObject>();
    public List<GameObject> wallList = new List<GameObject>();
    public List<GameObject> bushLis = new List<GameObject>();
        
    // Use this for initialization
    void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        //  Debug.Log(new System.Random(Time.time.ToString().GetHashCode()).Next(0,100));
       // if (Input.GetMouseButtonDown(0))
       //     GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 1; i++)
        {
            SmoothMap();
        }

        DrawMap();
    }

    /// <summary>
    ///
    /// </summary>
    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if(neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    /// <summary>
    /// wallCount
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                //
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                        wallCount += map[neighbourX, neighbourY];
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }


    /// <summary>
    /// 랜덤으로 맵을 채우기위함
    /// </summary>
    void RandomFillMap()
    {
        //useRandomSeed 사용시 맵의 랜덤
        if (useRandomSeed)
            seed = Time.time.ToString();

        System.Random psuedoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 1;
                else
                    map[x, y] = (psuedoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

    }

    /// <summary>
    /// 0,1로 생성된 맵을 통해서 gizmos함수를 통하여 색상별 큐브 생성
    /// </summary>
    void DrawMap()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject tmpSpriteObject = (map[x, y] == 1) ? Instantiate(wallList[0]) : Instantiate(tileList[0]);
                    tmpSpriteObject.transform.SetParent(this.transform);
                    tmpSpriteObject.transform.position = new Vector2(x*.15f, y*.15f);
                }
            }
        }
    }
}
