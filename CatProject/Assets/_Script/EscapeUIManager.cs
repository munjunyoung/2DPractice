using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeUIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject EscapePanel;
    private bool isPause = false;

    [SerializeField]
    private GameObject OptionPanel;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        OptionPanel.SetActive(false);
        EscapePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowEscapePanel();
    }

    /// <summary>
    /// NOTE : Escape 패널이 켜져있으면 꺼지고 꺼져있으면 켜짐, 또한 TimeScale을 0으로 만듦으로써 일시정지 기능
    /// </summary>
    public void ShowEscapePanel()
    {
        //ispause를 기준으로 변경
            isPause = isPause ? false : true;
            Time.timeScale = isPause ? 0 : 1;
            EscapePanel.SetActive(isPause);
    }

    /// <summary>
    /// NOTE : OPTION 패널 관련
    /// </summary>
    public void OptionButton()
    {
        if (OptionPanel.activeSelf)
            OptionPanel.SetActive(false);
        else
            OptionPanel.SetActive(true);
    }

    public void ApplyInOptionPanel()
    {
        OptionPanel.SetActive(false);
    }

    public void BackInOptionPanel()
    {
        OptionPanel.SetActive(true);
    }

    /// <summary>
    /// NOTE : 게임종료
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
