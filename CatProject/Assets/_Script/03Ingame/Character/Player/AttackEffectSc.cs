using UnityEngine;

public class AttackEffectSc : MonoBehaviour
{
    private Player playerSc;
    public GameObject[] hitEffectpullingArray;
    private int effectCount = 0;
    [HideInInspector]
    public int damage = 0;

    private void Start()
    {
        playerSc = gameObject.transform.parent.GetComponent<Player>();
        damage = playerSc.pDATA.attackDamage;
        gameObject.SetActive(false);
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    bool effectOn = true;
    //    var hitpos = (transform.position + collision.transform.position) * 0.5f;
    //    switch (collision.transform.tag)
    //    {
    //        case "Monster":
    //            collision.transform.GetComponent<Monster>().TakeDamage(damage, playerSc.transform, hitpos);
    //            break;
    //        case "Box":
    //        case "Garbage":
    //            collision.transform.GetComponent<Rb2dStructureSc>().TakeThis(hitpos);
    //            break;
    //        default:
    //            effectOn = false;
    //            break;
    //    }
    //    if(effectOn)
    //        PlayHitEffect(hitpos);
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool effectOn = true;
        switch (collision.transform.tag)
        {
            case "Monster":
                collision.transform.GetComponent<Monster>().TakeDamage(damage, playerSc.transform, collision.contacts[0].point);
                break;
            case "Box":
            case "Garbage":
                collision.transform.GetComponent<Rb2dStructureSc>().TakeThis(collision.contacts[0].point);
                break;
            case "Boss":
                collision.transform.GetComponent<BossMonsterController>().TakeDamage(damage);
                break;
            default:
                effectOn = false;
                break;
        }
        if(effectOn)
            PlayHitEffect(collision.contacts[0].point);
    }
    /// <summary>
    /// NOTE : effect포지션과 target포지션 중앙 설정
    /// </summary>
    /// <param name="_hitpos"></param>
    private void PlayHitEffect(Vector3 _hitpos)
    {
        if (effectCount != 0)
            effectCount = effectCount % hitEffectpullingArray.Length;

        Debug.Log(effectCount);
        //현재 체크하는 이펙트가 온일 경우 
        hitEffectpullingArray[effectCount].transform.position = _hitpos;
        hitEffectpullingArray[effectCount].SetActive(true);
        effectCount++;
    }
}
