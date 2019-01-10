using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerButtonManual : InputButtonManual
{
    protected CatType1 playerSc;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerSc = GameObject.Find("Player").GetComponent<CatType1>();
    }
}
