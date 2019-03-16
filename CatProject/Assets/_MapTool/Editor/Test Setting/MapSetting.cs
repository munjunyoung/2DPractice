using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSetting : EditorWindow
{
    static TypeOfTileSetType[] loadTileArray;
    static Tilemap maptile;
    static GameObject backGroundOb;


    private void OnEnable()
    {
        loadTileArray = TileLoadManager.LoadTile();
        maptile = GameObject.Find("TestGround").GetComponent<Tilemap>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void CreateNormalMap(int type, int width, int height, bool backgroundon)
    {
        maptile.ClearAllTiles();
    
        for(int i=0; i<width;i++)
        {
            maptile.SetTile(new Vector3Int(i, 0, 0), loadTileArray[type].terrainRuleTile);
            maptile.SetTile(new Vector3Int(i, 1, 0),  loadTileArray[type].terrainRuleTile);
            maptile.SetTile(new Vector3Int(i, height-1, 0),  loadTileArray[type].terrainRuleTile);
            maptile.SetTile(new Vector3Int(i, height-2, 0),  loadTileArray[type].terrainRuleTile);
        }

        for(int j=1;j<height-1;j++)
        {
            maptile.SetTile(new Vector3Int(0, j, 0),  loadTileArray[type].terrainRuleTile);
            maptile.SetTile(new Vector3Int(1, j, 0),  loadTileArray[type].terrainRuleTile);
            maptile.SetTile(new Vector3Int(width - 1, j, 0),  loadTileArray[type].terrainRuleTile);
            maptile.SetTile(new Vector3Int(width - 2, j, 0),  loadTileArray[type].terrainRuleTile);
        }

        ///배경 생성
        if (backgroundon)
        {
            backGroundOb = CreateBackGround(type, width, height);
            backGroundOb.transform.SetParent(maptile.transform);
        }
        else
        {
            backGroundOb = GameObject.Find("BackGroundParent");
            if (backGroundOb != null)
                DestroyImmediate(backGroundOb);
        }
    }

    public GameObject CreateBackGround(int type, int width, int height)
    {
        if (backGroundOb == null)
            backGroundOb = GameObject.Find("BackGroundParent");
        
        DestroyImmediate(backGroundOb);

        GameObject tmpParent = new GameObject("BackGroundParent");
        int count = 0;
        //배경 오브젝트 생성
        foreach (var tmptile in loadTileArray[type].tileType[0].tile)
        {
            GameObject backgroundob = new GameObject("BackGround", typeof(SpriteRenderer));
            backgroundob.transform.localPosition = Vector3.zero;
            backgroundob.transform.localRotation = Quaternion.identity;
            backgroundob.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
            backgroundob.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
            backgroundob.GetComponent<SpriteRenderer>().sprite = tmptile.sprite;
            backgroundob.GetComponent<SpriteRenderer>().size = new Vector2(width, height) - Vector2.one;
            backgroundob.GetComponent<SpriteRenderer>().sortingOrder = count;
            count++;
            backgroundob.transform.SetParent(tmpParent.transform);
        }

        return tmpParent;
    }
}
