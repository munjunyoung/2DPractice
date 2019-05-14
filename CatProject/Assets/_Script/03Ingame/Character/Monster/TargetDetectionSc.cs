using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectionSc : MonoBehaviour
{
    private Monster parentOb;
    private void Awake()
    {
        parentOb = transform.parent.GetComponent<Monster>();
    }
    /// <summary>
    /// NOTE : 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parentOb.OrderState.Equals(ORDER_STATE.Trace))
            return;

        if (collision.CompareTag("Player"))
        {
            parentOb.OrderState = ORDER_STATE.Trace;
            parentOb.targetOb = collision.transform;
        }
    }
}
