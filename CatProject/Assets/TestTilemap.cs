using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TestTilemap : MonoBehaviour
{
    public Tilemap generatedTilemap;
    public Tile testrule;
    public RuleTile testRuleTile;

    List<TileInfo[,]> mapdataList = new List<TileInfo[,]>();
    // Start is called before the first frame update
    void Start()
    {
        //ProcessTilemapData(generatedTilemap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
