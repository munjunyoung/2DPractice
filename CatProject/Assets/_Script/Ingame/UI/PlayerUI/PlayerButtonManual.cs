using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonManual : InputButtonManual
{
    protected CatType1 targetModel;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        targetModel = GameObject.Find("Player").GetComponent<CatType1>();
    }
}
