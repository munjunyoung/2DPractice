using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputButtonManual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected Image modelUIObject;
    protected Color currentColor;

    protected  virtual void Start()
    {
        modelUIObject = GetComponent<Image>() ? GetComponent<Image>() : modelUIObject;
        currentColor = modelUIObject.color;
    }

    protected virtual void Update()
    {
        SetKeyBoard();
    }

    public virtual void OnPointerDown(PointerEventData eventData){ ButtonClickDown(); }

    public virtual void OnPointerUp(PointerEventData eventData){ ButtonClickOn(); }
    /// <summary>
    /// 키보드 입력값 처리
    /// </summary>
    protected virtual void SetKeyBoard() { }
    /// <summary>
    /// 버튼이 눌렸을 경우 처리
    /// </summary>
    protected virtual void ButtonClickDown()
    {
        currentColor.a = 0.5f;
        modelUIObject.color = currentColor;
    }
    /// <summary>
    /// 버튼이 떼어졌을 경우 처리
    /// </summary>
    protected virtual void ButtonClickOn()
    {
        currentColor.a = 1f;
        modelUIObject.color = currentColor;
    }
}
