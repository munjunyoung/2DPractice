using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    //Scene전환
    public void StartButtonFunc()
    {
        GlobalManager.instance.LoadScene(Scene_Name.S_01Lobby);
    }

    public void ExitButtonFunc()
    {
        Application.Quit();
    }
}
