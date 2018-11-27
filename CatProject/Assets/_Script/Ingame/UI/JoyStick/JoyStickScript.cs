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

    private Vector2 inputVector;
    public Vector2 DirValue { get { return inputVector; } }

    /// <summary>
    /// Drag 
    /// </summary>
    /// <param name="pad"></param>
    public virtual void OnDrag(PointerEventData pad)
    {
        Vector2 tmpPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg, pad.position, pad.pressEventCamera, out tmpPos))
        {
            //tmpPos를 bgImg 각 방향의 사이즈 값으로 나눈다
            tmpPos.x = tmpPos.x / bgImg.sizeDelta.x;
            tmpPos.y = 0;
            //tmpPos.y = tmpPos.y / bgImg.sizeDelta.y;
            inputVector = tmpPos;
            //벡터 길이가 1보다 클경우 normalized
            inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;

            joyStickImg.anchoredPosition = inputVector * (bgImg.sizeDelta / 2f);
        }
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
