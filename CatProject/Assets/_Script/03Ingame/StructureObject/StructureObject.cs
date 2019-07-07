using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureObject : MonoBehaviour
{
    public DungeonRoom ownRoom = null;
    public SpriteRenderer ownSpRenderer;
    public Sprite[] spriteArray;
    public bool StateOn = false;
    public GameObject EffectOb;
    protected virtual void Awake()
    {
        ownSpRenderer = GetComponent<SpriteRenderer>();
    }
    
   
}
