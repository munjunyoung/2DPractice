using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPSliderSc : MonoBehaviour
{
    private Slider enemyHPSlider;
    
    
    private void Awake()
    {
        enemyHPSlider = GetComponent<Slider>();
    }

    public void SetSliderStartValue(int maxhp, int currenthp)
    {
        //체력 설정 
        enemyHPSlider.maxValue = maxhp;
        enemyHPSlider.value = currenthp;
    }
    
    /// <summary>
    /// NOTE : 체력바가 다를경우 업데이트 
    /// </summary>
    public void SetHPValue(int currenthp)
    {
            enemyHPSlider.value = currenthp;
    }
}
