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
    public StageClick[] doors;
    
    private void Awake()
    {
        instance = this;
        selectStageAlarmPanel.SetActive(false);
    }

    /// <summary>
    /// NOTE : 알람 패널 실행, 스테이지 전환 알람
    /// </summary>
    /// <param name="level"></param>
    public void ShowSelectStageAlarmPanel(int level)
    {
        selectStageAlarmPanel.SetActive(true);
        selectStageAlarmText.text = "LEVEL " + level +" START?";
        selectStageLevel = level;
    }

    #region UI

    /// <summary>
    /// NOTE : 알람 Yes 버튼 클릭시 실행 (스테이지 이동)
    /// </summary>
    public void YesButtonInSelectStagePanel()
    {
        switch(selectStageLevel)
        {
            case 1:
                GlobalManager.instance.LoadScene(Scene_Name.S_03Ingame);
                break;
            default:
                break;
        }
        
    }

    /// <summary>
    /// NOTE : 알람 NO버튼 클릭시 실행 (창 닫음)
    /// </summary>
    public void NoButtonInSelectStagePanel()
    {
        selectStageAlarmPanel.SetActive(false);
        foreach (var d in doors)
            d.SetOriginalColor();
    }
    #endregion
}