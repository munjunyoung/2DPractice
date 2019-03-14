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
    
    private void ProcessTilemapData(Tilemap _generatedTilemap)
    {
        TileInfo[,] tilearray = new TileInfo[_generatedTilemap.cellBounds.xMax, _generatedTilemap.cellBounds.yMax];

        foreach (var tilepos in _generatedTilemap.cellBounds.allPositionsWithin)
        {
            if (!_generatedTilemap.HasTile(tilepos))
                continue;
            Debug.Log(_generatedTilemap.GetTile(tilepos));
            
        }
    }
}

public struct GeneratedMapData
{
    BoundsInt tilemapBound;

}
