using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TestTilemap : MonoBehaviour
{
    public Tilemap generatedTilemap;
    public RuleTile testRuleTile;

    List<TileInfo[,]> mapdataList = new List<TileInfo[,]>();
    // Start is called before the first frame update
    void Start()
    {
        Test();
        //ProcessTilemapData(generatedTilemap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Test()
    {

        generatedTilemap.SetTile(new Vector3Int(0,0,0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(0, 1, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(0, 2, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(0, 3, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(1, 0, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(1, 1, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(1, 2, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(1, 3, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(2, 0, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(2, 1, 0), testRuleTile);
        generatedTilemap.SetTile(new Vector3Int(2, 1, 0), testRuleTile);

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
