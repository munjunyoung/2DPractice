using UnityEditor;
using UnityEngine;

public class ExampleAsset : ScriptableObject
{
    [SerializeField]
    string str;

    [SerializeField, Range(0, 10)]
    int num;
    [MenuItem("Example/Create ExampleAsset Instance")]
    static void CreateExamplAssetInstance()
    {
        var exampleAsset = CreateInstance <ExampleAsset>();

        AssetDatabase.CreateAsset(exampleAsset, "Assets/Editor/ExampleAsset.asset");
        AssetDatabase.Refresh();
    }
}

