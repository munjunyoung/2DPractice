using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    //싱글턴
    public static LobbyManager instance = null;
    private PLAYER_TYPE pType;
    public GameObject selectCompeletePanel;
    public Text selectCompeleteText;

    private void Awake()
    {
        instance = this;

        selectCompeletePanel.SetActive(false);
    }
 
    public void ActiveSelectCompletePanel(PLAYER_TYPE _pType)
    {
        selectCompeletePanel.SetActive(true);
        selectCompeleteText.text = "'" + _pType.ToString() + "'를 시작 ?";
        pType = _pType;
    }
    
    public void YesButtonInSelectCompleteUI()
    {
        LoadStageScene();
    }

    public void NoButtonInSelectCompleteUI()
    {
        
        selectCompeletePanel.SetActive(false);
    }

    public void LoadStageScene()
    {
        GlobalManager.instance.pType = pType;
        GlobalManager.instance.LoadScene(Scene_Name.StageSelect);
    }
    
}
