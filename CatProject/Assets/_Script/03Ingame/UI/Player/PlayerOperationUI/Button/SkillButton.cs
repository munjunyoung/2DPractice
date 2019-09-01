using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : PlayerActionButton
{
    //Player SkillData
    private PlayerSkill skillData;
    [SerializeField]
    private Image skillCooltimeImage;
    [SerializeField]
    private Text skillCooltimeText;

    [SerializeField]
    private Color gColor, rColor;
    private bool canUseSkill = true;
    
    protected override void Start()
    {
        base.Start();
        skillCooltimeImage.gameObject.SetActive(false);
        SetImageSkillButton();
    }

    private void SetImageSkillButton()
    {
        skillData = playerSc.mySkill;
        if (!LoadDataManager.instance.skillSpriteDic.ContainsKey(skillData.GetType().ToString()))
        {
            Debug.Log("해당 스킬이미지가 존재하지 않습니다.");
            return;
        }
        GetComponent<Image>().sprite = LoadDataManager.instance.skillSpriteDic[skillData.GetType().ToString()];
    }

    protected override void SetKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ButtonClickDown();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            ButtonClickUp();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
            //스킬이 실행되었을경우 실행 스킬 지속 시간 및 쿨타임 실행
        if (playerSc.ExecuteSkillCheck())
            StartCoroutine(isRunningSkillImage(skillData));
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
    }

    /// <summary>
    /// NOTE : 버프형 스킬 실행중일때 버튼 이미지 fIllamount, 색상 변경 및 text 남은시간 표시
    /// </summary>
    /// <param name="_skilldata"></param>
    /// <returns></returns>
    IEnumerator isRunningSkillImage(PlayerSkill _skilldata)
    {
        playerSc.isRunningSkillCooltime = true;
        skillCooltimeText.gameObject.SetActive(true);
        skillCooltimeImage.gameObject.SetActive(true);
        skillCooltimeImage.fillAmount = 0;
        skillCooltimeImage.color = gColor;
        float currentTime = _skilldata.durationTime;
        while (currentTime>0)
        {
            currentTime -= Time.fixedDeltaTime;
            skillCooltimeText.text = "" + (int)currentTime;
            skillCooltimeImage.fillAmount += 1 * Time.deltaTime / _skilldata.durationTime;
            yield return new WaitForFixedUpdate();
        }
        skillCooltimeText.gameObject.SetActive(false);
        StartCoroutine(CoolTimeCoroutine(_skilldata.coolTime));
    }

    /// <summary>
    /// NOTE : 스킬 실행 이후 쿨타임 이미지 fillamont, 색상 변경 및 text 남은시간 표시
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator CoolTimeCoroutine(float time)
    {
        skillCooltimeImage.gameObject.SetActive(true);
        skillCooltimeText.gameObject.SetActive(true);
        skillCooltimeImage.fillAmount = 1;
        skillCooltimeImage.color = rColor;
        float currentTime = time;
        while (currentTime>0)
        {
            currentTime -= Time.fixedDeltaTime;
            skillCooltimeText.text = ""+(int)currentTime;
            skillCooltimeImage.fillAmount -= 1 * Time.deltaTime / time;
            yield return new WaitForFixedUpdate();
        }
        skillCooltimeImage.gameObject.SetActive(false);
        skillCooltimeText.gameObject.SetActive(false);
        playerSc.isRunningSkillCooltime = false;
    }
}
