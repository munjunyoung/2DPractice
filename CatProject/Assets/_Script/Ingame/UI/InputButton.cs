using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    protected Image modelObjectImage;
    protected Color currentColor;

    protected  virtual void Start()
    {
        modelObjectImage = GetComponent<Image>() ? GetComponent<Image>() : modelObjectImage;
        currentColor = modelObjectImage.color;
    }

    protected virtual void Update()
    {
        SetKeyBoard();
    }

    public virtual void OnPointerDown(PointerEventData eventData){ ButtonClickDown(); }
    public virtual void OnPointerUp(PointerEventData eventData){ ButtonClickUp(); }
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
        modelObjectImage.color = currentColor;
    }
    /// <summary>
    /// 버튼이 떼어졌을 경우 처리
    /// </summary>
    protected virtual void ButtonClickUp()
    {
        currentColor.a = 1f;
        modelObjectImage.color = currentColor;
    }
}
