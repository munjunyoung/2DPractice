using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHitEffectSc : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(SetActiveOffByCount(0.5f));
    }

    IEnumerator SetActiveOffByCount(float count)
    {
        yield return new WaitForSeconds(count);
        gameObject.SetActive(false);
    }
}
