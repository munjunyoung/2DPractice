using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonManual : InputButtonManual
{
    protected Player playerSc;

    // Start is called before the first frame update
    void Start()
    {
        playerSc = GameObject.Find("Player").GetComponent<Player>();
    }
}
