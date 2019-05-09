using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Alarm_State { DIE, CLEAR };
public class PlayerUIManager : MonoBehaviour
{
    /// <summary>
    /// PLAYER
    /// </summary>
    private Player playerSc;
    [Header("PLAYER HP/TP UI")]
    [SerializeField]
    private Slider playerHPSlider;
    [SerializeField]
    private Slider playerTPSlider;
    [SerializeField]
    private Text playerHPText, playerTPText;
 
    [SerializeField]
    private GameObject AlarmPanel;
    [SerializeField]
    private Text AlarmText;
    [SerializeField]
    private Button YesButton;

    [SerializeField]
    private Text ItemNumberText;
    
    private void Start()
    {
        //player hp/tp ui start
        PlayerPointUIStart();

        AlarmPanel.SetActive(false);
    }
    
    #region PLAYER HP TP
    /// <summary>
    /// NOTE : 플레이어 HP TP UI 업데이트 시작
    /// </summary>
    private void PlayerPointUIStart()
    {
        playerSc = GameObject.FindWithTag("Player").GetComponent<Player>();

        playerHPSlider.maxValue = playerSc.pDATA.maxHP;
        playerTPSlider.maxValue = playerSc.pDATA.maxTP;
        SetHPUI(playerSc.CurrentHP);
        SetTPUI(playerSc.CurrentTP);
        StartCoroutine(PointSetUpdate());
        
        SetCatnipItemNumberText();
    }
    /// <summary>
    /// NOTE : HP UI UPDATE
    /// </summary>
    private void SetHPUI(int hp)
    {
        if (playerHPSlider.value.Equals(hp))
            return;

        playerHPSlider.value = hp;
        playerHPText.text = hp.ToString();
    }

    /// <summary>
    /// NOTE : TP UI UPDATE
    /// </summary>
    private void SetTPUI(int tp)
    {
        if (playerTPSlider.value.Equals(tp))
            return;

        playerTPSlider.value = tp;
        playerTPText.text = tp.ToString();
    }

    /// <summary>
    /// NOTE : 플레이어가 살아있는 동안 반복
    /// </summary>
    /// <returns></returns>
    IEnumerator PointSetUpdate()
    {
        while (playerSc.isAlive)
        {
            SetHPUI(playerSc.CurrentHP);
            SetTPUI(playerSc.CurrentTP);
            yield return new WaitForSeconds(0.1f);
        }
        SetHPUI(0);
        SetTPUI(0);
    }
    #endregion

    public void ShowAlaramPanel(Alarm_State _state)
    {
        switch(_state)
        {
            case Alarm_State.CLEAR:
                AlarmText.text = "Congratulation!";
                YesButton.onClick.AddListener(() => { GlobalManager.instance.LoadScene(Scene_Name.StageSelect);});
                break;
            case Alarm_State.DIE:
                AlarmText.text = "YOU DIE..";
                YesButton.onClick.AddListener(() => { GlobalManager.instance.LoadScene(Scene_Name.Lobby); });
                break;
        }
        AlarmPanel.SetActive(true);
    }

    /// <summary>
    /// NOTE : ITEM NUMBER TEXT 설정
    /// </summary>
    public void SetCatnipItemNumberText()
    {
        ItemNumberText.text = playerSc.catnipItemNumber.ToString();
    }
}


///// <summary>
///// ENEMY HP 오브젝트 풀링
///// </summary>
//private List<Slider> enemyHPSliderList = new List<Slider>();
//[Header("ENEMY HP UI")]
//[SerializeField]
//private GameObject enemySliderParent;
//[SerializeField]
//private Slider enemyHPSliderPrefab;

//private List<Monster> currentMonsterList = new List<Monster>();
//#region ENEMY HP
///// <summary>
///// NOTE : 방을 순회하여 가장 몬스터가 많은 숫자만큼 생성
///// </summary>
//public void CreateEnemyHPSlider(int numberofslider)
//{
//    for (int i = 0; i < numberofslider; i++)
//    {
//        Slider tmpslider = Instantiate(enemyHPSliderPrefab, Vector2.zero, Quaternion.identity, enemySliderParent.transform);
//        enemyHPSliderList.Add(tmpslider);
//        tmpslider.gameObject.SetActive(false);
//    }
//}

///// <summary>
///// NOTE : 방이 변경되거나 시작할때 적 HP 슬라이더 설정
///// </summary>
///// <param name="monsters"></param>
//public void OnEnemySlider(List<Monster> monsters)
//{
//    currentMonsterList = monsters;
//    for (int i = 0; i < enemyHPSliderList.Count ; i++)
//    {
//        if (i < monsters.Count)
//        {
//            enemyHPSliderList[i].gameObject.SetActive(true);
//            enemyHPSliderList[i].maxValue = monsters[i].mDATA.maxHP;
//            enemyHPSliderList[i].value = monsters[i].CurrentHP;
//        }
//        else
//            enemyHPSliderList[i].gameObject.SetActive(false);
//    }
//}

///// <summary>
///// NOTE : 적들의 HP 바 Position 업데이트
///// </summary>
//private void UpdateSliderPos()
//{
//    for (int i = 0; i < currentMonsterList.Count; i++)
//    {
//        Vector3 targetpos = Camera.main.WorldToScreenPoint(currentMonsterList[i].transform.position);
//        enemyHPSliderList[i].transform.position = targetpos + Vector3.up*25f;
//    }
//}

///// <summary>
///// 
///// </summary>
///// <param name="monster"></param>
//public void SetEnemyHPSliderValue(Monster monster)
//{
//    enemyHPSliderList[monster.monsterNumberinRoom].value = monster.CurrentHP;
//}

//#endregion