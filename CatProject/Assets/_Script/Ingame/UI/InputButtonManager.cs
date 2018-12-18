using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Player Object")]
    protected Player playerSc;

    private void Start()
    {
        playerSc = GameObject.Find("Player").GetComponent<Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData) { }

    public virtual void OnPointerUp(PointerEventData eventData) { }
}
