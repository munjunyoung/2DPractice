using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
class NewTileMapWindow : EditorWindow
{
    public static GameObject gridob = null;
    static GameObject TilemapOb = null;
    public string tilemapName;

    private Vector2 mousepos = Vector2.zero;
    
    private void OnEnable()
    {
        mousepos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Tilemap tmp = new Tilemap();
        
    }

    private void OnGUI()
    {
        //position 설정
        if (position.position != mousepos)
            position = new Rect(mousepos.x, mousepos.y, 300, 125);

        GUILayout.Space(10);
        ////font
        //GUIStyle font = new GUIStyle();
        //font.fontSize = 12;
        //font.fontStyle = FontStyle.Bold;

        GUILayout.Label("   파일 이름을 입력하세요",MapToolWindow.titleFont);
        GUILayout.Label("(입력하지 않을 경우 Tilemap으로 생성됩니다)");
        tilemapName = EditorGUILayout.TextField(tilemapName);
       
        
        GUILayout.Space(20);
        GUILayout.BeginHorizontal("Box");

        if (GUILayout.Button("Create", GUILayout.Height(35)))
        {
            if (gridob == null)
            {
                gridob = GameObject.Find("Grid");
                if (gridob == null)
                    gridob = ObjectFactory.CreateGameObject("Grid", typeof(Grid)); // new GameObject("Grid", typeof(Grid));
            }
            gridob.GetComponent<Grid>().cellGap = new Vector3(-0.01f, -0.01f, 0f);

            if (tilemapName == null)
                tilemapName = "Tilemap";
            GameObject tmptilemap = ObjectFactory.CreateGameObject(tilemapName, typeof(Tilemap), typeof(TilemapRenderer));// new GameObject(tilemapName, typeof(Tilemap), typeof(TilemapRenderer));
            tmptilemap.transform.SetParent(gridob.transform);
            TilemapOb = tmptilemap;
            TilemapOb.AddComponent<CompositeCollider2D>();
            TilemapOb.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            TilemapOb.AddComponent<TilemapCollider2D>().usedByComposite = true;
            TilemapOb.GetComponent<TilemapRenderer>().sortingLayerName = "Ground";
            TilemapOb.tag = "Ground";
            TilemapOb.layer = 8;
            MapToolWindow.SetActiveTilemap(TilemapOb);

            Selection.activeGameObject = tmptilemap;
            EditorGUIUtility.PingObject(tmptilemap);
            this.Close();
        }
        
        if (GUILayout.Button("No", GUILayout.Height(35)))
        {
            this.Close();
        }
        GUILayout.EndHorizontal();
    }
}
