using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectSc : MonoBehaviour
{
    private Player playerSc;
    public GameObject[] hitEffectpullingArray;
    [HideInInspector]
    public int damage = 0;

    private void Awake()
    {
        playerSc = gameObject.transform.parent.GetComponent<Player>();
        damage = playerSc.pDATA.attackDamage;

        gameObject.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Monster"))
        {
            collision.transform.GetComponent<Monster>().TakeDamage(damage, collision.transform, collision.contacts[0].point);
            EffectOn(collision.contacts[0].point);
        }
    }

    private void EffectOn(Vector3 pos)
    {
        bool checkOn = false;
        int count = 0;
        while (!checkOn)
        {
            if(count!=0)
                count = count % hitEffectpullingArray.Length;
            //현재 체크하는 이펙트가 온일 경우 
            if (hitEffectpullingArray[count].activeSelf)
            {
                count++;
            }
            else
            {
                hitEffectpullingArray[count].transform.position = pos;
                hitEffectpullingArray[count].SetActive(true);
                checkOn = true;
            }
            
        }
    }
}
