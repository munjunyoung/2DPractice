using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;
using System;

public class CSVDataReader
{
    private static CSVDataReader _instance = null;
    public static CSVDataReader instance
    {
        get
        {
            if (_instance == null)
                _instance = new CSVDataReader();
            return _instance;
        }
    }

    private readonly string DataPath = "Prefab/Character/CSVData/";

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    private Dictionary<string, Dictionary<string, string>> characterDataDic = new Dictionary<string, Dictionary<string, string>>();
    private Dictionary<string, Dictionary<string, string>> monsterDataDic = new Dictionary<string, Dictionary<string, string>>();
    
    private CSVDataReader()
    {
        characterDataDic = Read(DataPath + "CharacterData");
        monsterDataDic = Read(DataPath + "MonsterData");
        //.. MONSTER 엑셀도 완성되면 추가
    }

    /// <summary>
    /// NOTE : csv파일을 로드하여 해당 텍스트를 split하고 dictionary에 초기화
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private Dictionary<string, Dictionary<string, string>> Read(string path)
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

    /// <summary>
    /// NOTE : 엑셀 데이터 설정 자동화 (엑셀에 해당 변수 명에 맞게 추가하면 변경됨)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_data"></param>
    /// <param name="_datadic"></param>
    /// <param name="_typename"></param>
    public void SetData<T>(T _data, string _typename)
    {
        //해당 타입의 value값(value값또한 dic이므로) 처리
        Type tp = typeof(T);
        //타입에 따른 dictionary 변환
        Dictionary<string, Dictionary<string, string>> _datadic = tp.ToString().Equals("PlayerData") ? characterDataDic : monsterDataDic;
        //해당 타입의 내용이 들어가지 않을경우 리턴 
        if (!_datadic.ContainsKey(_typename))
        {
            Debug.Log("Load 오류");
            return;
        }
        else
        {
            //해당 클래스의 public 형식을 가진 모든 변수명들을 가져옴
            Dictionary<string, string> tmpdatadic = _datadic[_typename];
            FieldInfo[] variableStringdatas = tp.GetFields(BindingFlags.Instance | BindingFlags.Public);
            //가져온 field변수명 순회
            foreach (var vsd in variableStringdatas)
            {
                //field변수 명들이 datadic key값에 존재하는지 비교하여 처리
                if (tmpdatadic.ContainsKey(vsd.Name))
                {
                    //해당 클래스 데이터 처리
                    object typecheck = vsd.GetValue(_data);
                    //해당변수의 타입에 따라 처리
                    if (typecheck is int)
                        vsd.SetValue(_data, int.Parse(tmpdatadic[vsd.Name]));
                    else
                        vsd.SetValue(_data, float.Parse(tmpdatadic[vsd.Name]));

                }
            }
        }
    }
}

