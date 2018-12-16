using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(Player))]
public class InputButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Player Object")]
    public Player playerSc;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
}
