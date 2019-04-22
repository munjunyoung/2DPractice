using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum Scene_Name { Lobby, StageSelect, Level1}
public class GlobalManager : MonoBehaviour
{

    private Scene currentScene;
    public static GlobalManager instance;
    [HideInInspector]
    public PLAYER_TYPE pType;
    [HideInInspector]
        

    private void Awake()
    {
        instance = this;
    }
    
    public void LoadScene(Scene_Name _sname)
    {
        StartCoroutine(LoadSceneCoroutine(_sname));
    }

    public IEnumerator LoadSceneCoroutine(Scene_Name _nextSceneName)
    {
        Scene cs = SceneManager.GetActiveScene();
        var asyncOp = SceneManager.LoadSceneAsync(_nextSceneName.ToString(), LoadSceneMode.Additive);
        while (!asyncOp.isDone)
            yield return null;

        currentScene = SceneManager.GetSceneByName(_nextSceneName.ToString());
        SceneManager.MoveGameObjectToScene(gameObject, currentScene);
        
        SceneManager.UnloadSceneAsync(cs);
    }
}
