using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSliderSc : MonoBehaviour
{
    private Slider monsterHPSlider;
    
    
    private void Awake()
    {
        monsterHPSlider = GetComponent<Slider>();
    }

    public void SetSliderStartValue(int maxhp, int currenthp)
    {
        //체력 설정 
        monsterHPSlider.maxValue = maxhp;
        monsterHPSlider.value = currenthp;
    }
    
    /// <summary>
    /// NOTE : 체력바가 다를경우 업데이트 
    /// </summary>
    public void SetHPValue(int currenthp)
    {
            monsterHPSlider.value = currenthp;
    }
}
