using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackEffectSc : MonoBehaviour
{
    [SerializeField]
    private     GameObject[]    hitEffectPullingArray;
    private     int             effectCount = 0;
    [HideInInspector]
    public      int             damage = 0;
    // Start is called before the first frame update
    
    public void SetActiveOff()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(SetActiveOffByCount(0.5f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var point = new Vector2((transform.position.x + collision.transform.position.x) * 0.5f, (transform.position.y + collision.transform.position.y) * 0.5f);
            collision.transform.GetComponent<Player>().TakeDamage(damage, point);
            PlayPoolingHitEffect(point);
        }
        
    }
    
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.transform.CompareTag("Player"))
    //        //collision.transform.GetComponent<Player>().TakeDamage(damage, transform);

    //    PlayPoolingHitEffect(collision.contacts[0].point);
    //}

    private void PlayPoolingHitEffect(Vector3 _hitpos)
    {
        if (effectCount != 0)
            effectCount = effectCount % hitEffectPullingArray.Length;

        //현재 체크하는 이펙트가 온일 경우 
        hitEffectPullingArray[effectCount].transform.position = _hitpos;
        hitEffectPullingArray[effectCount].SetActive(true);
        effectCount++;
    }

    /// <summary>
    /// NOTE : ActiveOff;
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnumerator SetActiveOffByCount(float count)
    {
        yield return new WaitForSeconds(count);
        gameObject.SetActive(false);
    }
}
