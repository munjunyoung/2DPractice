using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public class TilemapEditor : ScriptableWizard
{
    private List<GameObject> activeobList = new List<GameObject>();
    static GameObject gridob = null;

    public string tilemapName;
    
    static void Open()
    {
       DisplayWizard<TilemapEditor>("Create TileMap");
    }

    private void OnEnable()
    {

        position = new Rect(0, 0, 320, 250);
        Debug.Log(this + " " + this.position);
    }

    private void OnWizardCreate()
    {
       if (gridob == null)
            gridob = new GameObject("Grid", typeof(Grid));
        

        GameObject tmptilemap = new GameObject(tilemapName, typeof(Tilemap), typeof(TilemapRenderer));
        tmptilemap.transform.SetParent(gridob.transform);


        activeobList.Add(tmptilemap);

        Selection.objects = activeobList.ToArray();
    }
}
