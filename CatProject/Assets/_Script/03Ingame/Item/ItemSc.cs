using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSc : MonoBehaviour
{
    protected bool isStop = false;
    protected int catnipamount;
    
    public void StopAction(float _stopcount)
    {
        StartCoroutine(StopCoroutine(_stopcount));
    }

    protected IEnumerator StopCoroutine(float _stopcount)
    {
        isStop = true;
        yield return new WaitForSeconds(_stopcount);
        isStop = false;
    }

}
