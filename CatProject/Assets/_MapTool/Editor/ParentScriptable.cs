using UnityEditor;
using UnityEngine;

public class ParentScriptable : ScriptableObject
{

    const string PATH = "Assets/Editor/New ParentScriptable.asset";

    [SerializeField]
    ChildScriptable child;

    [MenuItem("Example/CreateTest")]
    static void CreateScripable()
    {
        var parent = ScriptableObject.CreateInstance<ParentScriptable>();

        parent.child = ScriptableObject.CreateInstance<ChildScriptable>();

        parent.child.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.CreateAsset(parent, PATH);

        AssetDatabase.AddObjectToAsset(parent.child, PATH);

        AssetDatabase.ImportAsset(PATH);
    }
}
