using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapTool : EditorWindow
{
    public string gameObjectName;

    [MenuItem("MapTool/Show Pallete")]
    static void Open()
    {
        var tilemap = new Tilemap();
        ProjectWindowUtil.CreateAsset(tilemap, "TileMap");
    }
    
}

