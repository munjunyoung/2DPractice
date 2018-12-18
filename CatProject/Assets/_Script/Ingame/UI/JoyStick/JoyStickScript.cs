using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStickScript : InputButtonManager, IDragHandler
{
    [Header("JoyStick UI Reference")]
    [SerializeField]
    public RectTransform bgImg, joyStickImg;
    private float bgImgDeltaValue;
    
    private Vector2 keyboardVector;
    private Vector2 stickVector;

    private Vector2 inputVector;
    [HideInInspector]
    public Vector2 DirValue { get { return inputVector; } }
    
    bool onStick = false;

    private void Update()
    {
        InputKeyboard();
        SetStickValue();
    }

    #region joyStick
    /// <summary>
    /// Drag 
    /// </summary>
    /// <param name="pad"></param>
    public virtual void OnDrag(PointerEventData pad)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg, pad.position, pad.pressEventCamera, out stickVector))
        {
            stickVector.x = stickVector.x / (bgImg.sizeDelta.x * 0.5f);
            stickVector.y = 0;

            stickVector = (Mathf.Abs(stickVector.x) > 1f) ? stickVector.normalized : stickVector;
            
            joyStickImg.anchoredPosition = stickVector * (bgImg.sizeDelta * 0.5f);
        }
    }

    
    /// <summary>
    /// 누를경우 이미지 변경 , OnDrag 함수 실행
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        joyStickImg.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        onStick = true;
        OnDrag(eventData);
        
    }

    /// <summary>
    /// 터치에서 뗄 시 스틱 이미지 복구 및 위치 앵커포지션 초기화
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        joyStickImg.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        joyStickImg.anchoredPosition = Vector2.zero;
        onStick = false;
    }
    #endregion

    #region input Keyboard set

    private float keyinputX;
    /// <summary>
    /// 키보드 키 입력 관련 함수
    /// </summary>
    private void InputKeyboard()
    {
        var a = Input.GetKey(KeyCode.A) ? -0.1f : 0;
        var d = Input.GetKey(KeyCode.D) ? 0.1f : 0;

        if (!(a + d).Equals(0))
            keyinputX += (a + d);
        else
            keyinputX = 0;

        if (Mathf.Abs(keyinputX) > 1f)
            keyinputX = (a + d) > 0 ? 1f : -1f;

        keyboardVector = new Vector2(keyinputX, 0);

        if(!onStick)
            joyStickImg.anchoredPosition = keyboardVector * (bgImg.sizeDelta * 0.5f);
    }

    #endregion

    /// <summary>
    /// inputvector set
    /// </summary>
    private void SetStickValue()
    {
        inputVector = joyStickImg.anchoredPosition / (bgImg.sizeDelta.x*0.5f);
        playerSc.dirvalue = inputVector;

    }
}
