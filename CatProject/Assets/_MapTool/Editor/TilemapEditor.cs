using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public class TilemapEditor : ScriptableWizard
{
    static GameObject gridob = null;

    public string tilemapName;
    
    static void Open()
    {
       DisplayWizard<TilemapEditor>("Create TileMap");
    }

    private void OnEnable()
    {
        position = new Rect(0, 0, 320, 250);
    }

    private void OnWizardCreate()
    {
       if (gridob == null)
            gridob = new GameObject("Grid", typeof(Grid));

        if (tilemapName == null)
            tilemapName = "Tilemap";
        GameObject tmptilemap = new GameObject(tilemapName, typeof(Tilemap), typeof(TilemapRenderer));
        tmptilemap.transform.SetParent(gridob.transform);
        
        Selection.activeGameObject = tmptilemap;
        EditorGUIUtility.PingObject(tmptilemap);
    }
}
