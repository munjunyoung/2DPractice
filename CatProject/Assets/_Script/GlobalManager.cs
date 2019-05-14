using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum Scene_Name { S_00StartScene, S_01Lobby, S_02StageSelect, S_03Ingame }
public class GlobalManager
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
    public PLAYER_TYPE pType;

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

    public void ExitGame()
    {
        Application.Quit();
    }

    //public IEnumerator LoadSceneCoroutine(Scene_Name _nextSceneName)
    //{
    //    Scene cs = SceneManager.GetActiveScene();
    //    var asyncOp = SceneManager.LoadSceneAsync(_nextSceneName.ToString(), LoadSceneMode.Additive);
    //    while (!asyncOp.isDone)
    //        yield return null;

    //    currentScene = SceneManager.GetSceneByName(_nextSceneName.ToString());
    //    SceneManager.SetActiveScene(currentScene);
    //    SceneManager.MoveGameObjectToScene(gameObject, currentScene);
    //    SceneManager.UnloadSceneAsync(cs);
    //}

    
}
