using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectSceneManager : MonoBehaviour
{
    //싱글턴
    public static StageSelectSceneManager instance = null;

    public int selectStageLevel = -1;
    [SerializeField]
    private GameObject selectStageAlarmPanel;
    [SerializeField]
    public Text selectStageAlarmText;


    private void Awake()
    {
        instance = this;
        selectStageAlarmPanel.SetActive(false);
    }
    public void ShowSelectStageAlarmPanel(int level)
    {
        selectStageAlarmPanel.SetActive(true);
        selectStageAlarmText.text = level + "레벨 방을 시작 ?";
        selectStageLevel = level;
    }

    #region UI

    public void YesButtonInSelectStagePanel()
    {
        switch(selectStageLevel)
        {
            case 1:
                GlobalManager.instance.LoadScene(Scene_Name.Level1);
                break;
            default:
                break;
        }
        
    }

    public void NoButtonInSelectStagePanel()
    {
        selectStageAlarmPanel.SetActive(false);
    }
    #endregion
}