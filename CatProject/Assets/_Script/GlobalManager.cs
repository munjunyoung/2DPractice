using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum Scene_Name { S_00StartScene, S_01Lobby, S_02StageSelect, S_03Ingame }
public class GlobalManager : MonoBehaviour
{

    private Scene currentScene;
    private static GlobalManager _instance = null;
    public static GlobalManager instance
    {
        get
        {
            if(_instance==null)
            {
                _instance = new GlobalManager();
            }
            return _instance;
        }
    }
    
    [HideInInspector]
    public PLAYER_TYPE pType = PLAYER_TYPE.Cat1;
    public int stageLevel = -1;


    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }
    
    public void LoadScene(Scene_Name _sname)
    {
        SceneManager.LoadScene(_sname.ToString());
        //StartCoroutine(LoadSceneCoroutine(_sname));
        //Scene cs = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(_sname.ToString(), LoadSceneMode.Additive);
        //Scene ns = SceneManager.GetSceneByName(_sname.ToString());
        //SceneManager.MoveGameObjectToScene(gameObject, ns);
        //SceneManager.SetActiveScene(ns);
    }
}
