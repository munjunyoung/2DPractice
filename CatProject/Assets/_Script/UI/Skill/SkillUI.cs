using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillUI : MonoBehaviour {

    private bool onClick;
    public RectTransform skillLinePanel;
    private Animator skillLineAnim;
    
    public List<RectTransform> skillList;
	// Use this for initialization
	void Start () {
        onClick = false;
        skillLineAnim = skillLinePanel.GetComponent<Animator>();
	}
	
    public void ClickSkillMenuButton()
    {
        onClick = onClick ? false : true;
        skillLineAnim.SetBool("onClick", onClick);
    }
}
