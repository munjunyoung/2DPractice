using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.Tilemaps;

public class MapToolWindow : EditorWindow
{
    public static GUIStyle titleFont = new GUIStyle();
    private int buttonHeight = 35;

    private static string beforesSceneName = null;

    string type2Path = "Assets/resources/TileType2";
    static string scenePath = "Assets/_Scenes/";
    static string layoutPath = "Assets/_MapTool/Layout/";
    private string loadFolderPath = "Assets/resources/GeneratedMapData";
    private string csvfilepath = "Assets/resources/Prefab/Character/CSVData";

    [MenuItem("MapTool/Execute MapTool")]
    static void Open()
    {
        //Layout Save - 현재 씬이 맵 에디터씬이 아닌경우 layout저장 (맵 에디터를 종료 했을때 layout을 되돌려주기 위함
        if ((beforesSceneName = EditorSceneManager.GetActiveScene().name) != "MapEditScene")
            LayoutUtility.SaveLayout(layoutPath + "SaveLayout.wlt");

        Debug.Log(beforesSceneName);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
      
        //Change Scene
        EditorSceneManager.OpenScene(scenePath + "MapEditorScene/MapEditScene.unity");
        
        //Layout Load
        LayoutUtility.LoadLayout(layoutPath + "MapEditorLayout.wlt");

        EditorUtility.DisplayDialog("시작 경고", "그리기 전에 Tile Palette 창의 상단에 Active Tilemap을 확인해주세요", "OK");
        //Window 생성
        GetWindow<MapToolWindow>();
    }

    private void OnGUI()
    {
        titleFont.fontSize = 12;
        titleFont.fontStyle = FontStyle.Bold;
        
        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        GUILayout.Label("  TILE MAP", titleFont);
        GUILayout.BeginHorizontal("Box");
        // 새로운 타일맵 생성 
        if (GUILayout.Button("New TileMap", GUILayout.Height(buttonHeight)))
        {
            GetWindow<NewTileMapWindow>();
        }
        //타일팔레트 관련 내용으로 룰타일 생성
        if (GUILayout.Button("New RuleTile", GUILayout.Height(buttonHeight)))
        {
            SelectionInFolder(type2Path);
            EditorApplication.ExecuteMenuItem("Assets/Create/Tiles/Rule Tile");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("Box");
        // 기존 타일맵 로드
        if (GUILayout.Button("Load TileMap", GUILayout.Height(buttonHeight)))
        {
            FindLoadObject();
        }
        // 만든 타일맵 저장
        if (GUILayout.Button("Save TileMap", GUILayout.Height(buttonHeight)))
        {
            GetWindow<SaveObjectWindow>();
        }
        GUILayout.EndHorizontal();

        // 현재 씬에 만들어둔 타일맵 제거

        if (GUILayout.Button("Clear All TileMap", GUILayout.Height(buttonHeight)))
        {
            if (EditorUtility.DisplayDialog("All Delete", "All Delete TileMap in Hierarchy, Really?", "Yes", "No"))
            {
                var tmpgridob = GameObject.Find("Grid");

                if (tmpgridob == null)
                {
                    EditorUtility.DisplayDialog("Not Exist", "Not Exist", "OK");
                    return;
                }
                Object.DestroyImmediate(tmpgridob);
            }
        }
       
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        GUILayout.Label("  TEST SETTING", titleFont);
        //Test 환경 설정

        if (GUILayout.Button("Set Test Environment", GUILayout.Height(buttonHeight)))
        {
            GetWindow<TestSettingWindow>();
        }

        if(GUILayout.Button("Clear Player Character",GUILayout.Height(buttonHeight)))
        {
            if (EditorUtility.DisplayDialog("All Delete", "All Delete Player in Hierarchy, Really?", "Yes", "No"))
            {
                var playerob = GameObject.FindGameObjectWithTag("Player");
                DestroyImmediate(playerob);
            }
        }

        if (GUILayout.Button(" Data Setting Folder", GUILayout.Height(buttonHeight)))
        {
            FindCSVFile();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        GUILayout.Label("  ETC", titleFont);
        //에디터 종료 
        if (GUILayout.Button("MAP TOOL EXIT", GUILayout.Height(buttonHeight)))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            LayoutUtility.LoadLayout(layoutPath + "SaveLayout.wlt");
            if (beforesSceneName == null)
            {
                EditorUtility.DisplayDialog("NO", "이전 저장된 씬내용이 없습니다! LEVEL 1 SCENE으로 이동합니다", "OK");
                EditorSceneManager.OpenScene(scenePath + "S_00StartScene.unity");
                return;
            }
            EditorSceneManager.OpenScene(scenePath + beforesSceneName +".unity");
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// NOTE : Project란에서 오브젝트 선택 (해당 오브젝트들을 생성할때 해당 경로에서 생성 되도록 하기 위함 )
    /// </summary>
    /// <param name="path"></param>
    private void SelectionInFolder(string path)
    {
        Object tmpob = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        Selection.activeObject = tmpob;
    }

    private void FindCSVFile()
    {
        string path = EditorUtility.OpenFilePanel("SearchCSVFile", csvfilepath,"csv");
        if(path.Length != 0)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
    /// <summary>
    /// NOTE : Object Load File
    /// </summary>
    private void FindLoadObject()
    {
        string path = EditorUtility.OpenFilePanel("SearchFile", loadFolderPath, "prefab");
        if (path.Length != 0)
        {
            int startpathindex = path.IndexOf("Assets");
            string tmppath = path.Substring(startpathindex);
            Object loadtilemap = AssetDatabase.LoadAssetAtPath(tmppath, typeof(Object));
            GameObject tmpob = PrefabUtility.InstantiatePrefab(loadtilemap) as GameObject;

            var gridob = GameObject.Find("Grid");
            //현재 hirarchy에서 grid ob가 존재하지 않을 경우 
            if (gridob == null)
            {
                gridob = new GameObject("Grid", typeof(Grid));
                gridob.tag = "DrawGrid";
            }

            gridob.GetComponent<Grid>().cellGap = new Vector3(-0.01f, -0.01f, 0f);
            tmpob.transform.SetParent(gridob.transform);
            
            SetActiveTilemap(tmpob);

            Selection.activeGameObject = tmpob;
            EditorGUIUtility.PingObject(tmpob);
        }
    }

    /// <summary>
    /// NOTE : 타일팔레트의 ACTIVE TILEMAP을 설정하기 위함
    /// </summary>
    /// <param name="toselectedTilemap"></param>
    public static void SetActiveTilemap(GameObject toselectedTilemap)
    {
        foreach (var tilemap in FindObjectsOfType<Tilemap>())
        {
            if (tilemap != toselectedTilemap)
            {
                tilemap.gameObject.SetActive(false);
                EditorApplication.delayCall += () => tilemap.gameObject.SetActive(true);
            }
        }
    }
}
