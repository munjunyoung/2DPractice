using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

class SaveObjectWindow : EditorWindow
{
    static string savefrontfolderPath = "Assets/resources/GeneratedMapData";
    
    string[] folderType = { "Terrain", "Puzzle", "Battle" };
    static int toolbarint = 0;

    static GameObject selectionOB;
    public GameObject saveOb = null;

    static Vector2 mousepos = Vector2.zero;

    public void OnEnable()
    {
        mousepos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
    }

    private void OnGUI()
    {
        //position 설정
        if(position.position != mousepos)
            position = new Rect(mousepos.x,mousepos.y, 320, 200);

        selectionOB = Selection.activeGameObject;
        //선택된 오브젝트가 없는경우
        
        if (selectionOB == null)
        {
            EditorUtility.DisplayDialog("Not Selection", "Not Selection.. Select GameObject in Hierarchy", "Yes");
            this.Close();
        }
        else
        {
            if (!selectionOB.activeInHierarchy)
            {
                EditorUtility.DisplayDialog("Other Selection", "Other Selection.. Select GameObject in Hierarchy", "Yes");
                this.Close();
            }
            if (selectionOB.GetComponent<Tilemap>() == null)
            {
                EditorUtility.DisplayDialog("Not Tilemap Object", "Not Tilemap Object, Select Tilemap Object in Hierarchy", "Yes");
                this.Close();
            }
            
            //이미 프리펩이 존재하는 파일인 경우
            if (PrefabUtility.GetPrefabInstanceStatus(selectionOB).Equals(PrefabInstanceStatus.Connected))
            {
                saveOb = (GameObject)EditorGUILayout.ObjectField(selectionOB, typeof(GameObject), true);

                GUILayout.Label(" 해당 오브젝트는 Load Prefab입니다.",MapToolWindow.titleFont);
                GUILayout.Space(30);
                toolbarint = GUILayout.Toolbar(toolbarint, folderType);
                GUILayout.BeginHorizontal("Box");

                //새것으로 저장
                if (GUILayout.Button("새로 파일 저장", GUILayout.Height(35)))
                {
                    //Prefab unpack
                    PrefabUtility.UnpackPrefabInstance(saveOb, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    SaveSameNameCondition(saveOb);
                }
                //기존 파일 저장(apply 프리펩)
                if(GUILayout.Button("기존 파일 저장", GUILayout.Height(35)))
                {
                    PrefabUtility.ApplyPrefabInstance(saveOb, InteractionMode.AutomatedAction);
                    EditorUtility.DisplayDialog("Apply Prefab", "Apply!", "OK");
                }

                if(GUILayout.Button("아니요", GUILayout.Height(35)))
                {
                    this.Close();
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                saveOb = (GameObject)EditorGUILayout.ObjectField(selectionOB, typeof(GameObject), true);

                GUILayout.Label("해당 오브젝트가 맞습니까?",MapToolWindow.titleFont);
                GUILayout.Label("(오브젝트 클릭하여 체크 가능)");
                GUILayout.Space(20);
                toolbarint = GUILayout.Toolbar(toolbarint, folderType);


                GUILayout.BeginHorizontal("Box");
                //저장 버튼
                if (GUILayout.Button("저장", GUILayout.Height(35)))
                {
                    SaveSameNameCondition(saveOb);
                }
                if (GUILayout.Button("아니요", GUILayout.Height(35)))
                {
                    this.Close();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
    
    /// <summary>
    /// NOTE : 같은 이름이 존재하는 파일 저장 관련 조건
    /// </summary>
    /// <param name="_saveOb"></param>
    private void SaveSameNameCondition(GameObject _saveOb)
    {
        //이름이 같은 파일이 있을경우 
        //파일체크 함수 관련
        var files = Resources.LoadAll<GameObject>("GeneratedMapData/" + folderType[toolbarint]);
        foreach (var f in files)
        {
            if (f.name == _saveOb.name)
            {
                EditorUtility.DisplayDialog("Same Name", "같은 이름이 존재하므로 기존이름에 '1'을 붙여 저장합니다.", "OK");
                _saveOb.name = saveOb.name + " 1";
                SavePrefabAndSelectAlarm(_saveOb);
                this.Close();

            }
        }

        SavePrefabAndSelectAlarm(_saveOb);
        this.Close();
    }

    /// <summary>
    /// NOTE : 저장 및 해당 저장된 파일을 SELECTION하고 PINGOBJECT를 통하여 알림
    /// </summary>
    /// <param name="_saveOb"></param>
    private void SavePrefabAndSelectAlarm(GameObject _saveOb)
    {

        var createdprefab = PrefabUtility.SaveAsPrefabAssetAndConnect(_saveOb, savefrontfolderPath + "/" + folderType[toolbarint] + "/" + _saveOb.name + ".prefab", InteractionMode.AutomatedAction);

        Selection.activeObject = createdprefab;
        EditorGUIUtility.PingObject(createdprefab);
    }
}
