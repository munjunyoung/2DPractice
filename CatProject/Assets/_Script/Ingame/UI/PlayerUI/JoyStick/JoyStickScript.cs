using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStickScript : PlayerButtonManual, IDragHandler
{
    [Header("JoyStick UI Reference")]
    [SerializeField]
    public RectTransform bgImg;
    
    private float bgImgDeltaValue;
    
    private Vector2 keyboardVector;
    private Vector2 stickVector;
    private bool onStick = false;

    private Vector2 inputVector;
    [HideInInspector]
    public Vector2 DirValue { get { return inputVector; } }

    protected override void Start()
    {
        base.Start();
        modelUIObject = GameObject.Find("JoyStickImage").GetComponent<Image>();
        currentColor = modelUIObject.color;

    }
    protected override void Update()
    {
        base.Update();
        SetInputVector();
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

            modelUIObject.GetComponent<RectTransform>().anchoredPosition = stickVector * (bgImg.sizeDelta * 0.5f);
        }
    }

    /// <summary>
    /// 누를경우 이미지 변경 , OnDrag 함수 실행
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        onStick = true;
        OnDrag(eventData);
        
    }

    /// <summary>
    /// 터치에서 뗄 시 스틱 이미지 복구 및 위치 앵커포지션 초기화
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        modelUIObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        onStick = false;
    }
    #endregion
    
    private float keyinputX;
    /// <summary>
    /// 키보드 입력값 처리 함수
    /// TODO : 리팩토링 가능성이 높음
    /// </summary>
    protected override void SetKeyBoard()
    {
        var a = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? -0.1f : 0;
        var d = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 0.1f : 0;

        if (!(a + d).Equals(0))
            keyinputX += (a + d);
        else
            keyinputX = 0;

        if (Mathf.Abs(keyinputX) > 1f)
            keyinputX = (a + d) > 0 ? 1f : -1f;
        
        keyboardVector = new Vector2(keyinputX, 0);

        if (!onStick)
            modelUIObject.GetComponent<RectTransform>().anchoredPosition = keyboardVector * (bgImg.sizeDelta * 0.5f);
    }

    /// <summary>
    /// Button On일경우 InputVector 값 설정
    /// </summary>
    public void SetInputVector()
    {
        inputVector = modelUIObject.GetComponent<RectTransform>().anchoredPosition / (bgImg.sizeDelta.x * 0.5f);
        targetModel.currentMoveInputValue = inputVector.x;
    }
}
