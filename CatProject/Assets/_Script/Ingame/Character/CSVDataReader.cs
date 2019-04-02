using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;
using System;

public class CSVDataReader : MonoBehaviour
{
    private readonly string characterPath = "Character/Data/CharacterData";

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    Dictionary<string, Dictionary<string, string>> characterDataDic = new Dictionary<string, Dictionary<string, string>>();
    private void Start()
    {
       characterDataDic = Read(characterPath);

        //Test
        foreach (var t in characterDataDic["Cat1"])
            Debug.Log(t);
    }

    /// <summary>
    /// NOTE : csv파일을 로드하여 해당 텍스트를 split하고 dictionary에 초기화
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Dictionary<string, Dictionary<string, string>> Read(string path)
    {
        Dictionary<string, Dictionary<string, string>> tmplist = new Dictionary<string, Dictionary<string, string>>();
        TextAsset textdata = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(textdata.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
            return null;
        //Line 0번은 데이터들의 종류들 문자열 값
        var dataheader = Regex.Split(lines[0], SPLIT_RE);
        //공백 제거
        for (int i = 0; i < dataheader.Length; i++)
            dataheader[i] = dataheader[i].Trim();

        //Line 1부터 실행
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            var data = new Dictionary<string, string>();
            for (int j = 1; j < dataheader.Length; j++)
            {
                //공백 제거
                values[j] = values[j].Trim();
                data.Add(dataheader[j], values[j]);
            }
            tmplist.Add(values[0], data);
        }
        return tmplist;
    }

    public void Test()
    {
        Type tp = typeof(PlayerData);
        //해당 값들을 가져옴
        FieldInfo[] flds = tp.GetFields(BindingFlags.Public);
        PlayerData testData = new PlayerData();
        testData.maxHP.ToString();
    }
}
