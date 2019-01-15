using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStickSc : PlayerActionButton, IDragHandler
{
    [Header("JoyStick UI Reference")]
    [SerializeField]
    public RectTransform bgImg;

    private bool stickOn = false;
    private Vector2 stickVector;
    private float keyinputX = 0;

    [HideInInspector]
    public Vector2 inputVector;

    #region joyStick
    /// <summary>
    /// Drag
    /// </summary>
    /// <param name="pad"></param>
    public virtual void OnDrag(PointerEventData pad)
    {
        //localpoint가 vector이므로 x값만 사용하지만 stickvector로 선언하여 사용
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg, pad.position, pad.pressEventCamera, out stickVector))
        {
            stickVector.x = stickVector.x / (bgImg.sizeDelta.x * 0.5f);
            stickVector.y = 0;
            stickVector = (Mathf.Abs(stickVector.x) > 1f) ? stickVector.normalized : stickVector;

            modelObjectImage.GetComponent<RectTransform>().anchoredPosition = stickVector * (bgImg.sizeDelta * 0.5f);
        }
    }

    /// <summary>
    /// NOTE : 누를경우 이미지 변경 , OnDrag 함수 실행
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        stickOn = true;
        OnDrag(eventData);   
    }

    /// <summary>
    /// NOTE : 터치에서 뗄 시 스틱 이미지 복구 및 위치 앵커포지션 초기화
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        stickOn = false;
        modelObjectImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    #endregion
    
    /// <summary>
    /// NOTE : 키보드 입력값 처리 함수
    /// NOTE : 키보드 값을 누를때 바로 1을 초기화하는게 아니라 프레임마다 0.1F값이 + 됨
    /// TODO : 리팩토링 가능성이 높음
    /// </summary>
    protected override void SetKeyBoard()
    {
        if (!stickOn)
        {
            float a = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? -0.1f : 0;
            float d = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 0.1f : 0;

            keyinputX = !(a + d).Equals(0) ? keyinputX + (a + d) : 0;

            if (!a.Equals(0.0f) || !d.Equals(0.0f))
                ButtonClickDown();
            else
                ButtonClickUp();
            //키보드 플레이어 가속 설정
            if (Mathf.Abs(keyinputX) > 1f)
                keyinputX = (a + d) > 0 ? 1f : -1f;

            Vector2 keyboardVector = new Vector2(keyinputX, 0);

            modelObjectImage.GetComponent<RectTransform>().anchoredPosition = keyboardVector * (bgImg.sizeDelta * 0.5f);
        }
         SetInputVector();
    }

    protected override void ButtonClickDown()
    {
        base.ButtonClickDown();
        playerSc.MoveOn = true;
    }

    protected override void ButtonClickUp()
    {
        base.ButtonClickUp();
        playerSc.MoveOn = false;
    }

    /// <summary>
    /// Button On일경우 InputVector 값 설정
    /// </summary>
    private void SetInputVector()
    {
        inputVector = modelObjectImage.GetComponent<RectTransform>().anchoredPosition / (bgImg.sizeDelta.x * 0.5f);
        playerSc.currentMoveInputValue = inputVector.x;
    }
}
