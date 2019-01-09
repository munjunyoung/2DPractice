using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputButtonManual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public virtual void OnPointerDown(PointerEventData eventData) { }

    public virtual void OnPointerUp(PointerEventData eventData) { }

    public virtual void ButtonOn() { }
}
