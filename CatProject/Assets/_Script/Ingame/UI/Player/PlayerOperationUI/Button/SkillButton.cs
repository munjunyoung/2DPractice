using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillButton : PlayerActionButton
{
    private int skillCooltime = 15;
    [SerializeField]
    private Image skillCooltimeImage;
    [SerializeField]
    private Text skillCooltimeText;

    private bool canUseSkill = true;

    protected override void Start()
    {
        base.Start();
        skillCooltimeImage.gameObject.SetActive(false);
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
        if (!playerSc.isRunningSkill&&canUseSkill)
        {
            playerSc.OnSkill();
            StartCoroutine(CoolTimeCoroutine(skillCooltime));
        }
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
    }

    IEnumerator CoolTimeCoroutine(int time)
    {
        skillCooltimeImage.gameObject.SetActive(true);
        skillCooltimeImage.fillAmount = 1;
        float currentTime = time;
        while (currentTime>0)
        {
            currentTime -= Time.fixedDeltaTime;
            skillCooltimeText.text = ""+(int)currentTime;
            skillCooltimeImage.fillAmount -= 1 * Time.deltaTime / skillCooltime;
            yield return new WaitForFixedUpdate();
        }
        skillCooltimeImage.gameObject.SetActive(false);
    }
}
