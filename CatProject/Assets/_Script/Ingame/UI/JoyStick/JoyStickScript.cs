using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStickScript : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("JoyStick UI Reference")]
    public RectTransform bgImg;
    public RectTransform joyStickImg;

    private Vector2 stickPos;
    private Vector2 inputVector;
    public Vector2 DirValue { get { return inputVector; } }

    private void Update()
    {
        //SetStickValue();
    }
    
    /// <summary>
    /// Drag 
    /// </summary>
    /// <param name="pad"></param>
    public virtual void OnDrag(PointerEventData pad)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg, pad.position, pad.pressEventCamera, out stickPos))
        {
            //tmpPos를 bgImg 각 방향의 사이즈 값으로 나눈다
            stickPos.x = stickPos.x / bgImg.sizeDelta.x;
            stickPos.y = 0;
            //벡터 길이가 1보다 클경우 normalized
            stickPos = (stickPos.magnitude > 1f) ? stickPos.normalized : stickPos;

            joyStickImg.anchoredPosition = stickPos * (bgImg.sizeDelta / 2f);
            inputVector = stickPos;
        }
    }

    private void SetStickValue()
    {
        inputVector = stickPos; 
     //   inputVector = tmpPos;
     //   inputVector = inputVector.normalized;
    }

    /// <summary>
    /// 터치 다운시 이미지 변경 및 드래그 시작
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        joyStickImg.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        OnDrag(eventData);
    }

    /// <summary>
    /// 터치에서 뗄 시 스틱 이미지 복구 및 위치 앵커포지션 초기화
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        joyStickImg.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        inputVector = Vector2.zero;
        joyStickImg.anchoredPosition = Vector2.zero;
    }
}
