using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어 객체 참조가 필요하여 분리
/// </summary>
public class PlayerActionButton : InputButton
{
    protected Player playerSc = null;

    protected override void Start()
    {
        base.Start();
        playerSc = GameObject.FindWithTag("Player").GetComponent<CatType1>();
    }

    protected override void Update()
    {
        if (playerSc == null)
            return;
        base.Update();
    }
}
