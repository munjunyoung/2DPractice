using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

class TestSettingWindow : EditorWindow
{
    static Vector2 mousepos = Vector2.zero;
    static int buttonSelectionValue = 0;

    static MapSetting mapsettinginstance;

    //MapSetting 관련
    private static int mapWidth;
    private static int mapHeight;
    private static int selectedTypeMap;
    private static bool backgroundOn = true;

    private static GameObject playerOb;
    private enum PLAYER_TYPE { Cat1, Dog };
    private string playerPath = "Assets/resources/Prefab/Character/Player/";
    private Vector2 characterpos;
    private int selectedTypePlayer;

    private enum MONSTER_TYPE { Fox };
    private string monsterPath = "Assets/resources/Prefab/Character/Monster/";
    private Vector2 monsterPos;
    private int selectedTypeMonster;
    private int numberOfMonster;
    private bool randomPosOn = false;

    private void OnEnable()
    {
        if (GUIUtility.GUIToScreenPoint(Event.current.mousePosition)!=null)
            mousepos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
    }
    /// <summary>
    /// NOTE : 기본 땅 설정, 캐릭터 생성, 적 생성
    /// </summary>
    private void OnGUI()
    {

        if (position.position != mousepos)
            position = new Rect(mousepos.x, mousepos.y, 500, 300);

        GUILayout.Space(5);
        GUILayout.BeginHorizontal("Box");
        GUILayout.BeginArea(new Rect(0, 0, 110, 300));
        GUILayout.BeginVertical("Box");

        if (GUILayout.Button("Map Size", GUILayout.Width(95), GUILayout.Height(95)))
        {
            buttonSelectionValue = 0;
        }

        if (GUILayout.Button("Create Player", GUILayout.Width(95), GUILayout.Height(95)))
        {
            buttonSelectionValue = 1;
        }

        if (GUILayout.Button("Create Enemy", GUILayout.Width(95), GUILayout.Height(95)))
        {
            buttonSelectionValue = 2;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(120, 0, 380, 300));
        
        switch(buttonSelectionValue)
        {
            case 0:
                SetMapSize();
                break;
            case 1:
                CreatePlayer();
                break;
            case 2:
                CreateMonster();
                break;
        }

        GUILayout.EndArea();
        GUILayout.EndHorizontal();
        
    }


    /// <summary>
    /// NOTE : Map Size Setting Function GUI
    /// </summary>
    private void SetMapSize()
    {
        GUILayout.Space(10);
        GUILayout.Label("Map Size Setting", MapToolWindow.titleFont);
        GUILayout.BeginHorizontal("Box");
        GUILayout.Label("Width");
        mapWidth = EditorGUILayout.IntSlider(mapWidth, 10, 200);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal("Box");
        GUILayout.Label("Height");
        mapHeight = EditorGUILayout.IntSlider(mapHeight, 10, 200);
        GUILayout.EndVertical();
        GUILayout.Label("Map Type", MapToolWindow.titleFont);
        selectedTypeMap = GUILayout.Toolbar(selectedTypeMap, new string[] { "Type1", "Type2" });
        GUILayout.Space(10);
        GUILayout.Label("BackGround Exist", MapToolWindow.titleFont);
        backgroundOn = GUILayout.Toggle(backgroundOn, backgroundOn ? "ON" : "OFF", "button");

        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Change",GUILayout.Height(35)))
        {
            if(mapsettinginstance==null)
                mapsettinginstance = new MapSetting();

            mapsettinginstance.CreateNormalMap(selectedTypeMap, mapWidth, mapHeight, backgroundOn);
        }
        
        GUILayout.Space(10);
    }

    /// <summary>
    /// NOTE : Create Player setting Function GUI
    /// </summary>
    private void CreatePlayer()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        characterpos = EditorGUILayout.Vector2Field("Character Position", characterpos);
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        GUILayout.Label("CAT TYPE", MapToolWindow.titleFont);
        selectedTypePlayer = GUILayout.Toolbar(selectedTypePlayer, new string[] { PLAYER_TYPE.Cat1.ToString(), PLAYER_TYPE.Dog.ToString()});
        GUILayout.Label("추후 추가예정..");
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        
        if(GUILayout.Button("CREATE", GUILayout.Height(35)))
        {
            playerOb = GameObject.FindGameObjectWithTag("Player");

            //추후에 여러 플레이어 캐릭터가 존재할경우 변경 해야함 
            if (playerOb != null)
                DestroyImmediate(playerOb);

            PLAYER_TYPE pType = (PLAYER_TYPE)selectedTypePlayer;
            var tmpplayerob = AssetDatabase.LoadAssetAtPath(playerPath + pType.ToString() + ".prefab",typeof(Object));
            playerOb = PrefabUtility.InstantiatePrefab(tmpplayerob) as GameObject;
            playerOb.transform.position = characterpos;

            Selection.activeGameObject = playerOb;
            EditorGUIUtility.PingObject(playerOb);

        }
        GUILayout.Space(10);
    }

    /// <summary>
    /// NOTE : Create Enemy setting Function GUI
    /// </summary>
    private void CreateMonster()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        monsterPos = EditorGUILayout.Vector2Field("Monster Position", monsterPos);
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Monster Type", MapToolWindow.titleFont);
        selectedTypeMonster = GUILayout.Toolbar(selectedTypeMonster, new string[] { MONSTER_TYPE.Fox.ToString() });
        GUILayout.Label("추후 추가예정..");
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("CREATE", GUILayout.Height(35)))
        {
            MONSTER_TYPE mType = (MONSTER_TYPE)selectedTypeMonster;
            Object tmpmonsterob = AssetDatabase.LoadAssetAtPath(monsterPath + mType.ToString() + ".prefab", typeof(Object));
            GameObject insmonsterob = PrefabUtility.InstantiatePrefab(tmpmonsterob) as GameObject;
            insmonsterob.transform.position = monsterPos;

            Selection.activeGameObject = insmonsterob;
            EditorGUIUtility.PingObject(insmonsterob);
        }
        GUILayout.Space(10);
    }
}

