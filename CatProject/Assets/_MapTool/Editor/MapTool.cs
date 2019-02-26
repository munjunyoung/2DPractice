using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class MapTool : EditorWindow
{
    string type2Path = "Assets/resources/TileType2/TileType2Palette.prefab";
    static string scenePath = "Assets/_Scenes/MapEditScene.unity";
    static string layoutPath = "Assets/_MapTool/Layout/MapEditorLayout.wlt";

    [MenuItem("MapTool/Execute MapTool")]
    static void Open()
    {
        //Change Scene
        EditorSceneManager.OpenScene(scenePath);
        //Layout Load
        LayoutUtility.LoadLayout(layoutPath);
        //Window 생성
        GetWindow<MapTool>();
    }
    
    private void OnGUI()
    {
        
        if (GUILayout.Button("Create TileMap", GUILayout.Height(35)))
        {
            GetWindow<TilemapEditor>();
        }

        if (GUILayout.Button("Create RuleTile", GUILayout.Height(35)))
        {
            SelectionInFolder(type2Path);
            EditorApplication.ExecuteMenuItem("Assets/Create/Tiles/Rule Tile");
        }
    }

    /// <summary>
    /// NOTE : Project란에서 오브젝트 선택
    /// </summary>
    /// <param name="path"></param>
    private void SelectionInFolder(string path)
    {
        Object tmpob = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        Selection.activeObject = tmpob;
    }

    public static System.Type[] GetAllEditorWindowTypes()
    {
        var result = new System.Collections.Generic.List<System.Type>();
        System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
        System.Type editorWindow = typeof(EditorWindow);
        foreach (var A in AS)
        {
            System.Type[] types = A.GetTypes();
            foreach (var T in types)
            {
                if (T.IsSubclassOf(editorWindow))
                    result.Add(T);
            }
        }
        return result.ToArray();
    }
}
