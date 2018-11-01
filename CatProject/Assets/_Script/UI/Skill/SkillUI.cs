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
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClickSkillMenuButton()
    {
        onClick = onClick ? false : true;
        skillLineAnim.SetBool("onClick", onClick);
    }

    public void ClickSkill1Button()
    {

    }

    public void ClickSkill2Button()
    {

    }

    public void ClickSkill3Button()
    {

    }
    
}
