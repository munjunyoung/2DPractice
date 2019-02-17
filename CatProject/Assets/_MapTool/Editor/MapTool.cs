using UnityEditor;
using UnityEngine;

public class MapTool : EditorWindow
{
    [MenuItem("MapTool/SHOW MAPTOOL")]
    static void Open()
    {
        GetWindow<MapTool>();
        
        
    }

    private void OnEnable()
    {
        position = new Rect(0, 0, 320, 320);
        Debug.Log(position);
    }

    private void OnGUI()
    {
        Debug.Log("tEST");
        if (GUILayout.Button("Create TileMap", GUILayout.Height(50)))
        {
            GetWindow<TilemapEditor>();
        }

        if (GUILayout.Button("Show Palette",GUILayout.Height(50)))
        {
            
        }

    }
}

